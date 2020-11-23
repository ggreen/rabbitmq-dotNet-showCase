using System;
using RabbitMQ.Client;

namespace rabbit_demo_producer.App
{
    public class RabbitPublisher
    {
        
        private IModel channel;
        private string exchange;

        public RabbitPublisher(IModel channel, string exchange, String queue)
        {
            this.channel = channel;
            this.exchange = exchange;
        }
        
        public void Publish(byte[] body)
        {

            channel.BasicPublish(exchange: exchange,
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine(" [x] Sent {0}", body.Length);
        }
    }
}