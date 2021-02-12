using System;
using RabbitMQ.Client;

namespace rabbit_api.API
{
    public interface IRabbitConnectionCreator : IDisposable
    {
        IConnection GetConnection();
        IModel GetChannel();
    }
}