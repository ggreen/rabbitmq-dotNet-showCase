using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit_demo_producer.App
{
    public class RabbitConsumer
    {
       private EventingBasicConsumer consumer;
        private IModel channel;
        private string exchange;
        private string  queue;
       
        public void RegisterReceiver(ReceiveMessage receiver)
        {
            consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                receiver(model, ea);
            };

            channel.BasicConsume(queue: queue,
                                autoAck: false,
                                consumer: consumer);
        }

        public delegate void ReceiveMessage(object message, BasicDeliverEventArgs eventArg);

    }

}