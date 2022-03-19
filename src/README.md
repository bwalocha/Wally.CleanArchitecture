# Wally.CleanArchitecture

[[_TOC_]]

## ToDo

1. Add `AggregateRoot` `Shadow Properties` or `AuditableEntity` abstract class for selected `AggregateRoot`
   and `Entity`:

```c#
public DateTime Created { get; set; }

public string? CreatedBy { get; set; }

public DateTime? LastModified { get; set; }

public string? LastModifiedBy { get; set; }
```

2. `ValueObject` abstract base:

[MSDN](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/implement-value-objects)

```c#
public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }
        return ReferenceEquals(left, null) || left.Equals(right);
    }

    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !(EqualOperator(left, right));
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }
    // Other utility methods
}
```

3. Create `DependencyInjection` class in `Infrastructure`

[GitHub](https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/Infrastructure/DependencyInjection.cs)

4. Use Aspect Oriented Programming

[YouTube](https://www.youtube.com/watch?v=dLPKwEeqwgU&ab_channel=NickChapsas)

## Components

### Swagger

UI: https://localhost:7197/swagger/index.html

### Serilog

`serilog.json`

```json
{
  "Serilog": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.ApplicationInsights"
    ],
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "Debug"
      },
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console",
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}{NewLine}"
        }
      },
      {
        "Name": "ApplicationInsights",
        "Args": {
          "instrumentationKey": "---",
          "restrictedToMinimumLevel": "Information",
          "telemetryConverter": "Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter, Serilog.Sinks.ApplicationInsights"
        }
      },
      {
        "Name": "MSSqlServer",
        "Args": {
          "connectionString": "---",
          "tableName": "Log",
          "columnOptionsSection": {
            "addStandardColumns": [ "LogEvent" ],
            "removeStandardColumns": [ "Properties" ]
          }
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithThreadId"
    ],
    "Properties": {
      "Application": "Wally.CleanArchitecture"
    }
  }
}
```

### Health Checks

AppVer: https://localhost:7197
Health: https://localhost:7197/healthchecks

UI: https://localhost:7197/healthchecks-ui
UI cfg: https://localhost:7197/healthchecks-api/ui-settings

API: https://localhost:7197/healthchecks-api

Webhooks: https://localhost:7197/healthchecks-webhooks

[website](https://github.com/xabaril/AspNetCore.Diagnostics.HealthChecks)

`appsettings.json`

```json
...
  "HealthChecks-UI": {
    "DisableMigrations": true,
    "HealthChecks": [
      {
        "Name": "Poll Manager",
        "Uri": "/healthChecks"
      }
    ],
    "Webhooks": [
      {
        "Name": "",
        "Uri": "",
        "Payload": "",
        "RestoredPayload": ""
      }
    ],
    "EvaluationTimeOnSeconds": 10,
    "MinimumSecondsBetweenFailureNotifications": 60,
    "MaximumExecutionHistoriesPerEndpoint": 15,
    "HealthCheckDatabaseConnectionString": "Data Source=healthChecks"
  }
...
```

Known Issues:

- [ ] https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/issues/845

### Mapster

Known Issuees:

- [ ] https://github.com/MapsterMapper/Mapster/issues/381

```
System.MissingMethodException: Constructor on type 'Mapster.EFCore.MapsterAsyncEnumerable`1[[Wally.CleanArchitecture.Contracts.Responses.Users.GetUserResponse, Wally.CleanArchitecture.Contracts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]' not found.
   at System.RuntimeType.CreateInstanceImpl(BindingFlags bindingAttr, Binder binder, Object[] args, CultureInfo culture)
   at System.Activator.CreateInstance(Type type, BindingFlags bindingAttr, Binder binder, Object[] args, CultureInfo culture, Object[] activationAttributes)
   at System.Activator.CreateInstance(Type type, Object[] args)
   at Mapster.EFCore.MapsterQueryableProvider.ExecuteAsync[TResult](Expression expression, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ExecuteAsync[TSource,TResult](MethodInfo operatorMethodInfo, IQueryable`1 source, Expression expression, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ExecuteAsync[TSource,TResult](MethodInfo operatorMethodInfo, IQueryable`1 source, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleAsync[TSource](IQueryable`1 source, CancellationToken cancellationToken)
   at Wally.CleanArchitecture.Persistence.Repository`1.GetAsync[TResult](Guid id, CancellationToken cancellationToken) in C:\repo\wally\wally.CleanArchitecture\src\Wally.CleanArchitecture.Persistence\Repository.cs:line 0
   at Wally.CleanArchitecture.Application.Users.Queries.GetUserQueryHandler.HandleAsync(GetUserQuery query, CancellationToken cancellationToken) in C:\repo\wally\wally.CleanArchitecture\src\Wally.CleanArchitecture.Application\Users\Queries\GetUserQueryHandler.cs:line 19
   at Wally.CleanArchitecture.Application.Abstractions.QueryHandler`2.Handle(TQuery query, CancellationToken cancellationToken) in C:\repo\wally\wally.CleanArchitecture\src\Wally.CleanArchitecture.Application\Abstractions\QueryHandler.cs:line 16
   at MediatR.Internal.RequestHandlerWrapperImpl`2.<>c__DisplayClass1_0.<Handle>g__Handler|0()
   at MediatR.Pipeline.RequestExceptionProcessorBehavior`2.Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate`1 next)
```

### AutoMapper

[WebSite](https://automapper.org/)

### Tests

curl -X GET "https://localhost:7197/Users/3fa85f64-5717-4562-b3fc-2c963f66afa6" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"  
curl -X GET "https://localhost:7197/Users" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"  
curl -X GET "https://localhost:7197/Users?$orderby=Id&$top=1" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"  
curl -X GET "https://localhost:7197/Users?$count=true&$orderby=Id&$top=0" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"  
curl -X GET "https://localhost:7197/Users?$count=true&$orderby=Id&$skip=2&$top=2" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"
```json
{
	"items":[
		{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa8","name":"testDbData3"}
	],
	"pageInfo":{
		"index":1,
		"size":2,
		"totalItems":3
	}
}
```

curl -X GET "https://localhost:7197/Users?$orderby=Name%20desc&$top=3" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"
```json
{
	"items":[
		{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa8","name":"testDbData3"},
		{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa7","name":"testDbData2"},
		{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","name":"testDbData1"}
	],
	"pageInfo":{
		"index":0,
		"size":3,
		"totalItems":3
	}
}
```