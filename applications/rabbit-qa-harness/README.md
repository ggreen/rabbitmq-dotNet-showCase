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
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --MESSAGE="{\"id\": \"1\"}" --ROUTING_KEY=1  --REPEAT_COUNT=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=0 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"
 ```

### CONSUMERS

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --QUEUE=queue_imani --QUEUE_TYPE=quorum --RABBIT_PORT=5672 --ROUTING_KEY=1  --REPEAT_COUNT=2000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"
 ```


### single active customer

dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --QUEUE=queue_imani_single_active --QUEUE_TYPE=quorum  --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5672 --ROUTING_KEY=1  --REPEAT_COUNT=2000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/" --SINGLE_ACTIVE_CONSUMER=true


## Topic Exchange

### PRODUCER

```shell script

dotnet run  --EXCHANGE_TYPE=topic  --EXCHANGE=exchange_imani_topic  --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5672 --ROUTING_KEY=1  --REPEAT_COUNT=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=0 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"  --PRODUCERS=1 --CONSUMERS=0 --ROUTING_KEY="play.upstairs.today"
```

### CONSUMER upstairs


```shell script

dotnet run  --EXCHANGE_TYPE=topic   --EXCHANGE=exchange_imani_topic --QUEUE=queue_imani_upstairs_ex  --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5672 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --QUEUE_TYPE=quorum --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"  --ROUTING_KEY="play.upstairs.*"
```

### CONSUMER downstairs

```shell script

dotnet run  --EXCHANGE_TYPE=topic   --EXCHANGE=exchange_imani_topic --QUEUE=queue_imani_downstairs_ex  --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5672 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --QUEUE_TYPE=quorum --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"  --ROUTING_KEY="play.downstairs.*"
```

# Multiple Consumers/Producers


```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani --QUEUE=queue_imani --WAIT_FOR_CONFIRMATION_SECONDS=1   --PRODUCERS=0 --RABBIT_CLIENT_NAME=multipleConsumers --SLEEP_PERIOD_MS=1000  --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/" --QUEUE_TYPE=quorum  --CONSUMERS=10
```

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --MESSAGE="{\"id\": \"1\"}" --ROUTING_KEY=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=multipleProducers --SLEEP_PERIOD_MS=1000  --CONSUMERS=0 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"  --PRODUCERS=5 --REPEAT_COUNT=1000
```




# Security 

## Using Encryption

```shell script
cd applications/
cd secret-encryption/
export CRYPTION_KEY=DFDJFDFDxdfd2323
dotnet run amqp://guest:guest@$HOSTNAME:5672/
```

```shell script
export RABBIT_URIS="{cryption}jKYfHhsBcRw97sJNKrUn66eFtosu7Q9mCke376Dwie+0ThmsRy3HcbYhRg6f/kpQhnNPOdJb7tDFm1X4U5KrMQ=="
```

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --QUEUE=queue_imani  --RABBIT_PORT=5672 --ROUTING_KEY=1 --MESSAGE_SIZE=300  --REPEAT_COUNT=200000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=1 --QUEUE_TYPE=quorum 
```

## Application Security


The following will be refused

```
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --QUEUE=queue_imani  --RABBIT_PORT=5672 --ROUTING_KEY=1 --MESSAGE_SIZE=300  --REPEAT_COUNT=200000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=1 --QUEUE_TYPE=quorum  --RABBIT_URIS="amqp://app:app@$HOSTNAME:5672/"
```

Use exchange/queue based on naming conventions

```
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_app  --QUEUE=queue_app  --RABBIT_PORT=5672 --ROUTING_KEY=1 --MESSAGE_SIZE=300  --REPEAT_COUNT=200000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=1 --QUEUE_TYPE=quorum  --RABBIT_URIS="amqp://app:app@$HOSTNAME:5672/"
```

## SSL

dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --MESSAGE="{\"id\": \"1\"}" --ROUTING_KEY=1  --REPEAT_COUNT=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=0 --RABBIT_URIS="amqps://guest:guest@$HOSTNAME:5671/"

 ```