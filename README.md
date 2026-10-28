# credito-cartao (Emissão de Cartão)

Microsserviço que **consome** `PropostaGerada`, emite cartões conforme a proposta aprovada e publica `CartoesEmitidos`.

## Processos

| Projeto | Responsabilidade |
|---------|------------------|
| **Api** | REST `GET /api/v1/cartoes/{clienteId}` + health |
| **Worker** | Consumer RabbitMQ + outbox publisher |

## Stack

- .NET 10 / ASP.NET Core + Worker Service
- EF Core + SQL Server (database `EmissaoCartao`)
- RabbitMQ (consumer com DLQ + outbox + Polly retry)
- Idempotência (`processed_events` + unique `propostaId+sequencia`)
- xUnit + FluentAssertions + NSubstitute

## Regras

| Proposta | Emissão |
|----------|---------|
| Negada / 0 cartões | Só marca evento processado (sem cartão) |
| Aprovada 1 cartão | 1 cartão com `LimitePorCartao` |
| Aprovada 2 cartões | 2 cartões com `LimitePorCartao` cada |

Número do cartão é **sintético/mascarado** (não é PAN real).

## Resiliência (Proposta ↔ Cartão)

- Retentativas: outbox (até 5) + Polly no publish
- DLQ: `cartao.proposta-gerada.dlq`
- Consistência eventual via outbox (sem 2PC)

## Subir

1. Infra (Cadastro) + Proposta:

```bash
cd ../CadastroCliente/CadastroCliente && docker compose up -d
cd ../../PropostaCredito && docker compose up -d --build
```

2. Api + Worker:

```bash
cd ../EmissaoCartao
docker compose up -d --build
```

| Serviço | URL |
|---------|-----|
| API Cartão | http://localhost:8082 |
| Health | http://localhost:8082/health |

## Fluxo

```
PropostaGerada → Worker (fila + DLQ) → emissão idempotente → outbox
  → CartoesEmitidos → RabbitMQ
Api consulta GET /cartoes/{clienteId}
```
