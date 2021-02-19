using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using Imani.Solutions.Core.API.Util;
using System.Net.Security;
using System.Threading;

/// <summary>
/// RabbitMQ facade interface wrapper
/// Author: Gregory Green
/// </summary>
namespace rabbit_api.API
{
    public class Rabbit : IRabbitConnectionCreator
    {
        private readonly IConnectionFactory factory;
        private IConnection connection = null;
        private readonly IList<Uri> endpoints = null;

        private static readonly int DEFAULT_CONNECTION_RETRY_SECS = 15;
        // private IConnection connection;

        private static ConfigSettings config = new ConfigSettings();
        private IModel channel;

        internal ushort QosPreFetchLimit { get; private set; }

        private Rabbit(string host, int port, string virtualHost, string clientProvidedName, int networkRecoveryIntervalSecs, ushort qosPreFetchLimit, string userName, char[] password) :
             this(new ConnectionFactory()
             {
                 HostName = host,
                 VirtualHost = virtualHost,
                 Port = port,
                 ClientProvidedName = clientProvidedName,
                 AutomaticRecoveryEnabled = true,
                 NetworkRecoveryInterval = TimeSpan.FromSeconds(networkRecoveryIntervalSecs),
                 UserName = userName,
                 Password = new string(config.GetPropertyPassword("RABBIT_PASSWORD", "".ToCharArray()))
             },
            null,
            false,
            qosPreFetchLimit
            )
        {
        }

        private Rabbit(IList<Uri> endpoints, Boolean sslEnabled, string clientProvidedName, int networkRecoveryIntervalSecs, ushort qosPreFetchLimit) :
             this(new ConnectionFactory()
             {
                 Uri = endpoints[0],
                 ClientProvidedName = clientProvidedName,
                 AutomaticRecoveryEnabled = true,
                 Ssl = CreateSslOption(sslEnabled),

                 NetworkRecoveryInterval = TimeSpan.FromSeconds(networkRecoveryIntervalSecs)
             },
            endpoints,
            sslEnabled,
            qosPreFetchLimit
            )
        {
        }

        internal static IList<AmqpTcpEndpoint> ToAmqpTcpEndpoints(IList<Uri> endpoints)
        {
            if (endpoints == null)
                return null;

            IList<AmqpTcpEndpoint> amqpEndpoints = new List<AmqpTcpEndpoint>(endpoints.Count);
            foreach (Uri uri in endpoints)
            {
                amqpEndpoints.Add(new AmqpTcpEndpoint(uri, CreateSslOption(uri)));
            }
            return amqpEndpoints;
        }

        internal static SslOption CreateSslOption(Uri uri)
        {
            bool sslEnabled = uri.OriginalString.ToLower().Contains("amqps");
            return CreateSslOption(sslEnabled);
        }
        internal static SslOption CreateSslOption(bool sslEnabled)
        {
            return new SslOption()
            {
                Enabled = sslEnabled,
                AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors
            };
        }

        internal Rabbit(IConnectionFactory factory, IList<Uri> endpoints, Boolean sslEnabled, ushort qosPreFetchLimit)
        {
            this.factory = factory;

            this.QosPreFetchLimit = qosPreFetchLimit;
            this.endpoints = endpoints;
            // this.connection = CreateConnection();
        }

        public IConnection GetConnection()
        {
            if (this.connection == null)
            {
                this.connection = NewConnection();
                return this.connection;
            }

            if (this.connection.IsOpen)
            {
                return this.connection;
            }
            else
            {

                try
                {
                    Console.WriteLine("WARNING: DISPOSING connection");
                    this.connection.Dispose();
                }
                catch { }


                this.ResetConnection();

                return this.connection;
            }
        }

        private void ResetConnection()
        {
           
            while (this.connection == null || !this.connection.IsOpen)
            {
                try
                {
                    this.connection = NewConnection();
                    this.channel = connection.CreateModel();
                    Console.WriteLine("INFO: connected to cluster");
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine($"WARNING: {e.Message} restarting in {DEFAULT_CONNECTION_RETRY_SECS} seconds");
                    Thread.Sleep(TimeSpan.FromSeconds(DEFAULT_CONNECTION_RETRY_SECS));
                }
            }

        }

