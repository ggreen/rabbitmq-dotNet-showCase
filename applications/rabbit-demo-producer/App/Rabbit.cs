using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using Imani.Solutions.Core.API.Util;

namespace rabbit_demo_producer.App
{
    public class Rabbit
    {
        private readonly IConnectionFactory factory;
        private IConnection connection;

        private Rabbit(string host,int port) : this(new ConnectionFactory() { HostName = host, Port = port})
        {
        }

         internal Rabbit(IConnectionFactory factory)
        {
            this.factory = factory;
            connection = factory.CreateConnection();
        }


        public RabbitConsumerBuilder ConsumerBuilder()
        {
            return new RabbitConsumerBuilder(connection.CreateModel());
        }

        public RabbitPublisherBuilder PublishBuilder()
        {
            return new RabbitPublisherBuilder(connection.CreateModel());
        }

        public static Rabbit Connect()
        {
            var config = new ConfigSettings();

            string host = config.GetProperty("RABBIT_HOST","localhost");
            int port = config.GetPropertyInteger("RABBIT_PORT",5672);
            return new Rabbit(host,port);
        }

      
    }
}