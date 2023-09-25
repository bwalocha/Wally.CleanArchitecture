using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Swagger;

internal class SchemasFilter : IDocumentFilter
{
	public void Apply(OpenApiDocument document, DocumentFilterContext context)
	{
		var schemas = document.Components.Schemas;
		foreach (var schema in schemas)
		{
			if (schema.Key.EndsWith("Request") && schema.Key != nameof(HttpRequest))
			{
				continue;
			}

			if (schema.Key.EndsWith("Response") && schema.Key != nameof(HttpResponse))
			{
				continue;
			}

			document.Components.Schemas.Remove(schema);
		}
	}
}
