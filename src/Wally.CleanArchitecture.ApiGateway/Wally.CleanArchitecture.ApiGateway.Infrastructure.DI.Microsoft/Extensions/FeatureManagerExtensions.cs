using Microsoft.FeatureManagement;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Extensions;

public static class FeatureManagerExtensions
{
	/// <summary>Checks whether a given feature is enabled.</summary>
	/// <param name="featureManager">The Feature Manager.</param>
	/// <param name="feature">The name of the feature to check.</param>
	/// <returns>True if the feature is enabled, otherwise false.</returns>
	public static bool IsEnabled(this IFeatureManager featureManager, string feature)
	{
		return featureManager
			.IsEnabledAsync(feature)
			.Result;
	}
}
