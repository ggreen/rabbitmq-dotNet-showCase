using System;
using RabbitMQ.Client;

namespace rabbit_demo_producer.App
{
    public class RabbitPublisherBuilder : RabbitBuilder
    {
        public bool Persistent { get; private set; }

        public RabbitPublisherBuilder(IModel channel) : base(channel)
        {
            Persistent = true;
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

        public RabbitPublisherBuilder SetPersistent(bool persistent)
        {
            this.Persistent = persistent;
            return this;
        }
        public RabbitPublisher Build()
        {
            Construct();

            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = Persistent;

            return new RabbitPublisher(this.channel,Exchange,basicProperties);
        }
    }
}