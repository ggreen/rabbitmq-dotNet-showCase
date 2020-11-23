using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_demo_producer.App
{
    public class RabbitConsumerBuilder
    {
        private readonly IModel channel;


        private HashSet<string> queues = new HashSet<string>();

        public RabbitConsumerBuilder(IModel channel)
        {
            this.channel = channel;
        }

        public RabbitExchangeType ExchangeType { get; set; }

        public string Exchange { get; internal set; }

        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }

        private IDictionary<string, object> arguments;

        public ISet<string> Queues { get { return queues; } }

        public bool QueueExclusive { get; internal set; }
        public string RoutingKey { get; internal set; }

        public RabbitConsumerBuilder SetExchange(string exchange)
        {
            this.Exchange = exchange;

            return this;
        }

        public RabbitConsumerBuilder AddQueue(string queue)
        {
            this.queues.Add(queue);
            return this;
        }

        public RabbitConsumer Build()
        {
            if(String.IsNullOrEmpty(Exchange))
                throw new ArgumentException("Set Exchange required");


            if(this.queues.Count == 0)
                throw new ArgumentException("At Least 1 queue must be added");


            this.channel.ExchangeDeclare
            (Exchange, ExchangeType.ToString(), Durable, AutoDelete, arguments);
        

            foreach (var queue in queues)
            {
                this.channel.QueueDeclare(queue,Durable,QueueExclusive,AutoDelete,arguments);
                this.channel.QueueBind(queue,Exchange,RoutingKey,arguments);
            }

            return new RabbitConsumer();
        }
    }
}