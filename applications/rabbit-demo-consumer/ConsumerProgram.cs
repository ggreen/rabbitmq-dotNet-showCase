using System;
using System.Text;
using System.Threading;
using Imani.Solutions.Core.API.Util;
using rabbit_api.API;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_demo_consumer
{
    class ConsumerProgram
    {
        static void Main(string[] args)
        {
          var config = new ConfigSettings();

            var exchange = config.GetProperty("EXCHANGE");
            var queue = config.GetProperty("QUEUE");
            string routingKey = config.GetProperty("ROUTING_KEY","");
            RabbitExchangeType type = Enum.Parse<RabbitExchangeType>(config.GetProperty("EXCHANGE_TYPE"));

            Rabbit subject = Rabbit.Connect();
            var builder = subject.ConsumerBuilder()
            .SetExchange(exchange)
            .SetExchangeType(type)
            .UseQuorumQueues()
            .AddQueue(queue,routingKey);


            if("True".Equals(config.GetProperty("SINGLE_ACTIVE_CONSUMER","False")))
                builder = builder.SetSingleActiveConsumer();

            var consumer = builder.Build();

            consumer.RegisterReceiver(Reciever);

            while(true)
            {
                Thread.Sleep(10000);
            }

        }

        private static void Reciever(IModel channel,object message, BasicDeliverEventArgs eventArg)
        {
            var body = eventArg.Body;
            var msg = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine(msg);

             channel.BasicAck(eventArg.DeliveryTag, false);
        }
    }
}
