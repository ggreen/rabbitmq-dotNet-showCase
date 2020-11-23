using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;

namespace rabbit_demo_producer.App.Test
{
    [TestClass]
    public class RabbitConsumerBuilderTest
    {
        private Mock<IModel> mockChannel;
         private RabbitConsumerBuilder subject;
        private string expectedExchange ="myexchange";
        private string expectedQueue = "queue1";

        private RabbitExchangeType expectedType = RabbitExchangeType.fanout;
        private bool expectedDurable = true;
        private bool expectedAutoDelete = true;
        private IDictionary<string, object> expectedArguments = null;
        private bool expectedExclusive =true;
        private string expectedRoutingKey = "myKey";

        [TestInitialize]
         public void InitializeRabbitConsumerBuilderTest()
         {
             mockChannel = new Mock<IModel>();
             subject = new RabbitConsumerBuilder(mockChannel.Object);
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