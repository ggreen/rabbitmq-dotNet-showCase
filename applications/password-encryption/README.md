# Consumer



The following is an example to start the consumer.

```shell
 dotnet run --EXCHANGE_TYPE=direct --EXCHANGE=nylaExchange --QUEUE=nylaQueue  --RABBIT_PORT=5672 --RABBI_HOST=localhost --ROUTING_KEY=1 --SINGLE_ACTIVE_CONSUMER=True --RABBIT_CLIENT_NAME=consumer --RABBIT_USERNAME=guest --RABBIT_PASSWORD=guest
```
