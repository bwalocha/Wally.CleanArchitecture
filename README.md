# Wally.CleanArchitecture

## Installation

```
dotnet new --install wally.cleanarchitecture
```

## Usage

```
dotnet new wally.cleanarchitecture --author wally --output . --name MyCompanyName.MyAppName
```

## Details

[Architecture](https://viewer.diagrams.net/?tags=%7B%7D&highlight=0000ff&edit=_blank&layers=1&nav=1&title=Wally.CleanArchitecture#R5Vrfc%2BI2EP5reAzj34ZHDCTNTHrhjtxc%2B8QIvLHV2JYrywHfX18JyRhbSUrnIM40eSDSeiWv9ttvtRIM7Gm6u6Eoj38nISQDywh3A3s2sCzTHI35PyGpDhJHSiKKQyVrBEv8E5TQUNISh1C0FBkhCcN5W7ghWQYb1pIhSsm2rfZIkvZbcxSBJlhuUKJLf%2BCQxVI6svxG%2FhvgKK7fbHpqxSmqldVKihiFZHsksucDe0oJYbKV7qaQCO%2FVfpHjrl95ejCMQsZOGXDrPN4XZTV5CCt8s76%2F%2Brq6%2FnFlunKaZ5SUasXKWlbVLqCkzEIQs5gDO9jGmMEyRxvxdMtR57KYpYl6HKIiPug%2B4iSZkoTQ%2FUQ2AB8TqBcCZbB7dSnmwUE8tICkwGjFVdQA21M%2BVVFl%2B6q%2FbSByfE%2FK4iN4HFcpIhUW0WHuxnO8oZz3Xxxpao5czu%2Fm04eBxSc0br8s599k20u4McGa8lYkWt8Xs8nDXKrN%2BBDR7iAAIY9J1SWUxSQiGUrmjTRoMDJ4r9G5IyRXYPwFjFWKYKhkpI2bfKd40duYcLtISTfwlisUTRGNgL2lZ7wMMoUEMfzcNuT8gPkaYHm5TnARSyh4QinKFD4LFl6vWGhQFECfMV%2BZZUw0BFCRy0z%2FiHfCzceuywnO2N48Nxi4My5BCY4yLthw3wFPRAFO9yk%2FeCQZUxCYViOf4TTii0jwmn%2BinyUFsTiU5ytlU8G7E95dqu6weI7Ok9bccTutOcZIS2ueo2e1WnZ2XDwNl5u5SmiL%2B6Vq6Rlt8f2hnc5eVrtfPNzef1n2wTAOCK3%2BUOP3nT9FZ%2BjW3dnu%2BOGsOu4tgGLuXxFMe%2BEZ6WqdSFe3T7ZaWlSsRZ3F%2FfERmBpBBhTxRVwH0qrz8dNp09N1PZ2erk5P17oQEGMNCMlKbua1SlZBr%2By6MoaGP2pTzDG8S5LsF0ihhi5EYB7VmmOvjfqoA6cktRp1XH93JnJH%2FtCoM7qazLeN4Xhkjw9%2F7allHtCmnlCKqiM1RaVXl%2BCMrReX8JqlXX3fNjuxKi1oIvfg7l%2BoAfRo3m82R8GslwLvHcym19kvzIsG8wk7hn1qgdfnjmFr0FLgmbiAq5ySXfUh9o0M2JbQJ5xFsr5L8IZ7hmSrG8Rgi6oz1nm23d1I9OPru9Z5pqMB9Mrh9YUy7mOcX1s0HdkdmtpmzzR1TqWp3SdPzZEWB%2F%2FDM%2FHJWPR6JtYp2ZyJ9aru05yJfcPtN1fq10atAqWbHXsuxc9IG%2FdE2vSawfTr7C2sNceLiOTbe3KH1pAsSIHFRs8frQljJG17sNadKM4w4XmdQQWj5Anqy%2B6MZKDffxtGML8WtCpilAtT0l0kvjYZ7iklP1fcXm4P1LwFOn8GSV%2FzPIwyx93qw9cYVRckrWPsxa7ODQ20EDG0RoW%2B0%2FSR52pjRJJbfr1bzVT%2FfEnOd%2F794s8ZvWdBqFfsnxwS37B7hkS%2FjE2hKISTLOPvEkodmeIJ2CZWm8c7wpQWGwQCGbnxrYKyLgha3w6epTrwP1Yus%2FRc9vFg4XOg5NuFIHHMDnFM61KQ8G7z9bm8h2p%2BhWDP%2FwE%3D)

- [x] .Net 6 WebApi
	- [x] AppSettings
	- [x] CORS
	- [ ] FluentValidation (IRequest)
	- [ ] OAuth2
	- [x] RabbitMQ (Polly)
- [x] Swagger
- [x] Serilog
- [x] HealthChecks
	- [x] Configuration
- [x] MediatR
	- [x] LogBehavior
	- [x] TransactionBehavior
	- [x] ValidationBehavior (IComamnd, IQuery)
	- [ ] EventBehavior
	- [ ] NotificationBehavior
- [x] Scrutor [github](https://github.com/khellang/Scrutor)
- [x] OData
- [x] HttpGlobalExceptionFilter
- [x] EntityFrameworkCore (Concurrency)
- [ ] xUnit
	- [ ] ApprovalTests
	- [x] IntegrationTests
	- [x] UnitTests
	- [x] ConventionTests
- [ ] AppInsights Metrics
- [ ] Terraform Azure Infrastructure code (Service Principal, Group, AppInsights)
- [ ] Mapster [github](https://github.com/MapsterMapper/Mapster)
- [ ] AutoMapper
	- [ ] Validation
	- [x] Profiles
- [ ] SignalR
- [ ] Create ICommand and IQuery compatible with MediatR (new Wally.Lib.DDD)
- [ ] Support MapsterMapper
- [ ] Template for ReverseProxy (template argument for Service/Proxy selector)

## Info

[doc](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates)

## TODO

- [ ] DomainEvent naming convention tests
- [ ] DomainEvent with `[ExcludeFromCodeCoverage]` attribute convention tests
- [ ] Fix `[CS1591] Missing XML comment` warnings
