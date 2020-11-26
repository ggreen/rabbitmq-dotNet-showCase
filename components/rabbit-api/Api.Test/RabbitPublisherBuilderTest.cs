using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;

namespace rabbit_api.API.Test
{
    [TestClass]
    public class RabbitPublisherBuilderTest
    {
        private RabbitPublisherBuilder subject;

        private Mock<IModel> mockChannel;
        private string expectedExchange = "myexchange";
        private string expectedQueue = "queue1";
        private RabbitExchangeType expectedType = RabbitExchangeType.fanout;
        private bool expectedDurable = true;
        private bool expectedAutoDelete = true;
        private IDictionary<string, object> expectedArguments = null;
        private bool expectedExclusive = true;
        private string expectedRoutingKey = "myKey";
        private Mock<IBasicProperties> mockProperties;
        private Tuple<string, string> expectedTuple;

        [TestInitialize]
        public void InitializeRabbitConsumerBuilderTest()
        {
            expectedTuple = new Tuple<string, string>(expectedQueue,expectedRoutingKey);

            mockProperties = new Mock<IBasicProperties>();

            mockChannel = new Mock<IModel>();
            mockChannel.Setup(c => c.CreateBasicProperties()).Returns(mockProperties.Object);

            subject = new RabbitPublisherBuilder(mockChannel.Object);
        }


        [TestMethod]
        public void SetExchange()
        {
            subject.SetExchange(expectedExchange);
            Assert.AreEqual(expectedExchange, subject.Exchange);

            expectedExchange = "test1";
            subject.SetExchange(expectedExchange);
            Assert.AreEqual(expectedExchange, subject.Exchange);
        }


        [TestMethod]
        public void AddQueue()
        {
            subject.AddQueue(expectedQueue,expectedRoutingKey);

            Assert.IsTrue(subject.Queues.Contains(expectedTuple));
        }

        [TestMethod]
        public void AddQueue_throwsArgumentWhenRoutingKeyIsNull()
        { 
            Assert.ThrowsException<ArgumentException>
            (()=> subject.AddQueue(expectedQueue,null));
        }
        
        [TestMethod]
          public void AddQueue_throwsArgumentWhenQueueIsNullOrEmpty()
        { 
            Assert.ThrowsException<ArgumentException>
            (()=> subject.AddQueue(null,null));

                        Assert.ThrowsException<ArgumentException>
            (()=> subject.AddQueue("",""));
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
        public void SetExchangeType()
        {
            RabbitExchangeType expected = RabbitExchangeType.fanout;
            var actual = subject.SetExchangeType(expected);

            Assert.IsNotNull(actual);

            Assert.AreEqual(expected, actual.ExchangeType);

        }
        [TestMethod]
        public void SetConfirmPublish()
        {
            Assert.IsFalse(subject.IsConfirmPublish);
            var actual = subject.SetConfirmPublish();

            Assert.IsNotNull(actual);

            Assert.IsTrue(actual.IsConfirmPublish);

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

            VerifyBuild();
            
        }

         [TestMethod]
        public void Build_SetConfirmPublish()
        {
            subject = subject.SetConfirmPublish();

            VerifyBuild();
            
             mockChannel.Verify( c => c.ConfirmSelect());
        }

        private void VerifyBuild()
        {
           
            subject.SetExchange(expectedExchange);

            subject.ExchangeType = expectedType;
            subject.Durable = expectedDurable;
            subject.AutoDelete = expectedAutoDelete;
            subject.QueueExclusive = expectedExclusive;

            var actual = subject.Build();
            Assert.IsNotNull(actual);

            mockChannel.Verify(c => c.ExchangeDeclare(expectedExchange,
            expectedType.ToString(),
            expectedDurable,
            expectedAutoDelete,
            expectedArguments));

            mockChannel.Verify(c => c.QueueDeclare(expectedQueue, expectedDurable,
           expectedExclusive, expectedAutoDelete, expectedArguments),Times.Never);

        }
    }
}