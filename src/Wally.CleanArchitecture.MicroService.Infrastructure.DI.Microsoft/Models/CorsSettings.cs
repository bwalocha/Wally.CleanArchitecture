using System.Collections.Generic;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

public class CorsSettings
{
	public List<string> Origins { get; } = new();
}
