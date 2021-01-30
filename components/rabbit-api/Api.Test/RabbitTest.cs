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

        [TestMethod]
        public void ToAmqpTcpEndpoints()
        {
            Assert.IsNull(Rabbit.ToAmqpTcpEndpoints(null));

            IList<Uri> list = new List<Uri>();
            string uri1Text = "amqps://guest:guest@localhost:5671/";
            Uri uri1 = new Uri(uri1Text);
            list.Add(uri1);

            IList<AmqpTcpEndpoint> expected = Rabbit.ToAmqpTcpEndpoints(list);
            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
           AmqpTcpEndpoint amqpTcpEndpoint1 = expected[0];
           Assert.AreEqual("localhost",amqpTcpEndpoint1.HostName);
           Assert.AreEqual(5671,amqpTcpEndpoint1.Port);
           Assert.IsTrue(amqpTcpEndpoint1.Protocol.ApiName.Contains("AMQP"));
           Assert.IsTrue(amqpTcpEndpoint1.Ssl.Enabled);
           
        }

        [TestMethod]
        public void CreateSslOption_bool()
        {
            Assert.IsTrue(Rabbit.CreateSslOption(true).Enabled);
            Assert.IsFalse(Rabbit.CreateSslOption(false).Enabled);
        }

        [TestMethod]
        public void CreateSslOption_Uri()
        {
            
            Assert.IsTrue(Rabbit.CreateSslOption(new Uri("amqps://guest:guest@localhost:5671/")).Enabled);
            Assert.IsFalse(Rabbit.CreateSslOption(new Uri("amqp://guest:guest@localhost:5671/")).Enabled);
        }


        [TestInitialize]
        public void InitializeRabbitTest()
        {
            mockFactory.Setup( f => f.CreateConnection()).Returns(mockConnection.Object);
            mockConnection.Setup(c => c.CreateModel()).Returns(mockChannel.Object);
            mockChannel.Setup(c=> c.CreateBasicProperties()).Returns(mockProperties.Object);

            
            subject = new Rabbit(mockFactory.Object,null,false,expectedPrefetch);
         
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
            Uri expected = new Uri(uris);

            IList<Uri> actual = Rabbit.ParseUrisToEndPoints(uris);
            Assert.AreEqual(expected,actual[0]);
        }

           [TestMethod]
        public void ParseUrisToEndPoints_MultipleUris()
        {
            string uris1 = "amqp://guest:guest@host1/";
            string uris2 = "amqp://guest:guest@host2/";
            string uris = uris1+","+uris2;
            Uri expected1 = new Uri(uris1);
            Uri expected2 = new Uri(uris2);

            IList<Uri> actual = Rabbit.ParseUrisToEndPoints(uris);
            Assert.AreEqual("host1",actual[0].Host);
            Assert.AreEqual("host2",actual[1].Host);
            
        }

        [TestMethod]
        public void ParseUrisToEndPoints()
        {
            Assert.ThrowsException<ArgumentException>(() =>Rabbit.ParseUrisToEndPoints(null));
        }
    }


}