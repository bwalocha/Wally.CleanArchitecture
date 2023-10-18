# Wally.CleanArchitecture

[[_TOC_]]

## ToDo

I. `ValueObject` abstract base:

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

II. IRequestContext

III. CorrelationId + SeriLog.Enrich.CorrelationId

https://www.code4it.dev/blog/serilog-correlation-id/
https://github.com/ekmsystems/serilog-enrichers-correlation-id

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

### Tests

curl -X GET "https://localhost:7197/Users/3fa85f64-5717-4562-b3fc-2c963f66afa6" -H "accept:
application/json;odata.metadata=minimal;odata.streaming=true"  
curl -X GET "https://localhost:7197/Users" -H "accept: application/json;odata.metadata=minimal;odata.streaming=true"  
curl -X GET "https://localhost:7197/Users?$orderby=Id&$top=1" -H "accept:
application/json;odata.metadata=minimal;odata.streaming=true"  
curl -X GET "https://localhost:7197/Users?$count=true&$orderby=Id&$top=0" -H "accept:
application/json;odata.metadata=minimal;odata.streaming=true"  
curl -X GET "https://localhost:7197/Users?$count=true&$orderby=Id&$skip=2&$top=2" -H "accept:
application/json;odata.metadata=minimal;odata.streaming=true"

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

curl -X GET "https://localhost:7197/Users?$orderby=Name%20desc&$top=3" -H "accept:
application/json;odata.metadata=minimal;odata.streaming=true"

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
