# Producer/Publisher program


The following is an example to start the publisher.

```shell script
export CRYPTION_KEY=DFDJFDFDxdfd2323
export RABBIT_USERNAME=guest
export RABBIT_PASSWORD=guest
```


### producer and consumer

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --QUEUE=queue_imani  --RABBIT_PORT=5672 --ROUTING_KEY=1 --MESSAGE_SIZE=300  --REPEAT_COUNT=200000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=1 --QUEUE_TYPE=quorum --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"
```



### PRODUCER

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --MESSAGE="{\"id\": \"1\"}" --ROUTING_KEY=1  --REPEAT_COUNT=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=0 --RABBIT_URIS="amqps://guest:guest@$HOSTNAME:5671/"
 ```

### CONSUMERS

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --QUEUE=queue_imani  --RABBIT_PORT=5672 --ROUTING_KEY=1  --REPEAT_COUNT=2000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqps://guest:guest@$HOSTNAME:5671/"
 ```


### single active customer

dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --QUEUE=queue_imani_single_active --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5672 --ROUTING_KEY=1  --REPEAT_COUNT=2000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URI="amqps://guest:guest@$HOSTNAME:5671/" --SINGLE_ACTIVE_CONSUMER=true


## Topic Exchange

### PRODUCER

```shell script

dotnet run  --EXCHANGE_TYPE=topic  --EXCHANGE=exchange_imani_topic  --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5672 --ROUTING_KEY=1  --REPEAT_COUNT=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=0 --RABBIT_URIS="amqps://guest:guest@$HOSTNAME:5671/"  --PRODUCERS=1 --CONSUMERS=0 --ROUTING_KEY="play.upstairs.*"
```

### CONSUMER upstairs


```shell script

dotnet run  --EXCHANGE_TYPE=topic   --EXCHANGE=exchange_imani_topic --QUEUE=queue_imani_upstairs_ex  --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5672 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqps://guest:guest@$HOSTNAME:5671/"  --ROUTING_KEY="play.upstairs.*"
```

### CONSUMER downstairs

```shell script

dotnet run  --EXCHANGE_TYPE=topic   --EXCHANGE=exchange_imani_topic --QUEUE=queue_imani_downstairs_ex  --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5672 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqps://guest:guest@$HOSTNAME:5671/"  --ROUTING_KEY="play.downstairs.*"
```

# Performance Testing

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --MESSAGE="{\"id\": \"1\"}" --ROUTING_KEY=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000  --CONSUMERS=0 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"  --PRODUCERS=10 --REPEAT_COUNT=1000
```


```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani --QUEUE=queue_imani --WAIT_FOR_CONFIRMATION_SECONDS=1   --PRODUCERS=0 --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000  --RABBIT_URIS="amqps://guest:guest@$HOSTNAME:5671/" --QUEUE_TYPE=quorum  --CONSUMERS=10  
```