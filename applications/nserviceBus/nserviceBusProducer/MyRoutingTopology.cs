using System;
using System.Collections.Generic;
using NServiceBus.Transport;
using NServiceBus.Transport.RabbitMQ;
using RabbitMQ.Client;

namespace nserviceBusProducer
{
    internal class MyRoutingTopology : IRoutingTopology
    {
        public void BindToDelayInfrastructure(IModel channel, string address, string deliveryExchange, string routingKey)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IModel channel, IEnumerable<string> receivingAddresses, IEnumerable<string> sendingAddresses)
        {
            throw new NotImplementedException();
        }

        public void Publish(IModel channel, Type type, OutgoingMessage message, IBasicProperties properties)
        {
            throw new NotImplementedException();
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