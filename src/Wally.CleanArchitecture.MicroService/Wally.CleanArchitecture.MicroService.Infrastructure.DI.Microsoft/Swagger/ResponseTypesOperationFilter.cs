using System.Linq;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Swagger;

internal class ResponseTypesOperationFilter : IOperationFilter
{
	/// <summary>
	///     Applies the filter to the specified operation using the given context.
	/// </summary>
	/// <param name="operation">The operation to apply the filter to.</param>
	/// <param name="context">The current operation filter context.</param>
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1752#issue-663991077
		foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
		{
			// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/b7cf75e7905050305b115dd96640ddd6e74c7ac9/src/Swashbuckle.AspNetCore.SwaggerGen/SwaggerGenerator/SwaggerGenerator.cs#L383-L387
			var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
			var response = operation.Responses[responseKey];

			foreach (var contentType in response.Content.Keys)
			{
				if (responseType.ApiResponseFormats.All(x => x.MediaType != contentType))
				{
					response.Content.Remove(contentType);
				}
			}
		}
	}
}
