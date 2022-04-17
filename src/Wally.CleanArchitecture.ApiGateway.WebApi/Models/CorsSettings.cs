using System.Collections.Generic;

namespace Wally.CleanArchitecture.ApiGateway.WebApi.Models;

public class CorsSettings
{
	public List<string> Origins { get; } = new();
}
