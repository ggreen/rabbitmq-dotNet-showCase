using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/// <summary>
/// Author: Gregory Green
/// </summary>
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

        [TestMethod]
        public void ParseUrisToEndPoints_IsNull_ThrowsArgumentExcpetion()
        {
            Assert.ThrowsException<ArgumentException>(() => Rabbit.ParseUrisToEndPoints(null));
        }

        [TestMethod]
        public void ParseUrisToEndPoints_EmptyString_ThrowsArgumentExcpetion()
        {
            Assert.ThrowsException<ArgumentException>(() => Rabbit.ParseUrisToEndPoints(""));
        }

        [TestMethod]
        public void ParseUrisToEndPoints_OneUri_Equals()
        {
            string uris = "amqp://guest:guest@localhost/";
            AmqpTcpEndpoint expected = new AmqpTcpEndpoint(uris);

            IList<AmqpTcpEndpoint> actual = Rabbit.ParseUrisToEndPoints(uris);
            Assert.AreEqual(expected,actual[0]);
        }

           [TestMethod]
        public void ParseUrisToEndPoints_MultipleUris()
        {
            string uris1 = "amqp://guest:guest@host1/";
            string uris2 = "amqp://guest:guest@host2/";
            string uris = uris1+","+uris2;
            AmqpTcpEndpoint expected1 = new AmqpTcpEndpoint(uris1);
            AmqpTcpEndpoint expected2 = new AmqpTcpEndpoint(uris2);

            IList<AmqpTcpEndpoint> actual = Rabbit.ParseUrisToEndPoints(uris);
            Assert.AreEqual(expected1,actual[0]);
            Assert.AreEqual(expected2,actual[1]);
        }

        [TestMethod]
        public void ParseUrisToEndPoints()
        {
            Assert.IsNull(Rabbit.ParseUrisToEndPoints(null));
        }
    }


}