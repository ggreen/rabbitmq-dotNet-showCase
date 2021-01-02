# Consumer



The following is an example to start the consumer.

export CRYPTION_KEY=IMANI
export RABBIT_USERNAME=guest
export RABBIT_PASSWORD={cryption}4nhz02bDnrJ7YvhE/OSEIw==


## Direct Exchange
```shell
 dotnet run --EXCHANGE_TYPE=direct --EXCHANGE=nylaExchange --QUEUE=nylaQueue  --RABBIT_PORT=5672 --RABBI_HOST=localhost --ROUTING_KEY=1 --SINGLE_ACTIVE_CONSUMER=True --RABBIT_CLIENT_NAME=consumer 
```


## Topic Exchange

```shell script
 dotnet run --EXCHANGE_TYPE=topic --EXCHANGE=exchange_imani_topic --QUEUE=queue_imani_topic_upstairs  --RABBIT_PORT=5672 --RABBI_HOST=localhost --ROUTING_KEY="play.upstairs.*"  --RABBIT_CLIENT_NAME=consumer 
 ```

```shell script
  dotnet run --EXCHANGE_TYPE=topic --EXCHANGE=exchange_imani_topic --QUEUE=queue_imani_topic_downstairs  --RABBIT_PORT=5672 --RABBI_HOST=localhost --ROUTING_KEY="play.downstairs.*"  --RABBIT_CLIENT_NAME=consumer 
  ```