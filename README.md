# rabbitmq-dotNet-showCase

This is a reference DotNet Core client/wrapper for connecting with [RabbitMQ](https://www.rabbitmq.com/). It demonstrates the best practice development techniques.

This project implements the [builder design pattern](https://en.wikipedia.org/wiki/Builder_pattern) for producers and consumers.



Projects                                                                                                    | Notes
----------------------------------------------------------------------------------------------------------- | --------------------------
[components/rabbit-api](https://github.com/ggreen/rabbitmq-dotNet-showCase/tree/main/components/rabbit-api) | RabbitMQ client facade API
[applications/rabbit-qa-harness](https://github.com/ggreen/rabbitmq-dotNet-showCase/tree/main/applications/rabbit-qa-harness) | Rabbit Application client (supports producers and consumers)
[applications/secret-encryption](https://github.com/ggreen/rabbitmq-dotNet-showCase/tree/main/applications/secret-encryption) | Generates a encrypted secret and or password based on a salt "CRYPTION_KEY" to be placed in an environment variable

## Environments or Input Properties


This module uses the [ConfigSettings](https://github.com/imani-solutions/Imani.Solutions.Core.DotNet/blob/master/API/Util/ConfigSettings.cs) object from the open source [Imani Solutions DotNet API](https://github.com/imani-solutions/Imani.Solutions.Core.DotNet).
This supports getting string, numbers, secrets or encrypted passwords properties from input arguments or environment variables.


You can set the properties using an environment variable or input argument (prefixed with --PROPERTY_NAME).


PROPERTY            | Notes    | Default
------------------  | -------- | ----------
CRYPTION_KEY        | Password encryption salt key | 
RABBIT_HOST         | Host name | localhost
RABBIT_PORT         | Listen port | 5672
RABBIT_CONNECTION_RETRY_SECS         | Automatic reconnect time interval | 15
RABBIT_VIRTUAL_HOST | The rabbit virtual host | /
RABBIT_USERNAME | The rabbit username |
RABBIT_PASSWORD | The encrypted rabbit password with value from [applications/password-encryption](https://github.com/ggreen/rabbitmq-dotNet-showCase/tree/main/applications/password-encryption) |
RABBIT_URI  | Supports multiple hosts/ports and TLS/SLL in a rabbit cluster ex: "amqps://guest:guest@localhost:5671/" |
RABBIT_CLIENT_NAME | The client provided name | 
RABBIT_WAIT_FOR_CONFIRMATION_SECS | Publish wait for connection | 30
RABBIT_PREFETCH_LIMIT | Prefetch limit (mainly for consumers) | 1000


## Sample Code

### Consumer Code

```c#

Rabbit rabbit = Rabbit.Connect();
var consumer = rabbit.ConsumerBuilder()
                      .SetExchange(exchange)
                      .AddQueue(queue,expectedRoutingKey)
                      .Build();

consumer.RegisterReceiver(receiver);

private void receiver(IModel channel, object sender, BasicDeliverEventArgs eventArg)
{
  var actual = Encoding.UTF8.GetString(eventArg.Body.ToArray());

  channel.BasicAck(eventArg.DeliveryTag,false);
}
```

### Publisher Code

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
- Enable handlers to detect blocked connections'
- Use Durable exchanges, durable queues and persistent messages for reliablity
- Set auto delete on temporary queues
- Set client name to assist with identifying application connections
- Keep exchange names the same case for producers and consumers



### Client Publisher Side

- Use Publisher confirms for consistency
- Add a handler for BasicReturn when messages not routed to a queue
- Set the mandatory message property to true to prevent Unroutable messages.


### Client Consumer Side
- Use manual ACK for consumers per message
- Set Prefetch limit for consumers 
  - Set the prefetch limit (round trip latency/processing time) (ex: MAX 1K)
- If you use dead letter exchanges/queues set to a very high count since DLX do not support  confirmation and can lost messages
- Use consumer (push) over get (pull)
- Name queues based on patterns of routing keys where applicable
 

## Server Side

Many of these tips are based on an [ERLang Solutions best practices video](https://www.youtube.com/watch?v=HzPOQsMWrGQ)

- Use 3 Node Rabbit MQ cluster 
- Use lazy queues for larger queues where messages live longer on server (example: batch processing)
- Limit number of queues (less than 10K queues)
- Queues are single threaded (est: 50K/s); For greater throughput you can use Consistent Hash or sharing plugins
- Reduce HA batch-sync-size to prevent introducing network partition dues to server pauses related to synchronize large batch sizes
- Prefer TCP keep alives over heart beats
  - tcp_keepalive_time=2 minutes 
  - tcp_keepalive_intv1=15
- Increase net_tick time to 90-120s (default is 60s)
- XFS is the recommended file system
- export ERL_CRASH_DUMP_SECONDS=1
- export RABBITMQ_SERVER_ADDITIONAL_ERL_ARGS="+hmqd off_heap"
- Reduce Embed msg. Default is $4K in queue index. Recommended disable (0).
- Limit RSA not used
- If over 10K connections increase TCP buffer size (ex: sndbuf=8192, recbuf=8192)
- Set max queue lenght limits when using federated queues on Disaster Recovery (DR) cluster
  - When using federation set  overflow=drop-head (default) on DR cluster


# Troubleshooting


## Consumers 

### Not receiving produced message

- Assert proper exception logging exists within the client code
- Assert [ConnectionFactory](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.ConnectionFactory.html).AutomaticRecoveryEnabled = true
- Set [ConnectionFactory](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.ConnectionFactory.html).NetworkRecoveryInterval equals to an appropriate value (ex: 15 seconds)
-  Register a handler for blocked and unblocked connections
```c#
//Example
connection.ConnectionBlocked += HandleBlocked;
connection.ConnectionUnblocked += HandleUnblocked;
 private void HandleBlocked(object sender, ConnectionBlockedEventArgs args)
{
   Console.WriteLine("WARNING: Connection is now blocked");
}

public void HandleUnblocked(object sender, EventArgs args)
{
   Console.WriteLine("INFO: Connection is now unblocked");
}
```


# Troubleshooting


- rabbitmq unable to connect to "epmd" (port 4369)
  - Add host name to Operating System host file (Linux ex: /etc/hosts)
- Errors with TLS Self Signed Certifications, then add the following
```C#
Ssl = new SslOption(){
                    Enabled = sslEnabled,
                 AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors} 
```

google-site-verification: google2f17241940533d44.html