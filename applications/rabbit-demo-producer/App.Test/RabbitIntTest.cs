using System;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client.Events;

namespace rabbit_demo_producer.App.Test
{
    [TestClass]
    public class RabbitIntTest
    {
        private string exchange = "myexchange";
        private string queue = "myqueue";
        private string actual = null;
        private string expectedMsg = "{\"id\": 1}";
        private int sleepTimeMs = 1000;
        [TestMethod]
        public void IntTest()
        {
            Environment.SetEnvironmentVariable("RABBIT_PORT","5671");

            Rabbit subject = Rabbit.Connect();

            var consumer = subject.ConsumerBuilder()
            .SetExchange(exchange)
            .AddQueue(queue)
            .Build();

            consumer.RegisterReceiver(reciever);

            var msg = Encoding.UTF8.GetBytes(expectedMsg);
            RabbitPublisher publisher = subject.PublishBuilder().
            SetExchange(exchange)
            .AddQueue(queue)
            .Build();

        

            string routingKey = "";
            publisher.Publish(msg, routingKey);

            Thread.Sleep(sleepTimeMs);

            Assert.AreEqual(expectedMsg, actual);
        }
        private void reciever(object message, BasicDeliverEventArgs eventArg)
        {
            actual = Encoding.UTF8.GetString(eventArg.Body.ToArray());
        }
    }

}