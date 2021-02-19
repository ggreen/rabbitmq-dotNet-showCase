using System;
using System.Collections.Generic;
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
    /// ConsumerHarness is an example fault tolerant RabbitMQ consumer
    /// Author: Gregory Green
    /// </summary>
    public class ConsumerHarness
    {
        private long count = 0;
        private long MAX_RETRY_COUNT = 10;

        private RabbitConsumerBuilder builder;

        public bool IsNackRequeued { get; set; }
        public bool IsAckMultiple { get; set; }
        private Rabbit rabbit;

        public void Run()
        {
            if (rabbit == null)
                rabbit = Rabbit.Connect();

            var config = new ConfigSettings();

            var exchange = config.GetProperty("EXCHANGE");
            var queue = config.GetProperty("QUEUE");
            string routingKey = config.GetProperty("ROUTING_KEY", "");
            RabbitExchangeType type = Enum.Parse<RabbitExchangeType>(config.GetProperty("EXCHANGE_TYPE"));


            builder = rabbit.ConsumerBuilder()
                        .SetExchange(exchange)
                        .SetExchangeType(type)
                        .AddQueue(queue, routingKey);

            var queueType = Enum.Parse<RabbitQueueType>(config.GetProperty("QUEUE_TYPE"));

            builder.UseQueueType(queueType);

            if (config.GetPropertyBoolean("LAZY_QUEUE", true))
            {
                //builder.
            }

            if (config.GetPropertyBoolean("SINGLE_ACTIVE_CONSUMER", false))
                builder = builder.SetSingleActiveConsumer();

            var consumer = builder.Build();

            consumer.RegisterReceiver(Reciever);

            while (true)
            {
                Thread.Sleep(10000);
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
                Console.WriteLine($"CONSUMER: received {messageTxt.Length} bytes count {count} contentType: {contentType} Redelivered: {eventArg.Redelivered} HEADERS: {ToText(eventArg.BasicProperties.Headers)}");

                if (contentType != null && "application/json".Equals(contentType, StringComparison.OrdinalIgnoreCase))
                {
                    JObject.Parse(messageTxt);
                }

                channel.BasicAck(eventArg.DeliveryTag, IsAckMultiple);

            }
            catch (Exception e)
            {
                Console.WriteLine($"CONSUMER: DeliveryTag:{eventArg.DeliveryTag} Input Message: {messageTxt} ERROR: {e} IsNackRequeued:{IsNackRequeued}");
                if (!IsRetryable(e))
                {
                    Console.WriteLine($"CRITICAL: Nonretryable exception {e} Dropping message");
                    channel.BasicNack(eventArg.DeliveryTag, IsAckMultiple, false);
                }
                else if (eventArg.Redelivered && (long)eventArg.BasicProperties.Headers["x-delivery-count"] >= MAX_RETRY_COUNT)
                {
                    Console.WriteLine($"WARNING: {eventArg.BasicProperties.Headers["x-delivery-count"]} >= {MAX_RETRY_COUNT} will NOT be redeliveryed");
                    channel.BasicReject(eventArg.DeliveryTag, false);
                }
                else
                {
                    channel.BasicNack(eventArg.DeliveryTag, IsAckMultiple, IsNackRequeued);
                }
            }
        }

        private bool IsRetryable(Exception e)
        {
            return true; //TODO: simulate this in the future
        }

        private string ToText(IDictionary<string, object> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
                return "";

            StringBuilder text = new StringBuilder();
            foreach (KeyValuePair<string, object> kvp in dictionary)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                text.Append(string.Format(" Key = {0}, Value = {1} ", kvp.Key, kvp.Value));
            }
            return text.ToString();
        }
    }

}
