using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using rabbit_api.API;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/// <summary>
/// Author: Gregory Green
/// </summary>
namespace rabbit_api.Api.Test
{
    [TestClass]
    public class RabbitConsumerTest
    {
        private string queue = "queue";
        private bool autoAck = false;
        private Mock<IModel> mockedChannel = new Mock<IModel>();
        private RabbitConsumer subject;

        [TestInitialize]
        public void InitializeRabbitConsumerTest()
        {
            subject = new RabbitConsumer(mockedChannel.Object, queue,autoAck);
        }

        [TestMethod]
        public void Dispose()
        {
            using(subject)
            {

            }
            mockedChannel.Verify(c => c.Close());
        }

        [TestMethod]
        public void Consume()
        {
            subject.RegisterReceiver(receiver);
            
        }

        private void receiver(IModel channel, object message, BasicDeliverEventArgs eventArg)
        {
            throw new NotImplementedException();
        }
    }
}