using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Contracts.Requests.Users;
using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.MapperProfiles;
using Wally.CleanArchitecture.MicroService.Messaging.Consumers;
using Wally.CleanArchitecture.MicroService.Persistence;
using Wally.CleanArchitecture.MicroService.Persistence.SqlServer;
using Wally.CleanArchitecture.MicroService.PipelineBehaviours;
using Wally.CleanArchitecture.MicroService.WebApi;

namespace Wally.CleanArchitecture.MicroService.ConventionTests;

public static class Configuration
{
	public static Types Types => new() { AppSettings = new[] { typeof(AppSettings), }, };

	public static Assemblies Assemblies =>
		new()
		{
			Application =
				new[]
				{
					typeof(CreateUserCommand).Assembly, typeof(CreateUserRequest).Assembly,
					typeof(UserProfile).Assembly,
				},
			Domain = new[] { typeof(User).Assembly, },
			Infrastructure = new[]
			{
				typeof(ServiceCollectionExtensions).Assembly, typeof(UserCreatedConsumer).Assembly,
				typeof(ApplicationDbContext).Assembly, typeof(Helper).Assembly, typeof(LogBehavior<,>).Assembly,
			},
			Presentation = new[] { typeof(Startup).Assembly, },
		};
}
