dotnet add package RabbitMQ.Client --version 6.2.1
dotnet add package MSTest.TestFramework --version 2.1.2


```shell
dotnet run  --EXCHANGE_TYPE=fanout  --EXCHANGE=nylaExchange --QUEUE=nylaQueue --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5671 --RABBI_HOST=localhost
```
