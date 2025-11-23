using System;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;

public class OpenTelemetrySettings
{
	public Uri? Endpoint { get; init; }
}
