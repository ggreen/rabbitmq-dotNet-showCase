

cp plugins/rabbitmq_delayed_message_exchange-3.8.9-0199d11c.ez /usr/local/Cellar/rabbitmq/3.8.9_1/plugins

rabbitmq-plugins enable rabbitmq_delayed_message_exchange


restart

```shell script
dotnet run --EXCHANGE=exchange_delayed_example --DELAY_MS=10000 --MESSAGE="Hello"
```



export CRYPTION_KEY=DFDJFDFDxdfd2323


dotnet run --EXCHANGE_TYPE=direct --EXCHANGE=exchange_delayed_example  --QUEUE=queue_delayed_example --QUEUE_TYPE=quorum --RABBIT_PORT=5672  --RABBIT_CLIENT_NAME=consumer --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"

dotnet run --EXCHANGE_TYPE=direct --EXCHANGE=exchange_delayed_example  --QUEUE=queue_delayed_example_classic --QUEUE_TYPE=classic --RABBIT_PORT=5672  --RABBIT_CLIENT_NAME=consumer --PRODUCERS=0 --CONSUMERS=1 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"


Docker 

```shell script
docker cp /Users/devtools/integration/messaging/rabbit/rabbit-devOps/plugins/rabbitmq_delayed_message_exchange-3.8.9-0199d11c.ez docker-rabbitmq-cluster_rabbitmq1_1:/opt/rabbitmq/plugins

docker cp /Users/devtools/integration/messaging/rabbit/rabbit-devOps/plugins/rabbitmq_delayed_message_exchange-3.8.9-0199d11c.ez rabbitmq2:/opt/rabbitmq/plugins

docker cp /Users/devtools/integration/messaging/rabbit/rabbit-devOps/plugins/rabbitmq_delayed_message_exchange-3.8.9-0199d11c.ez rabbitmq3:/opt/rabbitmq/plugins
```


dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=exchange_imani  --MESSAGE="{\"id\": \"1\"}"  --QUEUE_TYPE=classic --QUEUE=queue_delayed_example_classic --ROUTING_KEY=1  --REPEAT_COUNT=100 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=producer --SLEEP_PERIOD_MS=1000 --PRODUCERS=1 --CONSUMERS=0 --RABBIT_URIS="amqp://guest:guest@$HOSTNAME:5672/"



Limitations

- Does not support publisher confirms
- Does not support mandate flag to detect unrouted messages
- Risk of data lost
- Does not support millions of delayed messages
- Does not honor pause minority
- 