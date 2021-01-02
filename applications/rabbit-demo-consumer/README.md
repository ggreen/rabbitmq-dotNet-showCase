# Consumer



The following is an example to start the consumer.

export CRYPTION_KEY=IMANI
export RABBIT_USERNAME=guest
export RABBIT_PASSWORD={cryption}4nhz02bDnrJ7YvhE/OSEIw==

```shell
 dotnet run --EXCHANGE_TYPE=direct --EXCHANGE=nylaExchange --QUEUE=nylaQueue  --RABBIT_PORT=5672 --RABBI_HOST=localhost --ROUTING_KEY=1 --SINGLE_ACTIVE_CONSUMER=True --RABBIT_CLIENT_NAME=consumer 
```
