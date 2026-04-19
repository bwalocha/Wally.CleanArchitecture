namespace Wally.CleanArchitecture.ApiGateway.WebApi.Contracts;

[ExcludeFromCodeCoverage]
public sealed class VersionResponse : IResponse
{
	public string Version { get; }

	public VersionResponse(string version)
	{
		Version = version;
	}
}
