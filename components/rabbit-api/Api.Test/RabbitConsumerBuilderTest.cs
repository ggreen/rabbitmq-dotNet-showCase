using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;

namespace rabbit_api.API
{
    [TestClass]
    public class RabbitConsumerBuilderTest
    {
        private Mock<IModel> mockChannel;
        private RabbitConsumerBuilder subject;
        private string expectedExchange = "myexchange";
        private string expectedQueue = "queue1";

        private RabbitExchangeType expectedType = RabbitExchangeType.fanout;
        private bool expectedDurable = true;
        private bool expectedAutoDelete = true;
        private IDictionary<string, object> expectedQueueArguments = new Dictionary<string,object>();
        private bool expectedExclusive = true;
        private string expectedRoutingKey = "myKey";
        private Tuple<string, string> expectedTuple;
        private IDictionary<string, object> expectedExchangeArguments = null;
        private readonly string expectedSingleActiveConsumerProp = "x-single-active-consumer";
        private readonly string expectedQuorumQueueProp = "x-queue-type";
        private readonly string expectedQuorumQueueValue = "quorum";
        private readonly ushort expectedPreFetchLimit = 100;

        [TestInitialize]
        public void InitializeRabbitConsumerBuilderTest()
        {
            mockChannel = new Mock<IModel>();
            subject = new RabbitConsumerBuilder(mockChannel.Object,expectedPreFetchLimit);
            expectedTuple = new Tuple<string, string>(expectedQueue, expectedRoutingKey);
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
        public void SetExchangeType()
        {
            RabbitExchangeType expected = RabbitExchangeType.fanout;
            var actual = subject.SetExchangeType(expected);

            Assert.IsNotNull(actual);

            Assert.AreEqual(expected, actual.ExchangeType);

        }

        [TestMethod]
        public void AddQueue()
        {
            subject.AddQueue(expectedQueue, expectedRoutingKey);

            Assert.IsTrue(subject.Queues.Contains(expectedTuple));
        }

        [TestMethod]
        public void AddQueue_throwsArgumentWhenRoutingKeyIsNull()
        {
            Assert.ThrowsException<ArgumentException>
            (() => subject.AddQueue(expectedQueue, null));
        }


        [TestMethod]
        public void AddQueue_throwsArgumentWhenQueueIsNullOrEmpty()
        {
            Assert.ThrowsException<ArgumentException>
            (() => subject.AddQueue(null, ""));
            Assert.ThrowsException<ArgumentException>
  (() => subject.AddQueue("", ""));
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
        public void UseQuorumQueues()
        {
              RabbitConsumerBuilder actual = subject.UseQuorumQueues();

            Assert.IsNotNull(actual);

            IDictionary<string,object> args = subject.QueueArguments;

            Assert.IsNotNull(args);

            Assert.AreEqual(expectedQuorumQueueValue, args[expectedQuorumQueueProp]);
        }

        [TestMethod]
        public void SetSingleActiveConsumer()
        {
            var actual = subject.SetSingleActiveConsumer();

            Assert.IsNotNull(actual);

            IDictionary<string,object> args = subject.QueueArguments;

            Assert.IsNotNull(args);

            Assert.AreEqual(true, args["x-single-active-consumer"]);

        }


        [TestMethod]
        public void IsDefaultDurable()
        {
            Assert.IsTrue(subject.Durable);
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
        public void Build_WithSingleActiveConsumer()
        {
            subject.SetSingleActiveConsumer();
            expectedQueueArguments = new Dictionary<string,object>();
            expectedQueueArguments[expectedSingleActiveConsumerProp] = true;


            VerifyBuild();
        }


         [TestMethod]
        public void Build_WithQuorumQueues()
        {
            subject.UseQuorumQueues();
            expectedQueueArguments = new Dictionary<string,object>();
            expectedQueueArguments["x-queue-type"] = "quorum";

            VerifyBuild();
        }

        [TestMethod]
        public void SetQosPreFetchLimit()
        {
            ushort expected = 1000;
            RabbitConsumerBuilder outSubject = subject.SetQosPreFetchLimit(expected);

            Assert.IsNotNull(outSubject);
            Assert.AreEqual(expected,outSubject.QosPreFetchLimit);
        }

        private void VerifyBuild()
        {
            subject.SetExchange(expectedExchange);
            subject.AddQueue(expectedTuple.Item1, expectedTuple.Item2);

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
            expectedExchangeArguments));

            mockChannel.Verify( c => c.BasicQos(0,expectedPreFetchLimit,false));

            mockChannel.Verify(c => c.QueueDeclare(expectedQueue, expectedDurable,
           expectedExclusive, expectedAutoDelete, expectedQueueArguments));


            mockChannel.Verify(c => c.QueueBind(expectedQueue, expectedExchange, expectedRoutingKey, null));

        }
    }
}