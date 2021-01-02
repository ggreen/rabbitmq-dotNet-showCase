using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_api.API
{
    /// <summary>
    /// Implements a builder pattern for creating a publisher with needed settings.
    /// 
    /// author: Gregory Green
    /// </summary>
    public class RabbitPublisherBuilder : RabbitBuilder
    {
        public bool Persistent { get; private set; }
        public bool IsConfirmPublish { get; private set; }

        public RabbitPublisherBuilder(IModel channel, ushort qosPreFetchLimit) : base(channel, qosPreFetchLimit)
        {
            Persistent = true;
            channel.BasicReturn += HandleReturn;
        }

        private void HandleReturn(object sender, BasicReturnEventArgs args)
        {
            Console.WriteLine($"WARNING: Returned Reply:{args.ReplyText} RoutingKey:{args.RoutingKey} Exchange: {args.Exchange}");
        }

        public RabbitPublisherBuilder SetExchange(string exchange)
        {
            base.Exchange = exchange;
            return this;
        }

        public RabbitPublisherBuilder AddQueue(string queue, String routingKey)
        {
            this.AddQueueRoutingKey(queue, routingKey);
            return this;
        }

        public RabbitPublisherBuilder SetPersistent(bool persistent)
        {
            this.Persistent = persistent;
            return this;
        }
        public RabbitPublisher Build()
        {
            if (IsConfirmPublish)
            {
                this.channel.ConfirmSelect();
            }

            ConstructExchange();


            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = Persistent;

            basicProperties.DeliveryMode = 2; // persistent

            return new RabbitPublisher(this.channel, Exchange, basicProperties, IsConfirmPublish);
        }

        public RabbitPublisherBuilder SetExchangeType(RabbitExchangeType type)
        {
            ExchangeType = type;
            return this;
        }

        public RabbitPublisherBuilder SetConfirmPublish()
        {
            this.IsConfirmPublish = true;
            return this;
        }

        public RabbitPublisherBuilder SetQosPreFetchLimit(ushort qos)
        {
            this.QosPreFetchLimit = qos;
            return this;
        }
    }
}