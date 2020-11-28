using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_api.API.Test
{

    [TestClass]
    public class RabbitTest
    {
        Rabbit subject;        
        private string topic = "t";
        private string queue = "q";
        private String expected = "{}";
        private string routingKey = "";
        private IDictionary<string, object> args = new Dictionary<string, object>();
        
        private Mock<IConnectionFactory> mockFactory = new Mock<IConnectionFactory>();
        
        private Mock<IConnection> mockConnection = new Mock<IConnection>();
        private Mock<IModel> mockChannel = new Mock<IModel>();
        private Mock<IBasicProperties> mockProperties = new Mock<IBasicProperties>();
        private ushort expectedPrefetch = 22;

        [TestInitialize]
        public void InitializeRabbitTest()
        {
            mockFactory.Setup( f => f.CreateConnection()).Returns(mockConnection.Object);
            mockConnection.Setup(c => c.CreateModel()).Returns(mockChannel.Object);
            mockChannel.Setup(c=> c.CreateBasicProperties()).Returns(mockProperties.Object);

            
            subject = new Rabbit(mockFactory.Object,expectedPrefetch);
         
        }

        [TestMethod]
        public void Constructor()
        {
            mockFactory.Verify(f => f.CreateConnection());
               Assert.AreEqual(expectedPrefetch,subject.QosPreFetchLimit);
        }

        [TestMethod]
        public void Publish()
        {

            var consumer = subject.ConsumerBuilder()
            .SetExchange(topic)
            .AddQueue(queue,routingKey)
            .Build();

            var msg = Encoding.UTF8.GetBytes(expected);
            RabbitPublisher publisher = subject.PublishBuilder().
            SetExchange(topic)
            .AddQueue(queue,routingKey)
            .Build();

            
           publisher.Publish(msg,routingKey);


        }

        [TestMethod]
        public void Dispose()
        {
            
            using(subject){

            }

            mockConnection.Verify( c => c.Close());
        }

     
    }


}