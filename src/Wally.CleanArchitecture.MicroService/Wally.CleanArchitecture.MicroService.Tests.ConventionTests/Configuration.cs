using System.CodeDom.Compiler;
using System.Linq;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Wally.CleanArchitecture.MicroService.Application;
using Wally.CleanArchitecture.MicroService.Application.Contracts;
using Wally.CleanArchitecture.MicroService.Application.DI.Microsoft;
using Wally.CleanArchitecture.MicroService.Application.MapperProfiles;
using Wally.CleanArchitecture.MicroService.Application.Messages;
using Wally.CleanArchitecture.MicroService.Domain;
using Wally.CleanArchitecture.MicroService.Infrastructure.BackgroundServices;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft.Models;
using Wally.CleanArchitecture.MicroService.Infrastructure.Messaging;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.MySql;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.PostgreSQL;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SQLite;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.SqlServer;
using Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Wally.CleanArchitecture.MicroService.WebApi;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public static class Configuration
{
	public const string Namespace = "Wally.CleanArchitecture.MicroService";

	internal static readonly Architecture Architecture = new ArchLoader()
		.LoadAssemblies(Assemblies.GetAllAssemblies().ToArray())
		.LoadAssemblies(typeof(System.DateTime).Assembly)
		.Build();

	internal static readonly IObjectProvider<IType> DomainProvider =
		Types()
			.That()
			.ResideInAssembly(Assemblies.Domain)
			.And()
			.DoNotHaveAnyAttributes(typeof(GeneratedCodeAttribute))
			.As("Domain Layer");

	internal static readonly IObjectProvider<IType> ApplicationProvider =
		Types()
			.That()
			.ResideInAssembly(Assemblies.Application)
			.And()
			.DoNotHaveAnyAttributes(typeof(GeneratedCodeAttribute))
			.As("Application Layer");

	internal static readonly IObjectProvider<IType> InfrastructureProvider =
		Types()
			.That()
			.ResideInAssembly(Assemblies.Infrastructure)
			.And()
			.DoNotHaveAnyAttributes(typeof(GeneratedCodeAttribute))
			.And()
			.DoNotResideInNamespaceMatching(@$"^.+\.{nameof(Mediator.Internals)}$")
			.As("Infrastructure Layer");

	internal static readonly IObjectProvider<IType> PresentationProvider =
		Types()
			.That()
			.ResideInAssembly(Assemblies.Presentation)
			.And()
			.DoNotHaveAnyAttributes(typeof(GeneratedCodeAttribute))
			.And()
			.DoNotResideInNamespace(nameof(Mediator))
			.And()
			.DoNotResideInNamespaceMatching(@$"^.+\.{nameof(Mediator.Internals)}$")
			.As("Presentation Layer");

	public static Types OtherTypes
		=> new()
		{
			AppSettings =
			[
				typeof(AppSettings),
			],
		};

	public static Assemblies Assemblies
		=> new()
		{
			Domain =
			[
				typeof(IDomainAssemblyMarker).Assembly,
			],
			Application =
			[
				typeof(IApplicationAssemblyMarker).Assembly,
				typeof(IApplicationContractsAssemblyMarker).Assembly,
				typeof(IApplicationDIMicrosoftAssemblyMarker).Assembly,
				typeof(IApplicationMapperProfilesAssemblyMarker).Assembly,
				typeof(IApplicationMessagesAssemblyMarker).Assembly,
			],
			Infrastructure =
			[
				typeof(IInfrastructureBackgroundServicesAssemblyMarker).Assembly,
				typeof(IInfrastructureDIMicrosoftAssemblyMarker).Assembly,
				typeof(IInfrastructureMessagingAssemblyMarker).Assembly,
				typeof(IInfrastructurePersistenceAssemblyMarker).Assembly,
				typeof(IInfrastructureMySqlAssemblyMarker).Assembly,
				typeof(IInfrastructurePostgreSqlAssemblyMarker).Assembly,
				typeof(IInfrastructureSQLiteAssemblyMarker).Assembly,
				typeof(IInfrastructureSqlServerAssemblyMarker).Assembly,
				typeof(IInfrastructurePipelineBehavioursAssemblyMarker).Assembly,
			],
			Presentation =
			[
				typeof(IPresentationAssemblyMarker).Assembly,
			],
		};
}
