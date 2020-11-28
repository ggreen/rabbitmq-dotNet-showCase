# rabbitmq-dotNet-showCase


This is reference DotNet Core client/wrapper for connecting with [RabbitMQ](https://www.rabbitmq.com/).

## Environments or Input Properties


The module using the [ConfigSettings](https://github.com/imani-solutions/Imani.Solutions.Core.DotNet/blob/master/API/Util/ConfigSettings.cs) object from the open source [Imani Solutions DotNet API](https://github.com/imani-solutions/Imani.Solutions.Core.DotNet).


You can set the properties using an environment variable or input argument (prefixed with --PROPERTY_NAME).


PROPERTY            | Notes    | Default
------------------  | -------- | ----------
RABBIT_HOST         | Host name | localhost
RABBIT_PORT         | Listen port | 5672
RABBIT_CONNECTION_RETRY_SECS         | Automatic reconnect time interval | 15
RABBIT_VIRTUAL_HOST | The rabbit virtual host | /'
RABBIT_USERNAME | The rabbit username |
RABBIT_PASSWORD | The rabbit password |
RABBIT_CLIENT_NAME | The client provided name | 
RABBIT_WAIT_FOR_CONFIRMATION_SECS | Publish wait for connection | 30
RABBIT_PREFETCH_LIMIT | Prefetch limit (mainly for consumers) | 1000




# Best Practices

## Client Side
- Use Quorum queues for consistency
- Enable auto reconnect for connections
- Use one connection per process
- Use one channel per thread
- Enable handle to detected blocked connections'
- Use Durable exchanges, durable queues and persistent messages for reliablity
- Set queue message limites (ex: 1 to 10 million messages)
- Set auto delete on temporary queues
- Set client name to assist with identifying application connections



### Client Publisher Side

- Use Publisher confirms for consistency
- Added handler for BasicReturn when messages not routed to a queue


### Client Consumer Side
- Use manual ACK for consumers per message
- Set Prefetch limit for consumers 
  - If you use dead letter exchanges/queues set to a very high count since DLX do not support  confirmation and can lost messages
  - Use consumer (push) over get (pull)
  - Set the prefetch limit (round trip latency/processing time) (ex: MAX 1K)

## Server Side
- Use 3 Node Rabbit MQ cluster 
- Use lazy queues for larger queues that ling longer on server (example: batch processing)
- Limit number of queues (less than 10 queues)
- Queue are single thread (est: 50K/s); For greater throughput you can use Consistent Hash or sharing plugins
- Reduce HA batch-sync-size to prevent introducing network partition dues to server pauses related to synchronize large batch sizes
- Prefer TCP keep alives over heart beats
- Increase net_tick tine to 90-120s (default is 60s)
