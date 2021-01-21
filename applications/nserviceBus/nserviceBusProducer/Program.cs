using System;
using NServiceBus;
using System.Text;

namespace nserviceBusProducer
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var endpointConfiguration = new EndpointConfiguration("Samples.RabbitMQ.SimpleSender");
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            transport.ConnectionString("host=localhost");

            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.EnableInstallers();
            transport.UseCustomRoutingTopology(
            topologyFactory: createDurableExchangesAndQueues =>
            {
                return new MyRoutingTopology(createDurableExchangesAndQueues);
            });

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
            var message = new MyMessage() { Id = "hello"};
            await endpointInstance.Publish(message)
                .ConfigureAwait(false);

        }
    }
}
