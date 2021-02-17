using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/// <summary>
/// Author: Gregory Green
/// </summary>
namespace rabbit_api.API
{
    public class RabbitConsumer : IDisposable
    {
        private readonly IList<ReceiveMessage> recievers = new List<ReceiveMessage>();
        private readonly IRabbitConnectionCreator creator;
        private readonly string queue;
        private bool autoAck = false;

        public RabbitConsumer(IRabbitConnectionCreator creator, string queue,bool autoAck)
        {
            this.creator = creator;
            this.queue = queue;
            this.autoAck = autoAck;

            creator.GetConnection().ConnectionShutdown += HandleShutdown;
        }

        public void Dispose()
        {
            this.creator.Dispose();
        }

        public void RegisterReceiver(ReceiveMessage receiver)
        {

            RegisterWithRabbit(receiver);
            this.recievers.Add(receiver);
        }

         private void RegisterWithRabbit(ReceiveMessage receiver)
        {
            var consumer = new EventingBasicConsumer(creator.GetChannel());

            consumer.Received += (sender, ea) =>
            {
                receiver(creator.GetChannel(),sender, ea);
            };

            creator.GetChannel().BasicConsume(queue: queue,
                                autoAck: autoAck,
                                consumer: consumer);
        }


        internal void HandleShutdown(object sender, ShutdownEventArgs e)
        {
            if(recievers.Count == 0)
                return;

            Console.WriteLine("WARNING: Processing shutdown. Reconnecting");

            try
            {
                foreach(var reciever in recievers)
                {
                    RegisterWithRabbit(reciever);
                }

                Console.WriteLine("INFO: Completed Reconnecting");
            }
            catch(Exception exception)
            {
                Console.WriteLine($"ERROR: {exception}");
            }

        }

        public delegate void ReceiveMessage(IModel channel, object message, BasicDeliverEventArgs eventArg);

    }

}