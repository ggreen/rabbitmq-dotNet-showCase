# Producer/Publisher program


The following is an example to start the publisher.


```shell
dotnet run  --EXCHANGE_TYPE=direct  --EXCHANGE=nylaExchange --MESSAGE="{\"id\": \"1\"}" --RABBIT_PORT=5671 --RABBI_HOST=localhost --ROUTING_KEY=1  --REPEAT_COUNT=2 --WAIT_FOR_CONFIRMATION_SECONDS=1 
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

