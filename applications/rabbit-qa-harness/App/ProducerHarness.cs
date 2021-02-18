using System;
using System.Text;
using Imani.Solutions.Core.API.Util;
using rabbit_api.API;
using System.Threading;
using RabbitMQ.Client.Exceptions;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace rabbit_qa_harness.App
{
    /// <summary>
    /// ProducerHarness is an example fault tolerant RabbitMQ producer
    /// author: Gregory Green
    /// </summary>
    class ProducerHarness
    {
        private const int JSON_MIN_SIZE = 10;
        private readonly ConfigSettings config = new ConfigSettings();
        private readonly string exchange;
        private readonly string message;
        private readonly string routingKey;
        private readonly int sleepPeriodMs;
        private readonly RabbitExchangeType type;
        private readonly int repeatCount;
        private string contentType;
        private Rabbit rabbit;

        private readonly byte[] msg;
        private readonly int producerCount;

        private RabbitPublisher publisher;

        private readonly string DEFAULT_CONTENT_TYPE = "text/plain";
        private static readonly object JSON_CONTENT_TYPE = "application/json";
        private readonly int errorSleepPeriodMs;

        internal static string GetMessage(ISettings config, string contentType)
        {

            string message = config.GetProperty("MESSAGE", "");
            int messageSize = config.GetPropertyInteger("MESSAGE_SIZE", 0);

            if (JSON_CONTENT_TYPE.Equals(contentType))
            {
                if (messageSize > 0)
                {
                    int bodySize = messageSize - JSON_MIN_SIZE;
                    string body = new Text().GenerateText(bodySize, "test");
                    message = new StringBuilder()
                    .Append("{\"msg\":\"")
                    .Append(body).Append("\"}").ToString();


                    return message;
                }
            }

            if (String.IsNullOrEmpty(message))
            {
                if (messageSize > 0)
                {
                    message = new Text().GenerateText(messageSize, "test");
                }
                else
                {
                    throw new ArgumentException("Set configuration property MESSAGE or MESSAGE_SIZE>0");
                }
            }
            else
            {
                if (messageSize > 0)
                    message = new Text().GenerateText(messageSize, message);

            }
            return message;
        }


        public ProducerHarness()
        {

            exchange = config.GetProperty("EXCHANGE");
            routingKey = config.GetProperty("ROUTING_KEY", "");
            sleepPeriodMs = config.GetPropertyInteger("SLEEP_PERIOD_MS");
            errorSleepPeriodMs = config.GetPropertyInteger("ERROR_SLEEP_PERIOD_MS", 15000);
            type = Enum.Parse<RabbitExchangeType>(config.GetProperty("EXCHANGE_TYPE"));
            repeatCount = config.GetPropertyInteger("REPEAT_COUNT", 1);
            producerCount = config.GetPropertyInteger("PRODUCER_COUNT", 1);
            contentType = config.GetProperty("CONTENT_TYPE", DEFAULT_CONTENT_TYPE);


            message = GetMessage(config, contentType);
            msg = Encoding.UTF8.GetBytes(message);

        }

        public void Run()
        {
            Connect();
            long sentCount = 0;

            try
            {
                for (int i = 0; i < repeatCount; i++)
                {
                    try
                    {
                        publisher.Publish(msg, routingKey);
                        sentCount++;
                        Console.WriteLine($"PRODUCER: Msg sent {message.Length} byte count {sentCount}");
                        Thread.Sleep(sleepPeriodMs);
                    }
                    catch (RabbitMQClientException rabbitException)
                    {
                        Console.WriteLine($"PRODUCER:  client exce[topm] {rabbitException}, sentCount:{sentCount} sleeping {errorSleepPeriodMs} milliseconds");
                        Thread.Sleep(errorSleepPeriodMs);
                        Console.WriteLine($"PRODUCER: WOKE after client exception {rabbitException.Message} sentCount:{sentCount}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"PRODUCER: EXCEPTION:{e} sentCount:{sentCount} sleeping {errorSleepPeriodMs} milliseconds");
                        Thread.Sleep(errorSleepPeriodMs);
                        Console.WriteLine($"PRODUCER: WOKE after RabbitMQ exception {e.Message} sentCount:{sentCount}");
                    }

                }
                Console.WriteLine($"PRODUCER: Sent {message.Length} bytes {sentCount} time(s)");
            }
            catch (Exception e)
            {
                Console.WriteLine($"PRODUCER: EXCEPTION:{e}");
            }
        }

        private void Connect()
        {
            rabbit = Rabbit.Connect();

            var builder = rabbit.PublishBuilder().
                            SetExchange(exchange)
                            .SetConfirmPublish()
                            .SetExchangeType(type)
                            .SetContentType(contentType);

            var queue = config.GetProperty("QUEUE", "");
            if (queue.Length > 0)
            {
                builder.UseQueueType(Enum.Parse<RabbitQueueType>(config.GetProperty("QUEUE_TYPE")));
                builder.AddQueue(queue, config.GetProperty("ROUTING_KEY", ""));
            }

            List<Thread> list = new List<Thread>(this.producerCount);


            publisher = builder.Build();

        }
    }
}
