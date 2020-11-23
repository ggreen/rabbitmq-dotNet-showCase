using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace rabbit_demo_producer.App
{
    public abstract class RabbitBuilder
    {

        internal readonly IModel channel;
        internal HashSet<string> queues = new HashSet<string>();
        // internal IDictionary<string, object> arguments;

        public RabbitBuilder(IModel channel)
        {
            this.channel = channel;
            this.RoutingKey = "";
            Durable = true;
            
        }
        public RabbitExchangeType ExchangeType { get; set; }

        public string Exchange { get; internal set; }

        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }

        public ISet<string> Queues { get { return queues; } }

        public bool QueueExclusive { get; internal set; }
        public string RoutingKey { get; internal set; }

        internal void Construct()
        {
            if (String.IsNullOrEmpty(Exchange))
                throw new ArgumentException("Set Exchange required");

            CheckQueues();

            this.channel.ExchangeDeclare
            (Exchange, ExchangeType.ToString(), Durable, AutoDelete);


            foreach (var queue in queues)
            {
                this.channel.QueueDeclare(queue, Durable, QueueExclusive, AutoDelete);
                this.channel.QueueBind(queue, Exchange, RoutingKey);
            }
        }

        internal void CheckQueues()
        {
            if (this.queues.Count == 0)
                throw new ArgumentException("At Least 1 queue must be added");

        }
    }
}