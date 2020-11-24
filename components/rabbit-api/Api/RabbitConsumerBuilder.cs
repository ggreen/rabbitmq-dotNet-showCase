using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_api.API
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

        public RabbitConsumerBuilder AddQueue(string queue, string routingKey)
        {
       
            base.AddQueueRoutingKey(queue,routingKey);
            return this;
        }
        public RabbitConsumerBuilder SetExchangeType(RabbitExchangeType type)
        {
            ExchangeType = type;
           return this;
        }
        public RabbitConsumer Build()
        {
            
            if (queues.Count < 1)
                throw new ArgumentException("If more than one queue, call Build(queueName)");

            var i = queues.GetEnumerator();
            i.MoveNext();

            return Build(i.Current.Item1);
        }
        public RabbitConsumer Build(string consumerQueue)
        {
            ConstructExchange();
            ConstructQueues();

            return new RabbitConsumer(this.channel, consumerQueue, AutoAck);
        }

    }
}