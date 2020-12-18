# Consumer



The following is an example to start the consumer.

export CRYPTION_KEY=DFDJFDFDxdfd2323
export RABBIT_USERNAME=admin
export ENCRYPTED_PASSWORD=s4ueQNUm/evSJo3doIGOxA==

```shell
 dotnet run --EXCHANGE_TYPE=direct --EXCHANGE=nylaExchange --QUEUE=nylaQueue  --RABBIT_PORT=5672 --RABBI_HOST=localhost --ROUTING_KEY=1 --SINGLE_ACTIVE_CONSUMER=True --RABBIT_CLIENT_NAME=consumer 
```
