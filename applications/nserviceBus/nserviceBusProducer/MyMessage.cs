using NServiceBus;

namespace nserviceBusProducer
{
    public class MyMessage : IEvent
    {
        public MyMessage()
        {
        }

        public string Id { get; internal set; }
    }
}