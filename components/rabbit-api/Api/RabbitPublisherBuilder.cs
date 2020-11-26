using System;
using RabbitMQ.Client;

namespace rabbit_api.API
{
    public class RabbitPublisherBuilder : RabbitBuilder
    {
        public bool Persistent { get; private set; }
        public bool IsConfirmPublish { get; private set; }

        public RabbitPublisherBuilder(IModel channel) : base(channel)
        {
            Persistent = true;
        }

        public RabbitPublisherBuilder SetExchange(string exchange)
        {
            base.Exchange = exchange;
            return this;
        }

        public RabbitPublisherBuilder AddQueue(string queue, String routingKey)
        {
            this.AddQueueRoutingKey(queue,routingKey);
            return this;
        }

        public RabbitPublisherBuilder SetPersistent(bool persistent)
        {
            this.Persistent = persistent;
            return this;
        }
        public RabbitPublisher Build()
        {
            if(IsConfirmPublish)
            {
                this.channel.ConfirmSelect();    
            }

            ConstructExchange();
         

            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = Persistent;

            return new RabbitPublisher(this.channel,Exchange,basicProperties,IsConfirmPublish);
        }

        public RabbitPublisherBuilder SetExchangeType(RabbitExchangeType type)
        {
            ExchangeType = type;
            return this;
        }

        public RabbitPublisherBuilder SetConfirmPublish()
        {
            this.IsConfirmPublish = true;
            return this;
        }
    }
}