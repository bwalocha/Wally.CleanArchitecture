using System;
using Wally.CleanArchitecture.MicroService.Application.Messages;
using Wally.CleanArchitecture.MicroService.Infrastructure.BackgroundServices;
using Wally.CleanArchitecture.MicroService.Infrastructure.DI.Microsoft;
using Wally.CleanArchitecture.MicroService.Infrastructure.Messaging;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;
using Wally.CleanArchitecture.MicroService.Infrastructure.PipelineBehaviours;
using Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ArchitectureTests
{
	[Fact]
	public void Architecture_Domain_ShouldNotReferenceAnyOtherLayer()
	{
		// Arrange
		IArchRule rule =
			Types()
				.That()
				.Are(Configuration.DomainProvider)
				.Should()
				.NotDependOnAny(Configuration.ApplicationProvider)
				.AndShould()
				.NotDependOnAny(Configuration.InfrastructureProvider)
				.AndShould()
				.NotDependOnAny(Configuration.PresentationProvider)
				.Because("Domain Layer types should not reference any layer");
		
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
				.Are(Configuration.ApplicationProvider)
				.Should()
				.NotDependOnAny(Configuration.InfrastructureProvider)
				.AndShould()
				.NotDependOnAny(Configuration.PresentationProvider)
				.Because("Application Layer types should not reference Infrastructure or Presentation layer");
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Architecture_ApplicationMessages_ShouldNotReferenceDomainLayer()
	{
		// Arrange
		IArchRule rule =
			Types().That().ResideInAssembly(typeof(IApplicationMessagesAssemblyMarker).Assembly)
				.As("Application.Messages Layer types")
				.Should()
				.NotDependOnAny(Configuration.DomainProvider);
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact(Skip = "TODO: exclude Mediator types")]
	public void Architecture_InfrastructureExceptBackgroundService_ShouldNotReferenceAnyLayerExceptDomain()
	{
		// Arrange
		IArchRule rule =
			Types()
				.That()
				.Are(Configuration.InfrastructureProvider)
				.And()
				.DoNotResideInAssembly(typeof(IInfrastructureBackgroundServicesAssemblyMarker).Assembly)
				.And()
				.DoNotResideInAssembly(typeof(IInfrastructureMessagingAssemblyMarker).Assembly)
				.And()
				.DoNotResideInAssembly(typeof(IInfrastructurePersistenceAssemblyMarker).Assembly)
				.And()
				.DoNotResideInAssembly(typeof(IInfrastructurePipelineBehavioursAssemblyMarker).Assembly)
				.As("Infrastructure Layer types")
				.Should()
				.NotDependOnAny(Configuration.ApplicationProvider)
				.AndShould()
				.NotDependOnAny(Configuration.PresentationProvider);
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Architecture_Infrastructure_ShouldNotReferenceAnyLayerExceptDomain()
	{
		// Arrange
		IArchRule rule =
			Types()
				.That()
				.Are(Configuration.InfrastructureProvider)
				.And()
				.DoNotResideInAssembly(typeof(IInfrastructureDIMicrosoftAssemblyMarker).Assembly)
				.And()
				.DoNotResideInAssembly(typeof(IInfrastructureBackgroundServicesAssemblyMarker).Assembly)
				.And()
				.DoNotResideInAssembly(typeof(IInfrastructureMessagingAssemblyMarker).Assembly)
				.And()
				.DoNotResideInAssembly(typeof(IInfrastructurePersistenceAssemblyMarker).Assembly)
				.And()
				.DoNotResideInAssembly(typeof(IInfrastructurePipelineBehavioursAssemblyMarker).Assembly)
				.And()
				.DoNotResideInAssembly(typeof(IInfrastructureSchedulerServiceAssemblyMarker).Assembly)
				.Should()
				.NotDependOnAny(Configuration.ApplicationProvider)
				.AndShould()
				.NotDependOnAny(Configuration.PresentationProvider);
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Architecture_Infrastructure_ShouldNotReferenceAnyLayerExceptDomainAndApplication()
	{
		// Arrange
		IArchRule rule =
			Types().That().Are(Configuration.InfrastructureProvider)
				.Should()
				.NotDependOnAny(Configuration.PresentationProvider);
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact(Skip = "TODO")]
	public void Architecture_Presentation_ShouldReferenceAnyLayer()
	{
		// Arrange
		IArchRule rule =
			Types().That().Are(Configuration.PresentationProvider)
				.Should()
				.DependOnAny(Configuration.DomainProvider)
				.AndShould()
				.DependOnAny(Configuration.ApplicationProvider)
				.AndShould()
				.DependOnAny(Configuration.InfrastructureProvider);
		
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
				.Are(Configuration.DomainProvider)
				.Should()
				.ResideInNamespaceMatching(@$"^{Configuration.Namespace.Replace(".", @"\.")}\.Domain(\..+)?$")
				.And()
				.Types()
				.That()
				.Are(Configuration.ApplicationProvider)
				.Should()
				.ResideInNamespaceMatching(@$"^{Configuration.Namespace.Replace(".", @"\.")}\.Application(\..+)?$")
				.And()
				.Types()
				.That()
				.Are(Configuration.InfrastructureProvider)
				.Should()
				.ResideInNamespaceMatching(@$"^{Configuration.Namespace.Replace(".", @"\.")}\.Infrastructure(\..+)?$")
				.And()
				.Types()
				.That()
				.Are(Configuration.PresentationProvider)
				.Should()
				.ResideInNamespaceMatching(@$"^{Configuration.Namespace.Replace(".", @"\.")}\.WebApi(\..+)?$");
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
	
	[Fact]
	public void Architecture_DomainAndApplication_ShouldNotUseDateTimeNow()
	{
		// Arrange
		IArchRule rule =
			Types()
				.That()
				.Are(Configuration.DomainProvider)
				.Or()
				.Are(Configuration.ApplicationProvider)
				.Or()
				.Are(Configuration.InfrastructureProvider)
				.Or()
				.Are(Configuration.PresentationProvider)
				.Should()
				.NotAccessGetter<DateTime>(DateTime.Now)
				.AndShould()
				.NotAccessGetter<DateTime>(DateTime.UtcNow)
				.AndShould()
				.NotAccessGetter<DateTimeOffset>(DateTimeOffset.Now)
				.AndShould()
				.NotAccessGetter<DateTimeOffset>(DateTimeOffset.UtcNow);
		
		// Act
		
		// Assert
		rule.Check(Configuration.Architecture);
	}
}
