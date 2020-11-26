using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace rabbit_api.API
{
    public abstract class RabbitBuilder
    {

        internal readonly IModel channel;
        internal HashSet<Tuple<string,string>> queues = new HashSet<Tuple<string,string>>();

        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }

        public ISet<Tuple<string,string>> Queues { get { return queues; } }

        public bool QueueExclusive { get; internal set; }
        public IDictionary<string, object> QueueArguments { get; internal set; }

        public RabbitBuilder(IModel channel)
        {
            this.channel = channel;
            Durable = true;

            this.QueueArguments = new Dictionary<string,object>();
        }
        public RabbitExchangeType ExchangeType { get; set; }

        public string Exchange { get; internal set; }

        internal void AddQueueRoutingKey(string queue, string routingKey)
        {
            if(String.IsNullOrEmpty(queue))
                throw new ArgumentException("queue cannot be null or empty");

            if(routingKey == null)
                throw new ArgumentException("routingKey cannot be null when adding a queue");

            this.queues.Add(new Tuple<string, string>(queue,routingKey));
        }


        internal void ConstructExchange()
        {
            if (String.IsNullOrEmpty(Exchange))
                throw new ArgumentException("Set Exchange required");

            

            this.channel.ExchangeDeclare
            (Exchange, ExchangeType.ToString(), Durable, AutoDelete);
        }
        internal void ConstructQueues()
        {
            CheckQueues();

            foreach (var queue in queues)
            {
                this.channel.QueueDeclare(queue.Item1, Durable, QueueExclusive, AutoDelete,QueueArguments);
                this.channel.QueueBind(queue.Item1, Exchange, queue.Item2);
            }
        }

        internal void CheckQueues()
        {
            if (this.queues.Count == 0)
                throw new ArgumentException("At Least 1 queue must be added");

        }
    }
}