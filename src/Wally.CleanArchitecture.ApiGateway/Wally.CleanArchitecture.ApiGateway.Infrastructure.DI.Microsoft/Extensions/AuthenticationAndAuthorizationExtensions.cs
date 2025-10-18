using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Models;

namespace Wally.CleanArchitecture.ApiGateway.Infrastructure.DI.Microsoft.Extensions;

public static class AuthenticationAndAuthorizationExtensions
{
	private const string ReverseProxy = nameof(ReverseProxy);
	private const string AuthenticatedUsersPolicyName = "AuthenticatedUsers";

	public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, AppSettings settings)
	{
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.Authority = settings.Authentication.Authority;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = settings.Authentication.Authority,
					ValidateAudience = true,
					ValidAudience = settings.Authentication.ClientId,
					ValidateLifetime = true,
				};
			});
		services.AddAuthorization(options =>
		{
			options.AddPolicy(AuthenticatedUsersPolicyName, policy =>
			{
				policy.RequireAuthenticatedUser();
			});
		});
		
		return services;
	}

	public static IApplicationBuilder UseAuthenticationAndAuthorization(this IApplicationBuilder app)
	{
		app.UseAuthentication();
		app.UseAuthorization();
		
		return app;
	}
}
