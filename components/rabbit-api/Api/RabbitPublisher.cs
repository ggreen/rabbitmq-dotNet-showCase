using System;
using RabbitMQ.Client;

namespace rabbit_api.API
{
    public class RabbitPublisher
    {
        private readonly IBasicProperties basicProperties;
        private IModel channel;
        private string exchange;

        public RabbitPublisher(IModel channel, string exchange,IBasicProperties basicProperties)
        {
            this.channel = channel;
            this.exchange = exchange;
            this.basicProperties = basicProperties;
        }
        
        public void Publish(byte[] body, string routingKey)
        {

            channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: basicProperties,
                                 body: body);

        }
    }
}