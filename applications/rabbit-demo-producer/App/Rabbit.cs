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

        private Rabbit(string host) : this(new ConnectionFactory() { HostName = host})
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
            string host = new ConfigSettings().GetProperty("RABBIT_HOST","localhost");
            return new Rabbit(host);
        }

      
    }
}