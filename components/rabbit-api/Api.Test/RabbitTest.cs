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
        private string topic = "t";
        private string queue = "q";
        private String expected = "{}";

        [TestMethod]
        public void Publish()
        {
            string routingKey = "";
            IDictionary<string, object> args = new Dictionary<string, object>();
            var mockFactory = new Mock<IConnectionFactory>();
            var mockConnection = new Mock<IConnection>();
            var mockChannel = new Mock<IModel>();
            var mockProperties = new Mock<IBasicProperties>();

            mockFactory.Setup( f => f.CreateConnection()).Returns(mockConnection.Object);
            mockConnection.Setup(c => c.CreateModel()).Returns(mockChannel.Object);
            mockChannel.Setup(c=> c.CreateBasicProperties()).Returns(mockProperties.Object);
            
            Rabbit subject = new Rabbit(mockFactory.Object);

            var consumer = subject.ConsumerBuilder()
            .SetExchange(topic)
            .AddQueue(queue,routingKey)
            .Build();

            // consumer.RegisterReceiver(reciever);

            var msg = Encoding.UTF8.GetBytes(expected);
            RabbitPublisher publisher = subject.PublishBuilder().
            SetExchange(topic)
            .AddQueue(queue,routingKey)
            .Build();

            
           publisher.Publish(msg,routingKey);


        }

     
    }


}