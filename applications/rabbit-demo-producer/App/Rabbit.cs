using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;

namespace rabbit_demo_producer.App
{
    public class Rabbit
    {
        private string host;
        private IConnectionFactory factory;
        private IConnection connection;

        private IModel channel;

        private string exchange;
        private string  queue;

        private string routingKey = "";

        private Rabbit()
        {
            // this.exchange = exchange;
            // this.queue = queue;

            // this.host = host;

            factory = new ConnectionFactory() { HostName = host};

            connection = factory.CreateConnection();
            // channel = ;

            // channel.ExchangeDeclare(exchange : exchange,
            //         type: "direct",
            //         durable : true,
            //         autoDelete : false);

            //  channel.QueueDeclare(queue: queue,
            //             durable: true,
            //             exclusive: false,
            //             autoDelete: false);

            // channel.QueueBind(queue,
            //                 exchange,
            //                 routingKey);
        }

        public RabbitConsumerBuilder ConsumerBuilder()
        {
            return new RabbitConsumerBuilder(connection.CreateModel());
        }

        public RabbitPublisherBuilder PublishBuilder()
        {
            return new RabbitPublisherBuilder();
        }

        public static Rabbit Connect()
        {
            throw new NotImplementedException();
        }

      
    }
}