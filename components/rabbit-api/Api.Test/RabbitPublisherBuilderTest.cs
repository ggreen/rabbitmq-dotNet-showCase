using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;

/// <summary>
/// 
/// Test case for RabbitPublisherBuilder
/// Author: Gregory Green
/// </summary>
namespace rabbit_api.API.Test
{
    [TestClass]
    public class RabbitPublisherBuilderTest
    {
        private RabbitPublisherBuilder subject;

        private Mock<IRabbitConnectionCreator> mockCreator;
        private Mock<IConnection> mockConnection;
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
        private readonly ushort expectedPreFetchLimit = 1000;
        private readonly string expectedContentType = "application/json";

        [TestInitialize]
        public void InitializeRabbitConsumerBuilderTest()
        {
            expectedTuple = new Tuple<string, string>(expectedQueue,expectedRoutingKey);

            mockProperties = new Mock<IBasicProperties>();
            this.mockCreator  = new Mock<IRabbitConnectionCreator>();
            this.mockConnection = new Mock<IConnection>();
            mockChannel = new Mock<IModel>();

            
            this.mockConnection.Setup( c => c.CreateModel()).Returns(mockChannel.Object);

        
            mockChannel.Setup(c => c.CreateBasicProperties()).Returns(mockProperties.Object);
            this.mockCreator.Setup( c => c.GetConnection()).Returns(mockConnection.Object);
            this.mockCreator.Setup( c => c.GetChannel()).Returns(mockChannel.Object);

            subject = new RabbitPublisherBuilder(mockCreator.Object,expectedPreFetchLimit);
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
        public void SetQosPreFetchLimit()
        {

            ushort expected = 1000;
            RabbitPublisherBuilder outSubject = subject.SetQosPreFetchLimit(expected);

            Assert.IsNotNull(outSubject);
            Assert.AreEqual(expected,outSubject.QosPreFetchLimit);

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
        public void SetQuorumQueue()
        {
            subject.AssignQueueTypeArgToQuorum();

            Assert.AreEqual(subject.QueueArguments["x-queue-type"],"quorum");
            
        }

         [TestMethod]
        public void UseClassicQueue()
        {
            RabbitPublisherBuilder actual   = subject.UseQueueType(RabbitQueueType.classic);
            Assert.AreEqual(subject,actual);

            Assert.AreEqual(subject.QueueArguments["x-queue-type"],"classic");

            
        }


        [TestMethod]
        public void SetQuorumQueue_Lazy()
        {
            subject.SetLazyQueue();

            subject.AssignQueueTypeArgToQuorum();

            Assert.AreEqual(subject.QueueArguments["x-queue-type"],"quorum");
            Assert.AreEqual(subject.QueueArguments["x-max-in-memory-length"],"0");
            Assert.IsFalse(subject.QueueArguments.ContainsKey("x-queue-mode"));
            
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
        public void SetLazyQueue()
        {
            Assert.IsFalse(subject.IsLazyQueues);
            // args.put("x-queue-mode", "lazy");
            RabbitPublisherBuilder actual = subject.SetLazyQueue();
            Assert.IsTrue(subject.IsLazyQueues);

            Assert.IsNotNull(actual);
            Assert.AreEqual("lazy",subject.QueueArguments["x-queue-mode"]);

            subject = subject.UseQueueType(RabbitQueueType.quorum);
            Assert.IsTrue(subject.IsLazyQueues);
            Assert.IsFalse(subject.QueueArguments.Keys.Contains("x-queue-mode"));
            Assert.AreEqual("0",subject.QueueArguments["x-max-in-memory-length"]);

        }


        [TestMethod]
        public void SetContentType()
        {
     
            RabbitPublisherBuilder actual = subject.SetContentType(expectedContentType);

            Assert.IsNotNull(actual);

            Assert.AreEqual(expectedContentType, actual.ContentType);
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


            // mockChannel.Verify(c => c.ExchangeDeclarePassive(expectedExchange));

            mockChannel.Verify(c => c.ExchangeDeclare(expectedExchange,
            expectedType.ToString(),
            expectedDurable,
            expectedAutoDelete,
            expectedArguments));

            mockChannel.Verify( c => c.BasicQos(0,expectedPreFetchLimit,false));

            mockChannel.Verify(c => c.QueueDeclare(expectedQueue, expectedDurable,
           expectedExclusive, expectedAutoDelete, expectedArguments),Times.Never);

        }
    }
}