# Wally.CleanArchitecture

## dotnet template

- [x] .Net 6 WebApi
	- [x] AppSettings
	- [x] CORS
	- [ ] FluentValidation (IRequest)
	- [ ] OAuth2
	- [ ] RabbitMQ (Polly)
- [x] Swagger
- [x] Serilog
- [x] HealthChecks
	- [x] Configuration
- [x] MediatR
	- [ ] LogBehavior
	- [ ] TransactionBehavior
	- [ ] ValidationBehavior (IComamnd, IQuery)
	- [ ] EventBehavior
	- [ ] NotificationBehavior
- [x] Scrutor [github](https://github.com/khellang/Scrutor)
```
Could not load type 'Microsoft.EntityFrameworkCore.Metadata.RelationalModelAnnotations' from assembly 'Microsoft.EntityFrameworkCore.Relational, Version=6.0.0.0
```
- [ ] OData
- [ ] HttpGlobalExceptionFilter
- [x] EntityFrameworkCore (Concurrency)
- [ ] xUnit
	- [ ] ApprovalTests
	- [ ] IntegrationTests
	- [ ] UnitTests
- [ ] AppInsights Metrics
- [ ] Terraform Azure Infrastructure code (Service Principal, Group, AppInsights)
- [ ] Mapster [github](https://github.com/MapsterMapper/Mapster)
- [ ] AutoMapper
	- [ ] Validation
	- [x] Profiles
- [ ] SignalR

## TODO

- [ ] rename `Wally.CleanArchitecture` to `Wally.CleanArchitecture.Template`
- [ ] create ICommand and IQuery compatible with MediatR (new Wally.Lib.DDD)
- [ ] fix MapsterMapper

### Create

[doc](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates)

```
dotnet new --list
```

```
These templates matched your input:

Template Name                                 Short Name           Language    Tags
--------------------------------------------  -------------------  ----------  -------------------------------------
ASP.NET Core Empty                            web                  [C#],F#     Web/Empty
ASP.NET Core gRPC Service                     grpc                 [C#]        Web/gRPC
ASP.NET Core Web API                          webapi               [C#],F#     Web/WebAPI
ASP.NET Core Web App                          webapp,razor         [C#]        Web/MVC/Razor Pages
ASP.NET Core Web App (Model-View-Controller)  mvc                  [C#],F#     Web/MVC
ASP.NET Core with Angular                     angular              [C#]        Web/MVC/SPA
ASP.NET Core with React.js                    react                [C#]        Web/MVC/SPA
ASP.NET Core with React.js and Redux          reactredux           [C#]        Web/MVC/SPA
Blazor Server App                             blazorserver         [C#]        Web/Blazor
Blazor WebAssembly App                        blazorwasm           [C#]        Web/Blazor/WebAssembly/PWA
Class Library                                 classlib             [C#],F#,VB  Common/Library
Console App                                   console              [C#],F#,VB  Common/Console
dotnet gitignore file                         gitignore                        Config
Dotnet local tool manifest file               tool-manifest                    Config
EditorConfig file                             editorconfig                     Config
global.json file                              globaljson                       Config
MSTest Test Project                           mstest               [C#],F#,VB  Test/MSTest
MVC ViewImports                               viewimports          [C#]        Web/ASP.NET
MVC ViewStart                                 viewstart            [C#]        Web/ASP.NET
NuGet Config                                  nugetconfig                      Config
NUnit 3 Test Item                             nunit-test           [C#],F#,VB  Test/NUnit
NUnit 3 Test Project                          nunit                [C#],F#,VB  Test/NUnit
Protocol Buffer File                          proto                            Web/gRPC
Razor Class Library                           razorclasslib        [C#]        Web/Razor/Library/Razor Class Library
Razor Component                               razorcomponent       [C#]        Web/ASP.NET
Razor Page                                    page                 [C#]        Web/ASP.NET
Solution File                                 sln                              Solution
Web Config                                    webconfig                        Config
Windows Forms App                             winforms             [C#],VB     Common/WinForms
Windows Forms Class Library                   winformslib          [C#],VB     Common/WinForms
Windows Forms Control Library                 winformscontrollib   [C#],VB     Common/WinForms
Worker Service                                worker               [C#],F#     Common/Worker/Web
WPF Application                               wpf                  [C#],VB     Common/WPF
WPF Class library                             wpflib               [C#],VB     Common/WPF
WPF Custom Control Library                    wpfcustomcontrollib  [C#],VB     Common/WPF
WPF User Control Library                      wpfusercontrollib    [C#],VB     Common/WPF
xUnit Test Project                            xunit                [C#],F#,VB  Test/xUnit
```

## Install

```
cd C:\repo\wally\Wally.CleanArchitecture
dotnet new --install .
```

## Uninstall

```
cd C:\repo\wally\Wally.CleanArchitecture
dotnet new --uninstall .
```

## Init

### Tests

curl -X GET "https://localhost:7197/Users/3fa85f64-5717-4562-b3fc-2c963f66afa6" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"
curl -X GET "https://localhost:7197/Users" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"
curl -X GET "https://localhost:7197/Users?$orderby=Id&$top=1" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"

curl -X GET "https://localhost:7197/Users?$count=true&$orderby=Id&$top=0" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"

```
curl -X GET "https://localhost:7197/Users?$count=true&$orderby=Id&$skip=2&$top=2" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"
{"items":[{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa8","name":"testDbData3"}],"pageInfo":{"index":1,"size":2,"totalItems":3}}
```

[order desc,top 3](https://localhost:7197/Users?$orderby=Name%20desc&$top=3)
```json
{"items":[{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa8","name":"testDbData3"},{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa7","name":"testDbData2"},{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","name":"testDbData1"}],"pageInfo":{"index":0,"size":3,"totalItems":3}}
```