using System;
using System.Collections.Generic;
using NServiceBus.Transport;
using NServiceBus.Transport.RabbitMQ;
using RabbitMQ.Client;

namespace nserviceBusProducer
{
    internal class MyRoutingTopology : IRoutingTopology
    {
        private bool createDurableExchangesAndQueues;
        private string exchangeType = "topic";
        private bool durable = true;
        private bool autoDelete = false;
        private IDictionary<string, object> arguments = new Dictionary<string,object>();
        private bool exclusive = false;
        private string exchange;
        private string routingKey;
        private bool mandatory;

        public MyRoutingTopology(bool createDurableExchangesAndQueues)
        {
            this.createDurableExchangesAndQueues = createDurableExchangesAndQueues;
        }

        public void BindToDelayInfrastructure(IModel channel, string address, string deliveryExchange, string routingKey)
        {
            this.exchange = deliveryExchange;
            this.routingKey = routingKey;

            arguments["x-queue-type"] = "quorum";

            channel.ExchangeDeclare(deliveryExchange,exchangeType,durable,autoDelete,arguments);
            channel.QueueDeclare(address,durable,exclusive,autoDelete,arguments);            
        }

        public void Initialize(IModel channel, IEnumerable<string> receivingAddresses, IEnumerable<string> sendingAddresses)
        {
        }

        public void Publish(IModel channel, Type type, OutgoingMessage message, IBasicProperties properties)
        {
            Console.WriteLine($"INFO: PUBLISHING {message.Body}");
            channel.BasicPublish(exchange,routingKey,mandatory,properties, message.Body);
        }

        public void RawSendInCaseOfFailure(IModel channel, string address, ReadOnlyMemory<byte> body, IBasicProperties properties)
        {
            throw new NotImplementedException();
        }

        public void Send(IModel channel, string address, OutgoingMessage message, IBasicProperties properties)
        {
            throw new NotImplementedException();
        }

        public void SetupSubscription(IModel channel, Type type, string subscriberName)
        {
            throw new NotImplementedException();
        }

        public void TeardownSubscription(IModel channel, Type type, string subscriberName)
        {
            throw new NotImplementedException();
        }
    }
}