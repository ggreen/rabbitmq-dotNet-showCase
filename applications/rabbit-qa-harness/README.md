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






### CONSUMERS

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --QUEUE=queue_imani --QUEUE_TYPE=quorum --RABBIT_PORT=5672 --ROUTING_KEY=1  --REPEAT_COUNT=2000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=consumer --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"
 ```


### PRODUCER

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --MESSAGE="{\"id\": \"1\"}" --ROUTING_KEY=1  --REPEAT_COUNT=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=producer --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=0 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"
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








# Resilency Testing

export CRYPTION_KEY=DFDJFDFDxdfd2323

dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani --QUEUE=queue_imani --QUEUE_TYPE=quorum  --MESSAGE_SIZE=100 --ROUTING_KEY=1  --REPEAT_COUNT=1000000000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=harnessConsumer --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"

dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani --QUEUE=queue_imani --QUEUE_TYPE=quorum  --MESSAGE_SIZE=100 --ROUTING_KEY=1  --REPEAT_COUNT=1000000000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=harnessProducer --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=0 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"



K8s

kubectl port-forward rabbitmq-server-0 25672:15672


# Federation

dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=federated_exchange --QUEUE=federated_queue --QUEUE_TYPE=quorum  --MESSAGE_SIZE=100 --ROUTING_KEY=1  --REPEAT_COUNT=1000000000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=consumer --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"


dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=federated_exchange  --QUEUE=federated_queue  --QUEUE_TYPE=quorum  --MESSAGE_SIZE=100 --ROUTING_KEY=1  --REPEAT_COUNT=1000000000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=producer --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=0 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"



# Redelivery

Producer

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --MESSAGE="Invalid" --ROUTING_KEY=1  --REPEAT_COUNT=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=producer --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONTENT_TYPE="application/json" --CONSUMERS=0 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"
```



Consumer

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --QUEUE=queue_imani_single_active --QUEUE_TYPE=quorum  --RABBIT_PORT=5672 --ROUTING_KEY=1  --REPEAT_COUNT=2000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/" --SINGLE_ACTIVE_CONSUMER=true --CONSUMER_IS_NASK_REQUEUED=true
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

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_hare  --QUEUE=queue_hare  --RABBIT_PORT=5672 --ROUTING_KEY=1 --MESSAGE_SIZE=300  --REPEAT_COUNT=200000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=bunny --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=1 --QUEUE_TYPE=quorum  --RABBIT_URIS="amqp://bunny:bunny@$HOSTNAME:5672/"
```

Use exchange/queue based on naming conventions

```
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_bunny  --QUEUE=queue_bunny  --RABBIT_PORT=5672 --ROUTING_KEY=1 --MESSAGE_SIZE=300  --REPEAT_COUNT=200000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=bunny --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=1 --QUEUE_TYPE=quorum  --RABBIT_URIS="amqp://bunny:bunny@$HOSTNAME:5672/"
```


dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_unittest  --QUEUE=queue_unittest  --RABBIT_PORT=5672 --ROUTING_KEY=1 --MESSAGE_SIZE=300  --REPEAT_COUNT=200000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=bunny --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=1 --QUEUE_TYPE=quorum  --RABBIT_URIS="amqp://hare:hare@$HOSTNAME:5672/"

## SSL

```
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --MESSAGE="{\"id\": \"1\"}" --ROUTING_KEY=1  --REPEAT_COUNT=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=0 --RABBIT_URIS="amqps://guest:guest@$HOSTNAME:5671/"

 ```


 ## Vhost

The following will not be allowed

```shell script
 dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_app  --QUEUE=queue_app  --QUEUE_TYPE=quorum  --MESSAGE="{\"id\": \"1\"}" --ROUTING_KEY=1  --REPEAT_COUNT=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=1 --RABBIT_URIS="amqps://guest:guest@$HOSTNAME:5671/app"
 ```


The following will be allowed

```shell script
  dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_app  --QUEUE=queue_app  --QUEUE_TYPE=quorum  --MESSAGE="{\"id\": \"1\"}" --ROUTING_KEY=1  --REPEAT_COUNT=1 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=rabbitMqQAHarness --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=1 --RABBIT_URIS="amqps://app:app@$HOSTNAME:5671/app"
  ```