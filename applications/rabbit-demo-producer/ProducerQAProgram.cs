using System;
using System.Text;
using Imani.Solutions.Core.API.Util;
using rabbit_api.API;
using System.Threading;
using RabbitMQ.Client.Exceptions;
using System.Collections.Generic;

namespace rabbit_demo_producer
{
    class ProducerQaProgram
    {
        private readonly ConfigSettings config = new ConfigSettings();
        private readonly string exchange;
        private readonly string message;
        private readonly string routingKey;
        private readonly int sleepPeriodMs;
        private readonly RabbitExchangeType type;
        private readonly int repeatCount;

        private readonly byte[] msg;
        private readonly int producerCount;

        public ProducerQaProgram()
        {

            exchange = config.GetProperty("EXCHANGE");
            routingKey = config.GetProperty("ROUTING_KEY", "");
            sleepPeriodMs = config.GetPropertyInteger("SLEEP_PERIOD_MS");
            type = Enum.Parse<RabbitExchangeType>(config.GetProperty("EXCHANGE_TYPE"));
            repeatCount = config.GetPropertyInteger("REPEAT_COUNT", 1);
            producerCount = config.GetPropertyInteger("PRODUCER_COUNT", 1);
            message = config.GetProperty("MESSAGE");
            msg = Encoding.UTF8.GetBytes(message);

        }


        static void Main(string[] args)
        {
            var program = new ProducerQaProgram();
            program.Run();
        }
        private void Run()
        {
            List<Thread> list = new List<Thread>();

            for (int i = 0; i < producerCount; i++)
            {
                Thread thread = new Thread(StartProducing);
                list.Add(thread);
                thread.Start();
            }

            foreach (Thread thread in list)
            {
                thread.Join();
            }

        }

        public void StartProducing()
        {
            using (Rabbit rabbit = Rabbit.Connect())
            {
                var builder = rabbit.PublishBuilder().
                                SetExchange(exchange)
                                .SetConfirmPublish()
                                .SetExchangeType(type);

                List<Thread> list = new List<Thread>(this.producerCount);

                int sentCount = 0;

                using (RabbitPublisher publisher = builder.Build())
                {
                    try
                    {
                        for (int i = 0; i < repeatCount; i++)
                        {
                            try
                            {
                                publisher.Publish(msg, routingKey);
                                sentCount++;
                                Console.WriteLine($"Msg sent count {sentCount}");
                                Thread.Sleep(sleepPeriodMs);
                            }
                            catch (RabbitMQClientException rabbitException)
                            {
                                Console.WriteLine($" Connection closed {rabbitException}, reopening");
                                Thread.Sleep(sleepPeriodMs);
                            }

                        }
                        Console.WriteLine($"Sent {message} {sentCount} time(s)");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"EXCEPTION:{e}");
                    }
                }
            }

        }
    }
}
