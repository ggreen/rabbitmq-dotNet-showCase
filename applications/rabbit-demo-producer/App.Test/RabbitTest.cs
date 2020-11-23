using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rabbit_demo_producer.App;
using RabbitMQ.Client.Events;

namespace rabbit_demo_producer.App.Test
{

    [TestClass]
    public class RabbitTest
    {
        private string topic = "t";
        private string queue = "q";
        private string actual = null;
        private String expected = "{}";
        private int sleepTimeMs = 1000;

        [TestMethod]
        public void Publish()
        {

            IDictionary<string, object> args = new Dictionary<string, object>();
            Rabbit subject = Rabbit.Connect();

            var consumer = subject.ConsumerBuilder()
            .SetExchange(topic)
            .AddQueue(queue)
            .Build();
            
            
            consumer.RegisterReceiver(reciever);


            var msg = Encoding.UTF8.GetBytes(expected);
            RabbitPublisher publisher = subject.PublishBuilder().
            SetExchange(topic)
            .AddQueue(queue)
            .Build();

            publisher.Publish(msg);

            Thread.Sleep(sleepTimeMs);

            Assert.AreEqual(expected, actual);
        }

        private void reciever(object message, BasicDeliverEventArgs eventArg)
        {
            actual = Encoding.UTF8.GetString(eventArg.Body.ToArray());
        }
    }


}