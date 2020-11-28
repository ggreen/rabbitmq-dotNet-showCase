using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace rabbit_api.API
{
    public abstract class RabbitBuilder
    {

        internal readonly IModel channel;
        /// <summary>
        /// The client can request that messages be sent in advance so that when the client 
        /// finishes processing a message, the following message is already held locally, 
        /// rather than needing to be sent down the channel. Prefetching gives a performance improvement. 
        /// This field specifies the prefetch window size in octets. 
        /// The server will send a message in advance if it is equal to or smaller in size than the available prefetch 
        /// size (and also falls into other prefetch limits). 
        /// May be set to zero, meaning "no specific limit", although other prefetch limits may still apply. 
        /// The prefetch-size is ignored if the no-ack option is set.
        /// The server MUST ignore this setting when the client is not processing any messages - i.e. 
        /// the prefetch size does not limit the transfer of single messages to a client, only the sending in advance of more messages while the client still has one or more unacknowledged messages.
        /// </summary>
        private readonly uint qosPrefetchSize =0;
        private readonly bool qosGlobal = false;
        internal HashSet<Tuple<string,string>> queues = new HashSet<Tuple<string,string>>();

        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }

        public ushort QosPreFetchLimit { get; internal set; }

        public ISet<Tuple<string,string>> Queues { get { return queues; } }

        public bool QueueExclusive { get; internal set; }
        public IDictionary<string, object> QueueArguments { get; internal set; }

        internal RabbitBuilder(IModel channel, ushort qosPreFetchLimit)
        {
            this.channel = channel;
            Durable = true;
            this.QosPreFetchLimit = qosPreFetchLimit;
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

            this.channel.BasicQos(this.qosPrefetchSize, this.QosPreFetchLimit,this.qosGlobal);

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