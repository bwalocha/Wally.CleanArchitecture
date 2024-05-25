using System.Collections.Generic;
using Yarp.ReverseProxy.Configuration;

// using Microsoft.Extensions.Primitives;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

public class ReverseProxySettings // : IProxyConfig
{
	public List<RouteConfig> Routes { get; } = new();
	
	public Dictionary<string, ClusterConfig> Clusters { get; } = new();
	
	/*IReadOnlyList<RouteConfig> IProxyConfig.Routes => Routes.AsReadOnly();
	
	IReadOnlyList<ClusterConfig> IProxyConfig.Clusters => Clusters.AsReadOnly();
	
	public IChangeToken ChangeToken { get; init; } = null!;*/
}
