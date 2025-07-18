using System;
using System.Linq;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.BackgroundServices;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ArchitectureTests
{
	[Fact]
	public void Architecture_Domain_ShouldNotReferenceAnyLayer()
	{
		// Arrange
		IArchRule rule =
			Types().That().ResideInAssembly(Configuration.Assemblies.Domain)
				.As("Domain Layer types")
				.Should()
				.NotDependOnAny(Types().That().DoNotResideInAssembly(Configuration.Assemblies.Domain));
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Architecture_Application_ShouldNotReferenceAnyLayerExceptDomain()
	{
		// Arrange
		IArchRule rule =
			Types()
				.That()
				.ResideInAssembly(Configuration.Assemblies.Application)
				.As("Application Layer types")
				.Should()
				.NotDependOnAny(Types()
					.That()
					.DoNotResideInAssembly(Configuration.Assemblies.Domain)
					.And()
					.DoNotResideInAssembly(Configuration.Assemblies.Application));
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact(Skip = "TODO: fix, extract Abstractions")]
	public void Architecture_InfrastructureExceptBackgroundService_ShouldNotReferenceAnyLayerExceptDomain()
	{
		// Arrange
		var allowedTypes = new[]
		{
			typeof(IRequestContext), typeof(Application.Contracts.Abstractions.IRequest),
			typeof(Application.Contracts.Abstractions.IResponse),
			typeof(ICommandAuthorizationHandler<,>), typeof(ICommand<>), typeof(IDomainEventHandler<>),
			typeof(IRepository<,>),
			typeof(IReadOnlyRepository<,>)
		};
		IArchRule rule =
			Types().That().Are(Configuration.Assemblies.Infrastructure
					.Where(a => a != typeof(IInfrastructureBackgroundServicesAssemblyMarker).Assembly)
					.GetAllTypes())
				.As("Infrastructure Layer types")
				.Should()
				.NotDependOnAny(Configuration.Assemblies.GetAllAssemblies()
					.Where(a => !Configuration.Assemblies.Domain.Contains(a))
					.Where(a => !Configuration.Assemblies.Infrastructure.Contains(a))
					// .Where(a => !Configuration.Assemblies.Application.Where(b => b == typeof(IApplicationContractsAssemblyMarker).Assembly).Contains(a))
					.GetAllTypes().Where(a => !allowedTypes.Contains(a)));
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact(Skip = "TODO: Infrastructure (BackgroundService) references Application")]
	public void Architecture_Infrastructure_ShouldNotReferenceAnyLayerExceptDomain()
	{
		// Arrange
		IArchRule rule =
			Types().That().Are(Configuration.Assemblies.Infrastructure.GetAllTypes())
				.As("Infrastructure Layer types")
				.Should()
				.NotDependOnAny(Configuration.Assemblies.GetAllAssemblies().Where(a => !Configuration.Assemblies.Domain.Contains(a)).Where(a => !Configuration.Assemblies.Infrastructure.Contains(a)).GetAllTypes());
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact(Skip = "TODO: fix")]
	public void Architecture_Infrastructure_ShouldNotReferenceAnyLayerExceptDomainAndApplication()
	{
		// TODO: Consider to remove dependency to Application
		// - pros: all interfaces in Domain - Clean Arhitecture 
		// - cons: too big Domain, interfaces like ICommandDispatcher, IMediator, INotificationBus in Domain
		
		// Arrange
		IArchRule rule =
			Types().That().Are(Configuration.Assemblies.Infrastructure.GetAllTypes())
				.As("Infrastructure Layer types")
				.Should()
				.NotDependOnAny(Configuration.Assemblies.GetAllAssemblies().Where(a => !Configuration.Assemblies.Domain.Contains(a)).Where(a => !Configuration.Assemblies.Application.Contains(a)).Where(a => !Configuration.Assemblies.Infrastructure.Contains(a)).GetAllTypes());
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact(Skip = "TODO")]
	public void Architecture_Presentation_ShouldReferenceAnyLayer()
	{
		// Arrange
		IArchRule rule =
			Types().That().Are(Configuration.Assemblies.Presentation.GetAllTypes())
				.As("Presentation Layer types")
				.Should()
				.DependOnAny(Configuration.Assemblies.GetAllAssemblies().GetAllTypes());
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Architecture_AllNamespaces_ShouldBeConsistent()
	{
		// Arrange
		IArchRule rule =
			Types()
				.That()
				.ResideInAssembly(Configuration.Assemblies.Domain)
				.As("Domain Layer types")
				.Should()
				.ResideInNamespaceMatching($"^{Configuration.Namespace.Replace(".", "\\.")}\\.Domain(\\..+)?$")
				.And()
				.Types()
				.That()
				.ResideInAssembly(Configuration.Assemblies.Application)
				.As("Application Layer types")
				.Should()
				.ResideInNamespaceMatching($"^{Configuration.Namespace.Replace(".", "\\.")}\\.Application(\\..+)?$")
				.And()
				.Types()
				.That()
				.ResideInAssembly(Configuration.Assemblies.Infrastructure)
				.As("Infrastructure Layer types")
				.Should()
				.ResideInNamespaceMatching($"^{Configuration.Namespace.Replace(".", "\\.")}\\.Infrastructure(\\..+)?$")
				.And()
				.Types()
				.That()
				.ResideInAssembly(Configuration.Assemblies.Presentation)
				.As("Presentation Layer types")
				.Should()
				.ResideInNamespaceMatching($"^{Configuration.Namespace.Replace(".", "\\.")}\\.WebApi(\\..+)?$");
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Architecture_DomainAndApplication_ShouldNotUseDateTimeNow()
	{
		// Arrange
		var types = Types()
			.That()
			.ResideInAssembly([..Configuration.Assemblies.Domain, ..Configuration.Assemblies.Application]);
		IArchRule rule = types
			.Should()
			.NotAccessGetter<DateTime>(DateTime.Now)
			.And(types
				.Should()
				.NotAccessGetter<DateTime>(DateTime.UtcNow)
				.And(types.Should()
					.NotAccessGetter<DateTimeOffset>(DateTimeOffset.Now)
					.And(types.Should()
						.NotAccessGetter<DateTimeOffset>(DateTimeOffset.UtcNow))));
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
}
