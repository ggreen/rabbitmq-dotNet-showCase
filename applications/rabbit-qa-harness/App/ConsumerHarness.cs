using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using Imani.Solutions.Core.API.Util;
using Newtonsoft.Json.Linq;
using rabbit_api.API;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_qa_harness.App
{
    /// <summary>
    /// Author: Gregory Green
    /// </summary>
    public class ConsumerHarness
    {
        private long count =0;

        public bool IsNackRequeued { get;  set; }
        public bool IsAckMultiple { get;  set; }

        public void Run()
        {
            var config = new ConfigSettings();

            var exchange = config.GetProperty("EXCHANGE");
            var queue = config.GetProperty("QUEUE");
            string routingKey = config.GetProperty("ROUTING_KEY", "");
            RabbitExchangeType type = Enum.Parse<RabbitExchangeType>(config.GetProperty("EXCHANGE_TYPE"));

            using (Rabbit subject = Rabbit.Connect())
            {
                var builder = subject.ConsumerBuilder()
                            .SetExchange(exchange)
                            .SetExchangeType(type)
                            .AddQueue(queue, routingKey);
                
                if(config.GetPropertyBoolean("QUORUM_QUEUES", true))
                {
                    builder.UseQuorumQueues();
                }

                if(config.GetPropertyBoolean("LAZY_QUEUE", true))
                {
                    //builder.
                }

                if (config.GetPropertyBoolean("SINGLE_ACTIVE_CONSUMER",false))
                    builder = builder.SetSingleActiveConsumer();

                using (var consumer = builder.Build())
                {

                    consumer.RegisterReceiver(Reciever);

                    while (true)
                    {
                        Thread.Sleep(10000);
                    }
                }
            }
        }

        private void Reciever(IModel channel, object messageObj, BasicDeliverEventArgs eventArg)
        {
            string messageTxt = null;
            try
            {
                var body = eventArg.Body;
                count++;
                String contentType = eventArg.BasicProperties.ContentType;

                messageTxt = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine($"CONSUMER: received {messageTxt.Length} bytes count {count}");

                if (contentType != null && "application/json".Equals(contentType,StringComparison.OrdinalIgnoreCase))
                {
                    JObject.Parse(messageTxt);
                }

                channel.BasicAck(eventArg.DeliveryTag, IsAckMultiple);

            }
            catch (Exception e)
            {
                Console.WriteLine($"CONSUMER: DeliveryTag:{eventArg.DeliveryTag} Input Message: {messageTxt} ERROR: {e} IsNackRequeued:{IsNackRequeued}");
                channel.BasicNack(eventArg.DeliveryTag,IsAckMultiple,IsNackRequeued);
            }
        }
    }
}
