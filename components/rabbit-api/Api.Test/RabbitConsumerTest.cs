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
        private Mock<IConnectionFactory> mockFactory;
        
        private Mock<IConnection> mockConnection;
        private Mock<IModel> mockChannel;
        

        private Mock<IRabbitConnectionCreator> mockedCreator;
        private RabbitConsumer subject;

        [TestInitialize]
        public void InitializeRabbitConsumerTest()
        {
            mockedCreator = new Mock<IRabbitConnectionCreator>();
            mockFactory = new Mock<IConnectionFactory>();
            mockConnection = new Mock<IConnection>();
            mockChannel = new Mock<IModel>();

            mockConnection.Setup(c => c.CreateModel()).Returns(mockChannel.Object);
            mockFactory.Setup( f => f.CreateConnection()).Returns(mockConnection.Object);
            mockedCreator.Setup(c => c.GetChannel()).Returns(mockChannel.Object);
            mockedCreator.Setup(c => c.GetConnection()).Returns(mockConnection.Object);
         
            subject = new RabbitConsumer(mockedCreator.Object, queue,autoAck);
        }

        [TestMethod]
        public void Dispose()
        {
            using(subject)
            {

            }
            mockedCreator.Verify(c => c.Dispose());
        }

        [TestMethod]
        public void Consume()
        {
            subject.RegisterReceiver(receiver);
            
        }

        [TestMethod]
        public void HandleShutdown()
        {
            subject.RegisterReceiver(receiver);

            subject.HandleShutdown("",null);

            mockedCreator.Verify(c => c.GetChannel());
        }

        private void receiver(IModel channel, object message, BasicDeliverEventArgs eventArg)
        {
        }
    }
}