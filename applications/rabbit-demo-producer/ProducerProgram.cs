using System;
using System.Text;
using Imani.Solutions.Core.API.Util;
using rabbit_api.API;

namespace rabbit_demo_producer
{
    class ProducerProgram
    {
        static void Main(string[] args)
        {
            var config = new ConfigSettings();

            var exchange = config.GetProperty("EXCHANGE");
            var message = config.GetProperty("MESSAGE");
            string routingKey = config.GetProperty("ROUTING_KEY","");
              RabbitExchangeType type = Enum.Parse<RabbitExchangeType>(config.GetProperty("EXCHANGE_TYPE"));

            Rabbit subject = Rabbit.Connect();

            var msg = Encoding.UTF8.GetBytes(message);
            RabbitPublisher publisher = subject.PublishBuilder().
            SetExchange(exchange)
            .SetExchangeType(type)
            .Build();

 
            publisher.Publish(msg, routingKey);
            Console.WriteLine($"Sent {message}");

        }
    }
}
