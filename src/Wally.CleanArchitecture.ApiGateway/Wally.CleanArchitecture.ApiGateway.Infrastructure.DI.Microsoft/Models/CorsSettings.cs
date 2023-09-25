using System;
using System.Collections.Generic;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

public class CorsSettings
{
	public List<Uri> Origins { get; } = new();
}
