using System;
using System.Text;
using Imani.Solutions.Core.API.Util;
using rabbit_api.API;
using System.Threading;
using RabbitMQ.Client.Exceptions;

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
            int sleepPeriodMs = config.GetPropertyInteger("SLEEP_PERIOD_MS");

            RabbitExchangeType type = Enum.Parse<RabbitExchangeType>(config.GetProperty("EXCHANGE_TYPE"));

            int repeatCount = config.GetPropertyInteger("REPEAT_COUNT", 1);

            using (Rabbit subject = Rabbit.Connect())
            {

                var msg = Encoding.UTF8.GetBytes(message);
                var builder = subject.PublishBuilder().
                SetExchange(exchange)
                .SetConfirmPublish()
                .SetExchangeType(type);

                int sentCount =0;
                Console.WriteLine($"IsConfirmPublish: {builder.IsConfirmPublish}");

                using (RabbitPublisher publisher = builder.Build())
                {

                    try
                    {
                        for (int i = 0; i < repeatCount; i++)
                        {
                           try{
                                publisher.Publish(msg, routingKey);
                                sentCount++;
                                Console.WriteLine($"Msg sent count {sentCount}");
                                Thread.Sleep(sleepPeriodMs);
                           }
                           catch(RabbitMQClientException rabbitException)
                           {
                               Console.WriteLine($" Connection closed {rabbitException}, reopening");
                               Thread.Sleep(sleepPeriodMs);
                           } 
                            
                        }
                        Console.WriteLine($"Sent {message} {sentCount} time(s)");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"EXCEPTION:{e}");
                    }



                }
            }
        }
    }
}
