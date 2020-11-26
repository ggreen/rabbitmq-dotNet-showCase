using System;
using Imani.Solutions.Core.API.Util;
using RabbitMQ.Client;

namespace rabbit_api.API
{
    public class RabbitPublisher : IDisposable
    {
        private readonly IBasicProperties basicProperties;
        private readonly IModel channel;
        private readonly string exchange;
        private readonly bool confirmPublish;

        private TimeSpan waitFromConfirmaiton;

        private int WAIT_FOR_CONFIRMATION_SECONDS = new ConfigSettings().GetPropertyInteger("WAIT_FOR_CONFIRMATION_SECONDS",30);


        public RabbitPublisher(IModel channel, string exchange, IBasicProperties basicProperties, bool confirmPublish)
        {
            this.channel = channel;
            this.exchange = exchange;
            this.basicProperties = basicProperties;
            this.confirmPublish = confirmPublish;

            if(confirmPublish)
                waitFromConfirmaiton  = new TimeSpan(0,0,WAIT_FOR_CONFIRMATION_SECONDS);
        }

        public void Dispose()
        {
            this.channel.Close();
        }

        public void Publish(byte[] body, string routingKey)
        {
            if (body == null || body.Length == 0)
                throw new ArgumentException("Body cannot be null or empty");

            if (routingKey == null)
                throw new ArgumentException("routingKey cannot be null");

            channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: basicProperties,
                                 body: body);

            if (this.confirmPublish)
            {
                channel.WaitForConfirms(waitFromConfirmaiton);

            }

        }
    }
}