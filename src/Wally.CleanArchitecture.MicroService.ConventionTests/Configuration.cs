using Wally.CleanArchitecture.MicroService.Application;
using Wally.CleanArchitecture.MicroService.Contracts;
using Wally.CleanArchitecture.MicroService.ConventionTests.Helpers;
using Wally.CleanArchitecture.MicroService.Domain;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.MapperProfiles;
using Wally.CleanArchitecture.MicroService.Messages;
using Wally.CleanArchitecture.MicroService.Messaging;
using Wally.CleanArchitecture.MicroService.Persistence;
using Wally.CleanArchitecture.MicroService.Persistence.MySql;
using Wally.CleanArchitecture.MicroService.Persistence.PostgreSQL;
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
				typeof(IInfrastructurePostgreSQLAssemblyMarker).Assembly,
				typeof(IInfrastructureMySqlAssemblyMarker).Assembly,
				typeof(IInfrastructurePipelineBehavioursAssemblyMarker).Assembly,
			},
			Presentation = new[] { typeof(IPresentationAssemblyMarker).Assembly, },
		};
}
