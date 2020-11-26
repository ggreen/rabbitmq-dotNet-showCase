using System;
using RabbitMQ.Client;

namespace rabbit_api.API
{
    public class RabbitPublisher : IDisposable
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

        public void Dispose()
        {
            this.channel.Close();
        }

        public void Publish(byte[] body, string routingKey)
        {
            if(body == null || body.Length == 0)
                throw new ArgumentException("Body cannot be null or empty");

            if(routingKey == null)
                throw new ArgumentException("routingKey cannot be null");

            channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: basicProperties,
                                 body: body);
        }
    }
}