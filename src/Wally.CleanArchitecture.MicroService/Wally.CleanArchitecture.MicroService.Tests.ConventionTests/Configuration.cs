using Wally.CleanArchitecture.MicroService.Application;
using Wally.CleanArchitecture.MicroService.Application.Contracts;
using Wally.CleanArchitecture.MicroService.Application.MapperProfiles;
using Wally.CleanArchitecture.MicroService.Application.Messages;
using Wally.CleanArchitecture.MicroService.Domain;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.Messaging;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.MySql;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.PostgreSQL;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer;
using Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Wally.CleanArchitecture.MicroService.WebApi;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public static class Configuration
{
	public static Types Types => new() { AppSettings = new[] { typeof(AppSettings), }, };

	public static Assemblies Assemblies =>
		new()
		{
			Application =
				new[]
				{
					typeof(IApplicationAssemblyMarker).Assembly,
					typeof(IApplicationContractsAssemblyMarker).Assembly,
					typeof(IApplicationMapperProfilesAssemblyMarker).Assembly,
					typeof(IApplicationMessagesAssemblyMarker).Assembly,
				},
			Domain = new[] { typeof(IDomainAssemblyMarker).Assembly, },
			Infrastructure = new[]
			{
				typeof(IInfrastructureDIMicrosoftAssemblyMarker).Assembly,
				typeof(IInfrastructureMessagingAssemblyMarker).Assembly,
				typeof(IInfrastructurePersistenceAssemblyMarker).Assembly,
				typeof(IInfrastructureSqlServerAssemblyMarker).Assembly,
				typeof(IInfrastructurePostgreSqlAssemblyMarker).Assembly,
				typeof(IInfrastructureMySqlAssemblyMarker).Assembly,
				typeof(IInfrastructurePipelineBehavioursAssemblyMarker).Assembly,
			},
			Presentation = new[] { typeof(IPresentationAssemblyMarker).Assembly, },
		};
}
