using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using rabbit_api.API;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_api.Api.Test
{
    [TestClass]
    public class RabbitConsumerTest
    {
        [TestMethod]
        public void Consume()
        {
            var mockedChannel = new Mock<IModel>();

            string queue = "queue";
            bool autoAck = false;
            var channel = mockedChannel;
            //channel.Setup(c => c.BasicConsume(queue,autoAck,It.IsAny<EventingBasicConsumer>()));


            var subject = new RabbitConsumer(channel.Object, queue,autoAck);
            subject.RegisterReceiver(receiver);

            // mockedChannel.Verify( c => c.BasicConsume(queue,autoAck,It.IsAny<EventingBasicConsumer>()));

            /*
            channel.BasicConsume(queue: queue,
                                autoAck: autoAck,
                                consumer: consumer);
                             */
            
        }

        private void receiver(IModel channel, object message, BasicDeliverEventArgs eventArg)
        {
            throw new NotImplementedException();
        }
    }
}