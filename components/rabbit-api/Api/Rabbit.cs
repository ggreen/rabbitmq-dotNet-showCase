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
/// Author: Gregory Green
/// </summary>
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
                Password = new string(config.GetPropertyPassword("RABBIT_PASSWORD","".ToCharArray()))
            },
            qosPreFetchLimit
            )
        {
            
        }
         private Rabbit(Uri uri, Boolean sslEnabled, string clientProvidedName, int networkRecoveryIntervalSecs, ushort qosPreFetchLimit) : 
             this(new ConnectionFactory()
            {
                HostName = "gregoryg-a01.vmware.com",
                Uri = uri,
                ClientProvidedName = clientProvidedName,
                AutomaticRecoveryEnabled = true,
                Ssl = new SslOption(){Enabled = sslEnabled,
                ServerName = "gregoryg-a01.vmware.com",
                // Certs = new X509CertificateCollection( new X509Certificate[] {X509Certificate.CreateFromCertFile("/Users/devtools/integration/messaging/rabbit/rabbit-devOps/tls-gen/basic/result/client_key.pem")}),
                // Certs = new X509CertificateCollection( new X509Certificate[] {.CreateFromCertFile("/Users/devtools/integration/messaging/rabbit/rabbit-devOps/tls-gen/basic/result/client_key.p12")}),
                // CertPath = "/Users/devtools/integration/messaging/rabbit/rabbit-devOps/tls-gen/basic/result/client_key.pem",
                CertPath = "/Users/devtools/integration/messaging/rabbit/rabbit-devOps/tls-gen/basic/result/client_key.p12",
                // CertPath = "/Users/devtools/integration/messaging/rabbit/rabbit-devOps/tls-gen/basic/client/keycert.p12",
                Version = SslProtocols.Tls12,
                // AcceptablePolicyErrors =  SslPolicyErrors.RemoteCertificateNameMismatch,
                 AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors,
                // CheckCertificateRevocation = false,
                // Version = SslProtocols.Tls11,
                CertPassphrase = "bunnies"
                // CertPassphrase = "MySecretPassword"
                } ,

                NetworkRecoveryInterval = TimeSpan.FromSeconds(networkRecoveryIntervalSecs)
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
            int networkRecoveryIntervalSecs = config.GetPropertyInteger("RABBIT_CONNECTION_RETRY_SECS",DEFAULT_CONNECTION_RETRY_SECS);
            string clientName = config.GetProperty("RABBIT_CLIENT_NAME");
            ushort qosPreFetchLimit = ushort.Parse(config.GetProperty("RABBIT_PREFETCH_LIMIT","1000"));

            string uriText = config.GetProperty("RABBIT_URI", "");
            if(uriText.Length > 1)
            {
                bool sslEnabled = uriText.ToLower().Contains("amqps:");

                Console.WriteLine($"sslEnabled :{sslEnabled}");

                return new Rabbit(new Uri(uriText),sslEnabled,clientName,networkRecoveryIntervalSecs,qosPreFetchLimit);
            }
     
            string host = config.GetProperty("RABBIT_HOST", "localhost");
            
            int port = config.GetPropertyInteger("RABBIT_PORT", 5672);
            string virtualHost = config.GetProperty("RABBIT_VIRTUAL_HOST","/");
            string userName = config.GetProperty("RABBIT_USERNAME");
            char[] password = config.GetProperty("RABBIT_PASSWORD").ToCharArray();
            return new Rabbit(host, port,virtualHost, clientName,networkRecoveryIntervalSecs,qosPreFetchLimit,userName,password);
        }

        public void Dispose()
        {
            this.connection.Close();
        }
    }
}