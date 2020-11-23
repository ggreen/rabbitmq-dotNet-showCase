using System;

namespace rabbit_demo_producer.App
{
    public class RabbitPublisherBuilder
    {
        public RabbitPublisherBuilder()
        {
        }

        public RabbitPublisherBuilder SetExchange(string topic)
        {
            return this;
        }

        public RabbitPublisherBuilder AddQueue(string queue)
        {
            return this;
        }

        internal RabbitPublisher Build()
        {
            throw new NotImplementedException();
        }
    }
}