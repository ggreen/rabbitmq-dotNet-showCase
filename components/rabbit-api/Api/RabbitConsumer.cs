using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_api.API
{
    public class RabbitConsumer : IDisposable
    {
        private readonly IModel channel;
        private readonly string queue;
        private bool autoAck = false;

        public RabbitConsumer(IModel channel, string queue,bool autoAck)
        {
            this.channel = channel;
            this.queue = queue;
            this.autoAck = autoAck;
        }

        public void Dispose()
        {
            this.channel.Close();
        }

        public void RegisterReceiver(ReceiveMessage receiver)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, ea) =>
            {
                receiver(this.channel,sender, ea);
            };

            channel.BasicConsume(queue: queue,
                                autoAck: autoAck,
                                consumer: consumer);
        }

        public delegate void ReceiveMessage(IModel channel, object message, BasicDeliverEventArgs eventArg);

    }

}