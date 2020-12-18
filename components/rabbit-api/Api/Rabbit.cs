using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using Imani.Solutions.Core.API.Util;

namespace rabbit_api.API
{
    public class Rabbit : IDisposable
    {
        private readonly IConnectionFactory factory;
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
                Password = new string(config.GetPropertyPassword("ENCRYPTED_PASSWORD"))
            },
            qosPreFetchLimit
            )
        {
            
        }

        internal Rabbit(IConnectionFactory factory,ushort  qosPreFetchLimit)
        {
            this.factory = factory;

            this.QosPreFetchLimit = qosPreFetchLimit;

            connection = factory.CreateConnection();

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

            string host = config.GetProperty("RABBIT_HOST", "localhost");
            int port = config.GetPropertyInteger("RABBIT_PORT", 5672);
            int networkRecoveryIntervalSecs = config.GetPropertyInteger("RABBIT_CONNECTION_RETRY_SECS",DEFAULT_CONNECTION_RETRY_SECS);
            string virtualHost = config.GetProperty("RABBIT_VIRTUAL_HOST","/");
            string clientName = config.GetProperty("RABBIT_CLIENT_NAME");
            string userName = config.GetProperty("RABBIT_USERNAME");
            char[] password = config.GetProperty("ENCRYPTED_PASSWORD").ToCharArray();
            ushort qosPreFetchLimit = ushort.Parse(config.GetProperty("RABBIT_PREFETCH_LIMIT","1000"));
            return new Rabbit(host, port,virtualHost, clientName,networkRecoveryIntervalSecs,qosPreFetchLimit,userName,password);
        }

        public void Dispose()
        {
            this.connection.Close();
        }
    }
}