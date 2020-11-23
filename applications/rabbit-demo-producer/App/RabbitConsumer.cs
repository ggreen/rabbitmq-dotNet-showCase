using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_demo_producer.App
{
    public class RabbitConsumer
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

       
        public void RegisterReceiver(ReceiveMessage receiver)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                receiver(model, ea);
            };

            channel.BasicConsume(queue: queue,
                                autoAck: autoAck,
                                consumer: consumer);
        }

        public delegate void ReceiveMessage(object message, BasicDeliverEventArgs eventArg);

    }

}