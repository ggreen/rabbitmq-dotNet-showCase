using System;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/// <summary>
/// Author: Gregory Green
/// </summary>
namespace rabbit_api.API.Test
{
    //[TestClass]
    public class RabbitIntTest
    {
        private string exchange = "myexchange";
        private string queue = "myqueue";
        private string actual = null;
        private string expectedMsg = "{\"id\": 1}";
        private int sleepTimeMs = 1000;
        private string expectedRoutingKey = "";

        //[TestMethod]
        public void IntTest()
        {
            Environment.SetEnvironmentVariable("RABBIT_PORT","5671");

            Rabbit subject = Rabbit.Connect();

            var consumer = subject.ConsumerBuilder()
            .SetExchange(exchange)
            .AddQueue(queue,expectedRoutingKey)
            .Build();

            consumer.RegisterReceiver(receiver);

            var msg = Encoding.UTF8.GetBytes(expectedMsg);
            RabbitPublisher publisher = subject.PublishBuilder().
            SetExchange(exchange)
            .AddQueue(queue,expectedRoutingKey)
            .Build();

        

            string routingKey = "";
            publisher.Publish(msg, routingKey);

            Thread.Sleep(sleepTimeMs);

            Assert.AreEqual(expectedMsg, actual);
        }
        private void receiver(IModel channel, object sender, BasicDeliverEventArgs eventArg)
        {
            actual = Encoding.UTF8.GetString(eventArg.Body.ToArray());

            channel.BasicAck(eventArg.DeliveryTag,false);
        }
    }

}