using System.Linq;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Swagger;

internal class ODataQueryOptionsOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var odataQueryParameterTypes = context.ApiDescription.ActionDescriptor.Parameters
			.Where(p => p.ParameterType.IsAssignableTo(typeof(ODataQueryOptions)))
			.ToList();

		if (!odataQueryParameterTypes.Any())
		{
			return;
		}

		// Remove the large queryOptions field from Swagger which gets added by default...
		foreach (var queryParamType in odataQueryParameterTypes)
		{
			var paramToRemove = operation.Parameters.SingleOrDefault(p => p.Name == queryParamType.Name);
			if (paramToRemove != null)
			{
				operation.Parameters.Remove(paramToRemove);
			}
		}

		// ...and add our own query parameters.
		operation.Parameters.Add(
			new OpenApiParameter
			{
				Name = "$filter",
				In = ParameterLocation.Query,
				Description = "Filter the results",
				Required = false,
				Schema = new OpenApiSchema { Type = "string", },
			});

		operation.Parameters.Add(
			new OpenApiParameter
			{
				Name = "$search",
				In = ParameterLocation.Query,
				Description = "Search term",
				Required = false,
				Schema = new OpenApiSchema { Type = "string", },
			});

		operation.Parameters.Add(
			new OpenApiParameter
			{
				Name = "$orderby",
				In = ParameterLocation.Query,
				Description = "Order the results",
				Required = false,
				Schema = new OpenApiSchema { Type = "string", },
			});

		operation.Parameters.Add(
			new OpenApiParameter
			{
				Name = "$select",
				In = ParameterLocation.Query,
				Description = "Select the properties to be returned in the response",
				Required = false,
				Schema = new OpenApiSchema { Type = "string", Deprecated = true, },
			});

		operation.Parameters.Add(
			new OpenApiParameter
			{
				Name = "$top",
				In = ParameterLocation.Query,
				Description = "Limit the number of results returned",
				Required = false,
				Schema = new OpenApiSchema { Type = "integer", Format = "int32", Minimum = 0, },
			});

		operation.Parameters.Add(
			new OpenApiParameter
			{
				Name = "$skip",
				In = ParameterLocation.Query,
				Description = "Skip the specified number of results",
				Required = false,
				Schema = new OpenApiSchema { Type = "integer", Format = "int32", Minimum = 0, },
			});
	}
}
