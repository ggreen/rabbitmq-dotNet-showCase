using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using Imani.Solutions.Core.API.Util;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

/// <summary>
/// RabbitMQ facade interface wrapper
/// Author: Gregory Green
/// </summary>
namespace rabbit_api.API
{
    public class Rabbit : IDisposable
    {
        private readonly IConnectionFactory factory;

        private readonly IList<Uri> endpoints = null;
        
        private static readonly int DEFAULT_CONNECTION_RETRY_SECS = 15;
        private IConnection connection;

        private static ConfigSettings config = new ConfigSettings();

        internal ushort QosPreFetchLimit { get;  private set; }

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
                Password = new string(config.GetPropertyPassword("RABBIT_PASSWORD","".ToCharArray()))
            },
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
                Ssl = new SslOption(){
                    Enabled = sslEnabled,
                 AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors} ,

                NetworkRecoveryInterval = TimeSpan.FromSeconds(networkRecoveryIntervalSecs)
            },
            qosPreFetchLimit
            )
        {
            this.endpoints = endpoints;
        }

        internal Rabbit(IConnectionFactory factory,ushort  qosPreFetchLimit)
        {
            this.factory = factory;

            this.QosPreFetchLimit = qosPreFetchLimit;

            if(this.endpoints != null && this.endpoints.Count > 0)
            {
                IList<AmqpTcpEndpoint> amqpEndpoints = new List<AmqpTcpEndpoint>(this.endpoints.Count);
                foreach( Uri uri in this.endpoints)
                {
                    amqpEndpoints.Add(new AmqpTcpEndpoint(uri.Host,uri.Port));
                }

                connection = factory.CreateConnection(amqpEndpoints);
            }
            else
            {
                connection = factory.CreateConnection();
            }

            connection.ConnectionBlocked += HandleBlocked;
            connection.ConnectionUnblocked += HandleUnblocked;
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
            return new RabbitConsumerBuilder(connection.CreateModel(),QosPreFetchLimit);
        }

        public RabbitPublisherBuilder PublishBuilder()
        {
            return new RabbitPublisherBuilder(connection.CreateModel(),QosPreFetchLimit);
        }

        public static Rabbit Connect()
        {
            var config = new ConfigSettings();
            int networkRecoveryIntervalSecs = config.GetPropertyInteger("RABBIT_CONNECTION_RETRY_SECS",DEFAULT_CONNECTION_RETRY_SECS);
            string clientName = config.GetProperty("RABBIT_CLIENT_NAME");
            ushort qosPreFetchLimit = ushort.Parse(config.GetProperty("RABBIT_PREFETCH_LIMIT","1000"));

            string urisText = config.GetPropertySecret("RABBIT_URIS", "");
            if(urisText.Length > 1)
            {
                bool sslEnabled = urisText.ToLower().Contains("amqps:");
                return new Rabbit(ParseUrisToEndPoints(urisText),sslEnabled,clientName,networkRecoveryIntervalSecs,qosPreFetchLimit);
            }
     
            string host = config.GetProperty("RABBIT_HOST", "localhost");
            
            int port = config.GetPropertyInteger("RABBIT_PORT", 5672);
            string virtualHost = config.GetProperty("RABBIT_VIRTUAL_HOST","/");
            string userName = config.GetProperty("RABBIT_USERNAME");
            char[] password = config.GetProperty("RABBIT_PASSWORD").ToCharArray();
            return new Rabbit(host, port,virtualHost, clientName,networkRecoveryIntervalSecs,qosPreFetchLimit,userName,password);
        }

        internal static IList<Uri> ParseUrisToEndPoints(string urisText)
        {
            if(String.IsNullOrWhiteSpace(urisText))
            {
                throw new ArgumentException("URIS required");
            }

            string[] urisArray = urisText.Split(",");
            IList<Uri> list = new List<Uri>();
            foreach(string uri in urisArray)
            {
                list.Add(new Uri(uri));
            }

            return list;

        }

        public void Dispose()
        {
            this.connection.Close();
        }
    }
}