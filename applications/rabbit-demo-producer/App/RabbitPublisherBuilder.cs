using System;
using RabbitMQ.Client;

namespace rabbit_demo_producer.App
{
    public class RabbitPublisherBuilder : RabbitBuilder
    {
        private bool persistent = true;

        public RabbitPublisherBuilder(IModel channel) : base(channel)
        {
        }

        public RabbitPublisherBuilder SetExchange(string exchange)
        {
            base.Exchange = exchange;
            return this;
        }

        public RabbitPublisherBuilder AddQueue(string queue)
        {
            base.queues.Add(queue);
            return this;
        }

        public RabbitPublisherBuilder Persistent(bool persistent)
        {
            this.persistent = persistent;
            return this;
        }
        public RabbitPublisher Build()
        {
            Construct();

            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = persistent;

            return new RabbitPublisher(this.channel,Exchange,basicProperties);
        }
    }
}