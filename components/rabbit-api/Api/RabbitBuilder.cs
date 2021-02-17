using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

/// <summary>
/// Author: Gregory Green
/// </summary>
namespace rabbit_api.API
{
    public abstract class RabbitBuilder
    {
        public const string QUORUM_QUEUE_TYPE = "quorum";
        public const string CLASSIC_QUEUE_TYPE = "classic";
        private const string QUEUE_MODE_PROP = "x-queue-mode";
        private const string QUORUM_QUEUE_MAX_IN_MEMORY_LEN_PROP = "x-max-in-memory-length";
        private const string QUEUE_TYPE_PROP = "x-queue-type";
        //private IConnection connection;

        private IRabbitConnectionCreator creator;
       // protected IModel channel;

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
        private readonly uint qosPrefetchSize = 0;
        private readonly bool qosGlobal = false;
        protected HashSet<Tuple<string, string>> queues = new HashSet<Tuple<string, string>>();

        internal RabbitBuilder(IRabbitConnectionCreator connectionCreator, ushort qosPreFetchLimit)
        {
            Init(connectionCreator,qosPreFetchLimit);

        }
        internal void Init(IRabbitConnectionCreator connectionCreator, ushort qosPreFetchLimit)
        {
            this.creator = connectionCreator;
            //this.connection = connectionCreator.CreateConnection();
            
            // connection.ConnectionShutdown += HandleShutdown;

            //this.channel = this.connection.CreateModel();

            Durable = true;
            this.QosPreFetchLimit = qosPreFetchLimit;
            this.QueueArguments = new Dictionary<string, object>();
        }

        public virtual void HandleShutdown(object sender, ShutdownEventArgs e)
        {
            
        }

        

        public bool IsLazyQueues { get; internal set; }

        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }

        public ushort QosPreFetchLimit { get; internal set; }

        public ISet<Tuple<string, string>> Queues { get { return queues; } }

        public bool QueueExclusive { get; internal set; }
        public IDictionary<string, object> QueueArguments { get; internal set; }

        internal bool IsQuorumQueues
        {
            get
            {
                return this.QueueArguments.ContainsKey(QUEUE_TYPE_PROP) &&
                        this.QueueArguments[QUEUE_TYPE_PROP].Equals(QUORUM_QUEUE_TYPE);
            }
        }

        
        public RabbitExchangeType ExchangeType { get; set; }

        public string Exchange { get; internal set; }

        internal void AddQueueRoutingKey(string queue, string routingKey)
        {
            if (String.IsNullOrEmpty(queue))
                throw new ArgumentException("queue cannot be null or empty");

            if (routingKey == null)
                throw new ArgumentException("routingKey cannot be null when adding a queue");

            this.queues.Add(new Tuple<string, string>(queue, routingKey));
        }


        internal void ConstructExchange()
        {
            if (String.IsNullOrEmpty(Exchange))
                throw new ArgumentException("Set Exchange required");

            creator.GetChannel().BasicQos(this.qosPrefetchSize, this.QosPreFetchLimit, this.qosGlobal);


            try{
                 creator.GetChannel().ExchangeDeclare
                (Exchange, ExchangeType.ToString(), Durable, AutoDelete);
                
            }
            catch(OperationInterruptedException e)
            {
                Console.WriteLine($"WARNING: {e.Message} so using EXISTING exchange");
                creator.GetChannel().ExchangeDeclarePassive(Exchange);
            }
        }
        internal void ConstructQueues()
        {
            //CheckQueues();

            foreach (var queue in queues)
            {
                try
                {
                    creator.GetChannel().QueueDeclare(queue.Item1, Durable, QueueExclusive, AutoDelete, QueueArguments);
                }
                catch(OperationInterruptedException e) {

                    Console.WriteLine($"WARNING: {e.Message} so using EXISTING queue");

                    creator.GetChannel().QueueDeclarePassive(queue.Item1);
                }

                creator.GetChannel().QueueBind(queue.Item1, Exchange, queue.Item2);
            }
        }

        internal void CheckQueues()
        {
            if (this.queues.Count == 0)
                throw new ArgumentException("At Least 1 queue must be added");

        }

        internal void AssignQueueTypeArgToQuorum()
        {
            this.QueueArguments[QUEUE_TYPE_PROP] = QUORUM_QUEUE_TYPE;

            if (this.IsLazyQueues)
            {
                if (this.IsQuorumQueues)
                {
                    this.QueueArguments.Remove(QUEUE_MODE_PROP);
                }

                AssignQuorumQueueMaxInMemoryZero();

            }
        }

        internal void AssignQueueType(RabbitQueueType queueType)
        {
            switch(queueType)
           {
               case RabbitQueueType.quorum:    this.AssignQueueTypeArgToQuorum();
               break;
               case RabbitQueueType.classic: this.AssignQueueTypeArgClassic();
               break;
           }
        }


        internal void AssignQueueTypeArgClassic()
        {
            this.QueueArguments[QUEUE_TYPE_PROP] = CLASSIC_QUEUE_TYPE;

        }

        internal void AssignQuorumQueueMaxInMemoryZero()
        {
            this.QueueArguments[QUORUM_QUEUE_MAX_IN_MEMORY_LEN_PROP] = "0";
        }

        internal void AssignQueueModeArgToLazy()
        {
            IsLazyQueues = true;

            if (IsQuorumQueues)
            {
                AssignQuorumQueueMaxInMemoryZero();
            }
            else
            {
                this.QueueArguments[QUEUE_MODE_PROP] = "lazy";
            }
        }
    }
}