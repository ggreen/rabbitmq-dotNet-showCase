using System;
using System.Text;
using System.Threading;
using Imani.Solutions.Core.API.Util;
using rabbit_api.API;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace rabbit_demo_consumer_parser
{
    class ConsumerProgramParser
    {
        static void Main(string[] args)
        {
          var config = new ConfigSettings();

            var exchange = config.GetProperty("EXCHANGE");
            var queue = config.GetProperty("QUEUE");
            string routingKey = config.GetProperty("ROUTING_KEY","");
            RabbitExchangeType type = Enum.Parse<RabbitExchangeType>(config.GetProperty("EXCHANGE_TYPE"));

            Rabbit subject = Rabbit.Connect();
            var consumer = subject.ConsumerBuilder()
            .SetExchange(exchange)
            .SetExchangeType(type)
            .AddQueue(queue,routingKey)
            .Build();

            consumer.RegisterReceiver(Reciever);

            while(true)
            {
                Thread.Sleep(10000);
            }

        }

        private static void Reciever(IModel channel,object message, BasicDeliverEventArgs eventArg)
        {
            try{
            var body = eventArg.Body;
                var msg = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine(msg);

                var domain = JsonSerializer.Deserialize<ExampleDomain>(msg);
                Console.WriteLine($"domain id: {domain.Id}");


                channel.BasicAck(eventArg.DeliveryTag, false);
            }
            catch(Exception e)
            {
                channel.BasicNack(eventArg.DeliveryTag,false,false);

                Console.WriteLine($"ERROR: {e.Message} TRACE: {e.StackTrace}");
                throw e;
            }
  
        }
    }
}
