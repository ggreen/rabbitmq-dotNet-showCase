using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;

namespace rabbit_demo_producer.App.Test
{
    [TestClass]
    public class RabbitPublisherBuilderTest
    {
        private RabbitPublisherBuilder subject;

        private Mock<IModel> mockChannel;
        private string expectedExchange ="myexchange";
        private string expectedQueue = "queue1";
        private RabbitExchangeType expectedType = RabbitExchangeType.fanout;
        private bool expectedDurable = true;
        private bool expectedAutoDelete = true;
        private IDictionary<string, object> expectedArguments = null;
        private bool expectedExclusive =true;
        private string expectedRoutingKey = "myKey";
        private Mock<IBasicProperties> mockProperties;

        [TestInitialize]
         public void InitializeRabbitConsumerBuilderTest()
         {
             mockProperties = new Mock<IBasicProperties>();

             mockChannel = new Mock<IModel>();
             mockChannel.Setup(c => c.CreateBasicProperties()).Returns(mockProperties.Object);
                          
             subject = new RabbitPublisherBuilder(mockChannel.Object);
         }


        [TestMethod]
        public void SetExchange()
        {
            subject.SetExchange(expectedExchange);
            Assert.AreEqual(expectedExchange,subject.Exchange);

            expectedExchange = "test1";
            subject.SetExchange(expectedExchange);
            Assert.AreEqual(expectedExchange,subject.Exchange);
        }


        [TestMethod]
        public void AddQueue()
        { 
            subject.AddQueue(expectedQueue);

            Assert.IsTrue(subject.Queues.Contains(expectedQueue));
        }

        [TestMethod]
        public void IsDefaultDurable()
        {
            Assert.IsTrue(subject.Durable);
        }


        [TestMethod]
        public void IsPersistent()
        {
            Assert.IsTrue(subject.Persistent);
        }


        [TestMethod]
        public void Build_Throws_QueueRequired()
        {
            subject.SetExchange(expectedExchange);

            Assert.ThrowsException<ArgumentException>(
            () => subject.Build()
            );
            
        }

        [TestMethod]
        public void Build_Throws_ExchangeRequired()
        {
            Assert.ThrowsException<ArgumentException>(
            () => subject.Build()
            );
        }

         [TestMethod]
        public void Build()
        {
            subject.SetExchange(expectedExchange);
            subject.AddQueue(expectedQueue);

            subject.ExchangeType = expectedType;
            subject.Durable = expectedDurable;
            subject.AutoDelete  =expectedAutoDelete;
            subject.QueueExclusive = expectedExclusive;
            subject.RoutingKey = expectedRoutingKey;

            var actual = subject.Build();
            Assert.IsNotNull(actual);

            mockChannel.Verify(c => c.ExchangeDeclare(expectedExchange,
            expectedType.ToString(),
            expectedDurable,
            expectedAutoDelete,
            expectedArguments));

            mockChannel.Verify( c => c.QueueDeclare(expectedQueue,expectedDurable,
            expectedExclusive,expectedAutoDelete,expectedArguments));

            mockChannel.Verify( c => c.QueueBind(expectedQueue,expectedExchange,expectedRoutingKey,expectedArguments));
            
        }
    }
}