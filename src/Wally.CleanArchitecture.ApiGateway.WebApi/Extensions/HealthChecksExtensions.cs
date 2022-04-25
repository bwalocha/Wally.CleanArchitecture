using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Wally.CleanArchitecture.ApiGateway.WebApi.Extensions;

public static class HealthChecksExtensions
{
	public static IHealthChecksBuilder AddVersionHealthCheck(this IHealthChecksBuilder builder)
	{
		builder.AddCheck<VersionHealthCheck>("VER", tags: new[] { "VER", "Version", });

		return builder;
	}

	private class VersionHealthCheck : IHealthCheck
	{
		private readonly string? _version;

		public VersionHealthCheck()
		{
			_version = GetType()
				.Assembly.GetName()
				.Version?.ToString();
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
		{
			return Task.FromResult(HealthCheckResult.Healthy(_version));
		}
	}
}
