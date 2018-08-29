# Lykke.Service.Balances 

# Purpose

  - Keeping the read-model of balances of each wallet, and of the whole market.

# Contracts

Input (v1, Matching Engine events; RabbitMQ, protobuf):
  - CashInEvent, CashOutEvent, CashTransferEvent, ExecutionEvent.

Input (v2, "balances" context; RabbitMQ, protobuf):
  - BalanceUpdatedEvent, UpdateTotalBalanceCommand.

Output ("balances" context; RabbitMQ, protobuf):
  - BalanceUpdatedEvent.
  
Output (HTTP):
  - get wallet balances by wallet id;
  - get wallet balances by wallet id and asset id;
  - get total market balances;
  - get total market balances by asset id;

# Scaling

| Image | Resources | Default instances number | Max instances |
| ------ | ------ | ------ | ------ |
| Lykke.Service.Balances | C1-R1 | 2 | 10 |

# Dependencies
  - Azure Table Storage (logs and data);
  - Redis (cache);
  - RabbitMQ (new ME events; Registration events; Cqrs);

# Service owners
Core team
