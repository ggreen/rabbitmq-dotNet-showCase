using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using rabbit_api.API;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/// <summary>
/// <author>
/// </summary>
namespace rabbit_api.Api.Test
{
    [TestClass]
    public class RabbitPublisherTest
    {
        private string exchange = "exchange";
        private Mock<IModel> mockedChannel = new Mock<IModel>();
        private RabbitPublisher subject;
        private Mock<IBasicProperties> basicProperties = new Mock<IBasicProperties>();
        private readonly bool confirmPublish = true;
        //private readonly string contentType = "application/json";

        [TestInitialize]
        public void InitializeRabbitPublisherTest()
        {

            subject = new RabbitPublisher(mockedChannel.Object, exchange, basicProperties.Object,confirmPublish);
        }

        [TestMethod]
        public void Dispose()
        {
            using (subject)
            {

            }
            mockedChannel.Verify(c => c.Close());
        }

        [TestMethod]
        public void Publish_ThrowsArgumentWhenBodyNull()
        {
            byte[] body = null;
            string routingKey = null;
            Assert.ThrowsException<ArgumentException>(() =>
            subject.Publish(body, routingKey));

        }


          

        [TestMethod]
        public void Publish_RoutingKeyCannotBeNull()
        {
            byte[] body = Encoding.UTF8.GetBytes("hello");
            string routingKey = null;
            Assert.ThrowsException<ArgumentException>(() =>
            subject.Publish(body, routingKey));

        }
        [TestMethod]
        public void Publish_DoesWait_WhenNotConfirm()
        {
            this.subject = new RabbitPublisher(mockedChannel.Object, exchange, basicProperties.Object,false);

            byte[] body = Encoding.UTF8.GetBytes("hello");
            string routingKey = "";
            subject.Publish(body, routingKey);

            mockedChannel.Verify(c => c.WaitForConfirms(It.IsAny<TimeSpan>()),Times.Never);
        }

        [TestMethod]
        public void Publish_ConfirmPublish()
        {
            byte[] body = Encoding.UTF8.GetBytes("hello");
            string routingKey = "";


            subject.Publish(body, routingKey);

            mockedChannel.Verify(c => c.WaitForConfirmsOrDie(It.IsAny<TimeSpan>()));
        }
    }
}