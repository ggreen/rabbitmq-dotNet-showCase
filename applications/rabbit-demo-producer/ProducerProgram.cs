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
            string routingKey = config.GetProperty("ROUTING_KEY", "");
            RabbitExchangeType type = Enum.Parse<RabbitExchangeType>(config.GetProperty("EXCHANGE_TYPE"));

            int repeatCount = config.GetPropertyInteger("REPEAT_COUNT",1);

            using (Rabbit subject = Rabbit.Connect())
            {

                var msg = Encoding.UTF8.GetBytes(message);
                var builder = subject.PublishBuilder().
                SetExchange(exchange)
                .SetExchangeType(type);

                using (RabbitPublisher publisher = builder.Build())
                {

                    for (int i = 0; i < repeatCount; i++)
                    {
                        publisher.Publish(msg, routingKey);
                    }
                    
                    Console.WriteLine($"Sent {message} {repeatCount} time(s)");
                }
            }
        }
    }
}
