# Producer/Publisher program


The following is an example to start the publisher.


export CRYPTION_KEY=DFDJFDFDxdfd2323
export RABBIT_USERNAME=guest
export RABBIT_PASSWORD=guest


## Direct Exchange

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=nylaExchange --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5672 --RABBIT_HOST=localhost --ROUTING_KEY=1  --REPEAT_COUNT=2000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=producer --SLEEP_PERIOD_MS=1000 --PRODUCER_COUNT=5

```

## RABBIT_URI

```shell script
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=nylaExchange --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5672 --ROUTING_KEY=1  --REPEAT_COUNT=2000 --WAIT_FOR_CONFIRMATION_SECONDS=1  --RABBIT_CLIENT_NAME=producer --SLEEP_PERIOD_MS=1000 --PRODUCER_COUNT=5 --RABBIT_URI="amqps://guest:guest@$HOSTNAME:5671/"
 ```



## Topic Exchange

```shell script
dotnet run --EXCHANGE_TYPE=topic --EXCHANGE=exchange_imani_topic --QUEUE=queue_imani_topic  --RABBIT_PORT=5672 --RABBI_HOST=localhost --ROUTING_KEY="play.upstairs.*"  --RABBIT_CLIENT_NAME=consumer
```


# Loop test

```shell
#!/bin/bash
i="0"
while true; do
i=$[$i+1]

dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=nylaExchange --MESSAGE="{\"id\": \"$i\"}" --RABBIT_PORT=5671 --RABBI_HOST=localhost --ROUTING_KEY=1  --REPEAT_COUNT=50
done
```

