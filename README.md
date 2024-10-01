# Wally.CleanArchitecture

## Installation

```
dotnet new install Wally.CleanArchitecture.Template
```

## Usage

```
dotnet new wally.cleanarchitecture --output . --name MyCompanyName.MyAppName --serviceName MyServiceName -proxy=true -service=true -frontend=true -storybook=true
```

## Details

[Architecture](https://viewer.diagrams.net/?tags=%7B%7D&highlight=0000ff&edit=_blank&layers=1&nav=1&title=Wally.CleanArchitecture#R5Vrfc%2BI2EP5reAzj34ZHDCTNTHrhjtxc%2B8QIvLHV2JYrywHfX18JyRhbSUrnIM40eSDSeiWv9ttvtRIM7Gm6u6Eoj38nISQDywh3A3s2sCzTHI35PyGpDhJHSiKKQyVrBEv8E5TQUNISh1C0FBkhCcN5W7ghWQYb1pIhSsm2rfZIkvZbcxSBJlhuUKJLf%2BCQxVI6svxG%2FhvgKK7fbHpqxSmqldVKihiFZHsksucDe0oJYbKV7qaQCO%2FVfpHjrl95ejCMQsZOGXDrPN4XZTV5CCt8s76%2F%2Brq6%2FnFlunKaZ5SUasXKWlbVLqCkzEIQs5gDO9jGmMEyRxvxdMtR57KYpYl6HKIiPug%2B4iSZkoTQ%2FUQ2AB8TqBcCZbB7dSnmwUE8tICkwGjFVdQA21M%2BVVFl%2B6q%2FbSByfE%2FK4iN4HFcpIhUW0WHuxnO8oZz3Xxxpao5czu%2Fm04eBxSc0br8s599k20u4McGa8lYkWt8Xs8nDXKrN%2BBDR7iAAIY9J1SWUxSQiGUrmjTRoMDJ4r9G5IyRXYPwFjFWKYKhkpI2bfKd40duYcLtISTfwlisUTRGNgL2lZ7wMMoUEMfzcNuT8gPkaYHm5TnARSyh4QinKFD4LFl6vWGhQFECfMV%2BZZUw0BFCRy0z%2FiHfCzceuywnO2N48Nxi4My5BCY4yLthw3wFPRAFO9yk%2FeCQZUxCYViOf4TTii0jwmn%2BinyUFsTiU5ytlU8G7E95dqu6weI7Ok9bccTutOcZIS2ueo2e1WnZ2XDwNl5u5SmiL%2B6Vq6Rlt8f2hnc5eVrtfPNzef1n2wTAOCK3%2BUOP3nT9FZ%2BjW3dnu%2BOGsOu4tgGLuXxFMe%2BEZ6WqdSFe3T7ZaWlSsRZ3F%2FfERmBpBBhTxRVwH0qrz8dNp09N1PZ2erk5P17oQEGMNCMlKbua1SlZBr%2By6MoaGP2pTzDG8S5LsF0ihhi5EYB7VmmOvjfqoA6cktRp1XH93JnJH%2FtCoM7qazLeN4Xhkjw9%2F7allHtCmnlCKqiM1RaVXl%2BCMrReX8JqlXX3fNjuxKi1oIvfg7l%2BoAfRo3m82R8GslwLvHcym19kvzIsG8wk7hn1qgdfnjmFr0FLgmbiAq5ySXfUh9o0M2JbQJ5xFsr5L8IZ7hmSrG8Rgi6oz1nm23d1I9OPru9Z5pqMB9Mrh9YUy7mOcX1s0HdkdmtpmzzR1TqWp3SdPzZEWB%2F%2FDM%2FHJWPR6JtYp2ZyJ9aru05yJfcPtN1fq10atAqWbHXsuxc9IG%2FdE2vSawfTr7C2sNceLiOTbe3KH1pAsSIHFRs8frQljJG17sNadKM4w4XmdQQWj5Anqy%2B6MZKDffxtGML8WtCpilAtT0l0kvjYZ7iklP1fcXm4P1LwFOn8GSV%2FzPIwyx93qw9cYVRckrWPsxa7ODQ20EDG0RoW%2B0%2FSR52pjRJJbfr1bzVT%2FfEnOd%2F794s8ZvWdBqFfsnxwS37B7hkS%2FjE2hKISTLOPvEkodmeIJ2CZWm8c7wpQWGwQCGbnxrYKyLgha3w6epTrwP1Yus%2FRc9vFg4XOg5NuFIHHMDnFM61KQ8G7z9bm8h2p%2BhWDP%2FwE%3D)

- [x] .Net 8 WebApi ReverseProxy
    - [x] YARP
- [x] .Net 8 WebApi MicroService
	- [x] AppSettings
	- [x] CORS
	- [x] FluentValidation
	- [x] OAuth2
- [x] Swagger
- [x] Serilog
- [x] HealthChecks
    - [x] UI
	- [x] MicroService endpoint
- [x] MediatR
	- [x] LogBehavior
	- [x] TransactionBehavior
	- [x] ValidatorBehavior (IComamnd, IQuery)
	- [x] DomainEventBehavior
	- [ ] NotificationBehavior
    - [x] UpdateMetadataBehavior
    - [x] SoftDeleteBehavior
- [x] MassTransit
    - [x] Azure ServiceBus
    - [x] RabbitMQ
    - [x] Kafka
- [x] Scrutor [github](https://github.com/khellang/Scrutor)
- [x] OData
- [x] HttpGlobalExceptionFilter
- [ ] Entity Framework
    - [x] MySql
    - [x] MS Sql
    - [x] PostgreSQL
    - [ ] Concurrency
- [ ] xUnit
	- [ ] ApprovalTests
	- [x] IntegrationTests
	- [x] UnitTests
	- [x] ConventionTests
- [ ] AppInsights Metrics
- [ ] IaC
    - [ ] Terraform
    - [ ] Bicep
- [ ] Mapper
    - [-] Mapster [github](https://github.com/MapsterMapper/Mapster)
    - [x] AutoMapper
        - [x] Validation
        - [x] Profiles
- [x] SignalR
- [ ] GitHub
    - [ ] Workflow

## Info

[doc](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates)

### Storybook

```
npm create vite@latest wally.cleanarchitecture.storybook.webapp -- --template vue-ts
cd ./wally.cleanarchitecture.storybook.webapp
npx storybook@next init --package-manager npm --parser ts --builder vite --disable-telemetry
```

## TODO

- [ ] Fix `[CS1591] Missing XML comment` warnings
- [x] Move CORS settings to ApiGateway
- [x] Authentication in ApiGateway
- [ ] Authorization in MicroService
- [x] SignalR Hub Service
- [ ] Move Application.Contracts to Presentation layer
- [x] EntityId [YT](https://www.youtube.com/watch?v=B3Iq346KwUQ&t=655s)
- [ ] Entity Framework Repository [GH](https://github.com/ffernandolima/ef-core-data-access)
- [ ] Multitenancy [GH](https://github.com/Finbuckle/Finbuckle.MultiTenant)
- [ ] IRequestContext with CorrelationId + SeriLog.Enrich.CorrelationId [1.](https://www.code4it.dev/blog/serilog-correlation-id/) [2.](https://github.com/ekmsystems/serilog-enrichers-correlation-id)
- [ ] Graceful Shutdown [post](https://dev.to/arminshoeibi/real-graceful-shutdown-in-kubernetes-and-aspnet-core-2290)