        private IConnection NewConnection()
        {
            factory.HandshakeContinuationTimeout = TimeSpan.FromSeconds(DEFAULT_CONNECTION_RETRY_SECS);
            factory.ContinuationTimeout = TimeSpan.FromSeconds(DEFAULT_CONNECTION_RETRY_SECS);

            IConnection rabbitConnection = null;
            if (this.endpoints != null && this.endpoints.Count > 0)
            {
                rabbitConnection = factory.CreateConnection(
                    ToAmqpTcpEndpoints(endpoints));
            }
            else
            {
                rabbitConnection = factory.CreateConnection();
            }

            rabbitConnection.ConnectionBlocked += HandleBlocked;
            rabbitConnection.ConnectionUnblocked += HandleUnblocked;

            return rabbitConnection;
        }

        private void HandleBlocked(object sender, ConnectionBlockedEventArgs args)
        {
            Console.WriteLine("WARNING: Connection is now blocked");
        }

        public void HandleUnblocked(object sender, EventArgs args)
        {
            Console.WriteLine("INFO: Connection is now unblocked");
        }

        public RabbitConsumerBuilder ConsumerBuilder()
        {
            return new RabbitConsumerBuilder(this, QosPreFetchLimit);
        }

        public RabbitPublisherBuilder PublishBuilder()
        {
            return new RabbitPublisherBuilder(this, QosPreFetchLimit);
        }

        public static Rabbit Connect()
        {
            var config = new ConfigSettings();
            int networkRecoveryIntervalSecs = config.GetPropertyInteger("RABBIT_CONNECTION_RETRY_SECS", DEFAULT_CONNECTION_RETRY_SECS);
            string clientName = config.GetProperty("RABBIT_CLIENT_NAME");
            ushort qosPreFetchLimit = ushort.Parse(config.GetProperty("RABBIT_PREFETCH_LIMIT", "1000"));

            string urisText = config.GetPropertySecret("RABBIT_URIS", "");
            if (urisText.Length > 1)
            {
                bool sslEnabled = urisText.ToLower().Contains("amqps:");
                return new Rabbit(ParseUrisToEndPoints(urisText), sslEnabled, clientName, networkRecoveryIntervalSecs, qosPreFetchLimit);
            }

            string host = config.GetProperty("RABBIT_HOST", "localhost");

            int port = config.GetPropertyInteger("RABBIT_PORT", 5672);
            string virtualHost = config.GetProperty("RABBIT_VIRTUAL_HOST", "/");
            string userName = config.GetProperty("RABBIT_USERNAME");
            char[] password = config.GetProperty("RABBIT_PASSWORD").ToCharArray();
            return new Rabbit(host, port, virtualHost, clientName, networkRecoveryIntervalSecs, qosPreFetchLimit, userName, password);
        }

        internal static IList<Uri> ParseUrisToEndPoints(string urisText)
        {
            if (String.IsNullOrWhiteSpace(urisText))
            {
                throw new ArgumentException("URIS required");
            }

            string[] urisArray = urisText.Split(",");
            IList<Uri> list = new List<Uri>();
            foreach (string uri in urisArray)
            {
                list.Add(new Uri(uri));
            }

            return list;

        }

        public void Dispose()
        {
            Console.WriteLine("WARNING: %%%%%% dispose connection");
            if (this.channel != null)
                this.channel.Dispose();

            if (this.connection != null)
                this.connection.Dispose();
        }

        public IModel GetChannel()
        {
            if (this.channel == null)
            {
                this.channel = GetConnection().CreateModel();
                return this.channel;
            }

            if (!this.channel.IsClosed && this.connection.IsOpen)
            {
                if(this.channel.NextPublishSeqNo == 0)
                {
                    if(this.channel != null)
                    {
                        Console.WriteLine("WARNING:NextPublishSeqNo == 0");
                        this.channel.Dispose();
                        this.channel = null;
                    }
                        

                    if(this.connection != null){
                        this.connection.Dispose();
                        this.connection = null;
                    }
                    
                    this.ResetConnection();
                    this.channel.ConfirmSelect();
                }
                return this.channel;
            }
            else
            {
                try { 
                    Console.WriteLine("WARNING: DISPOSING connection");

                    this.channel.Dispose(); } catch { };
                
                this.channel = GetConnection().CreateModel();
                return this.channel;
            }
        }
    }
}