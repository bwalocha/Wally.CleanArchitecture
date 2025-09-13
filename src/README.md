# Wally.CleanArchitecture

[[_TOC_]]

## Sequence Diagram

```mermaid
---
config:
  theme: neo-dark
  look: neo
---
sequenceDiagram
    actor Client
    box transparent ApiGateway
        participant ApiGateway as ApiGateway
    end
    box transparent MicroService
        participant Controller as API Controller
        participant QueryHandler as Query Handler
        participant ReadOnlyRepository as ReadOnly Repository
    end
    Client->>Gateway: GET
    Gateway->>Controller: GET
    Controller->>QueryHandler: Query
    QueryHandler->>ReadOnlyRepository: GetAsync
    ReadOnlyRepository->>QueryHandler: Result
    QueryHandler->>Controller: Response
    Controller->>Gateway: Response
    Gateway->>Client: Response
```

## Components

### Swagger

UI: https://localhost:7197/swagger/index.html

### Health Checks

AppVer: https://localhost:7197
Health: https://localhost:7197/healthchecks

UI: https://localhost:7197/healthchecks-ui
UI cfg: https://localhost:7197/healthchecks-api/ui-settings

API: https://localhost:7197/healthchecks-api

Webhooks: https://localhost:7197/healthchecks-webhooks

[website](https://github.com/xabaril/AspNetCore.Diagnostics.HealthChecks)

### AutoMapper

[WebSite](https://automapper.org/)
