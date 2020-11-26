dotnet add package RabbitMQ.Client --version 6.2.1
dotnet add package MSTest.TestFramework --version 2.1.2


```shell
 dotnet run --EXCHANGE_TYPE=direct --EXCHANGE=nylaExchange --QUEUE=nylaQueue  --RABBIT_PORT=5671 --RABBI_HOST=localhost --ROUTING_KEY=1 --SINGLE_ACTIVE_CONSUMER=True
```
