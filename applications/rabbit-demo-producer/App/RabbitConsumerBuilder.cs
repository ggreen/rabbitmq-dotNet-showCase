using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_demo_producer.App
{
    public class RabbitConsumerBuilder : RabbitBuilder
    {
        public bool AutoAck { get; set; }

        public RabbitConsumerBuilder(IModel channel) : base(channel)
        {
        }

        public RabbitConsumerBuilder SetExchange(string exchange)
        {
            this.Exchange = exchange;

            return this;
        }

        public RabbitConsumerBuilder AddQueue(string queue)
        {
            base.queues.Add(queue);
            return this;
        }
        public RabbitConsumer Build()
        {
            CheckQueues();

            if (queues.Count > 1)
                throw new Exception("If more than one queue, call Build(queueName)");

            var i = queues.GetEnumerator();
            i.MoveNext();

            return Build(i.Current);
        }
        public RabbitConsumer Build(string consumerQueue)
        {
            Construct();

            return new RabbitConsumer(this.channel, consumerQueue, AutoAck);
        }

      
    }
}