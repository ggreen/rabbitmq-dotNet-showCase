dotnet add package RabbitMQ.Client --version 6.2.1
dotnet add package MSTest.TestFramework --version 2.1.2


```shell
dotnet run  --EXCHANGE_TYPE=fanout  --EXCHANGE=nylaExchange --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5671 --RABBI_HOST=localhost --ROUTING_KEY=1
```



# Loop test

```shell
#!/bin/bash
i="0"
while true; do
i=$[$i+1]

dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=nylaExchange --MESSAGE="{\"id\": \"$i\"}" --RABBIT_PORT=5671 --RABBI_HOST=localhost --ROUTING_KEY=1  --REPEAT_COUNT=100000
done
```

