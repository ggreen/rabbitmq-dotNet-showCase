# rabbitmq-dotNet-showCase


This is reference DotNet Core client/wrapper for connecting with [RabbitMQ](https://www.rabbitmq.com/).


Projects                                                                                                    | Notes
----------------------------------------------------------------------------------------------------------- | --------------------------
[components/rabbit-api](https://github.com/ggreen/rabbitmq-dotNet-showCase/tree/main/components/rabbit-api) | RabbitMQ client facade API
[applications/rabbit-demo-consumer](https://github.com/ggreen/rabbitmq-dotNet-showCase/tree/main/applications/rabbit-demo-consumer) | Consumer example
[applications/rabbit-demo-consumer-parser](https://github.com/ggreen/rabbitmq-dotNet-showCase/tree/main/applications/rabbit-demo-consumer-parser) | Consumer that also parsing the payload
[applications/rabbit-demo-producer](https://github.com/ggreen/rabbitmq-dotNet-showCase/tree/main/applications/rabbit-demo-producer) | Producer example
[applications/password-encryption]()

## Environments or Input Properties


The module using the [ConfigSettings](https://github.com/imani-solutions/Imani.Solutions.Core.DotNet/blob/master/API/Util/ConfigSettings.cs) object from the open source [Imani Solutions DotNet API](https://github.com/imani-solutions/Imani.Solutions.Core.DotNet).


You can set the properties using an environment variable or input argument (prefixed with --PROPERTY_NAME).


PROPERTY            | Notes    | Default
------------------  | -------- | ----------
RABBIT_HOST         | Host name | localhost
RABBIT_PORT         | Listen port | 5672
RABBIT_CONNECTION_RETRY_SECS         | Automatic reconnect time interval | 15
RABBIT_VIRTUAL_HOST | The rabbit virtual host | /
RABBIT_USERNAME | The rabbit username |
RABBIT_PASSWORD | The rabbit password |
RABBIT_CLIENT_NAME | The client provided name | 
RABBIT_WAIT_FOR_CONFIRMATION_SECS | Publish wait for connection | 30
RABBIT_PREFETCH_LIMIT | Prefetch limit (mainly for consumers) | 1000


## Sample Code

## Consumer

```c#

      Rabbit rabbit = Rabbit.Connect();
            var consumer = rabbit.ConsumerBuilder()
            .SetExchange(exchange)
            .AddQueue(queue,expectedRoutingKey)
            .Build();

            consumer.RegisterReceiver(receiver);

        }
        private void receiver(IModel channel, object sender, BasicDeliverEventArgs eventArg)
        {
            var actual = Encoding.UTF8.GetString(eventArg.Body.ToArray());

            channel.BasicAck(eventArg.DeliveryTag,false);
        }
```

Publisher Code

```C#

  var msg = Encoding.UTF8.GetBytes(expectedMsg);
  RabbitPublisher publisher = subject.PublishBuilder().
            SetExchange(exchange)
            .AddQueue(queue,expectedRoutingKey)
            .Build();
        

  string routingKey = "";
  publisher.Publish(msg, routingKey);

```


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

Many of these tips are based on an [ERG Solutions best practices video](https://www.youtube.com/watch?v=HzPOQsMWrGQ)

- Use 3 Node Rabbit MQ cluster 
- Use lazy queues for larger queues that ling longer on server (example: batch processing)
- Limit number of queues (less than 10 queues)
- Queue are single thread (est: 50K/s); For greater throughput you can use Consistent Hash or sharing plugins
- Reduce HA batch-sync-size to prevent introducing network partition dues to server pauses related to synchronize large batch sizes
- Prefer TCP keep alives over heart beats
  - tcp_keepalive_time=2 minutes 
  - tcp_keepalive_intv1=15
- Increase net_tick tine to 90-120s (default is 60s)
- XFS is the recommended file system
- export ERL_CRASH_DUMP_SECONDS=1
- export RABBITMQ_SERVER_ADDITIONAL_ERL_ARGS="+hmqd off_heap"
- Reduce Embed msg. Default is $4K in queue index. Recommended disable (0).
- Limit RSA not used
- If over 10K connections increase TCP buffer size (ex: sndbuf=8192, recbuf=8192)
