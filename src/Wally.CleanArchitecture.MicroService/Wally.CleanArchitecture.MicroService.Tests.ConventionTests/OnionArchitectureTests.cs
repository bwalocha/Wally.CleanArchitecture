using FluentAssertions;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class OnionArchitectureTests
{
	[Fact]
	public void Domain_ShouldNotUseReferenceToApplication()
	{
		var domainAssemblies = Configuration.Assemblies.Domain;
		var applicationTypes = Configuration.Assemblies.Application.GetAllExportedTypes();

		using (new AssertionScope())
		{
			domainAssemblies.Should()
				.SatisfyRespectively(
					a =>
					{
						foreach (var type in applicationTypes)
						{
							a.Should()
								.NotReference(type.Assembly);
						}
					});
		}
	}

	[Fact]
	public void Domain_ShouldNotUseReferenceToInfrastructure()
	{
		var domainAssemblies = Configuration.Assemblies.Domain;
		var infrastructureTypes = Configuration.Assemblies.Infrastructure.GetAllExportedTypes();

		using (new AssertionScope())
		{
			domainAssemblies.Should()
				.SatisfyRespectively(
					a =>
					{
						foreach (var type in infrastructureTypes)
						{
							a.Should()
								.NotReference(type.Assembly);
						}
					});
		}
	}

	[Fact]
	public void Domain_ShouldNotUseReferenceToPresentation()
	{
		var domainAssemblies = Configuration.Assemblies.Domain;
		var infrastructureTypes = Configuration.Assemblies.Presentation.GetAllExportedTypes();

		using (new AssertionScope())
		{
			domainAssemblies.Should()
				.SatisfyRespectively(
					a =>
					{
						foreach (var type in infrastructureTypes)
						{
							a.Should()
								.NotReference(type.Assembly);
						}
					});
		}
	}

	[Fact]
	public void OnionArchitecture_AllNamespaces_ShouldBeConsistent()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllExportedTypes();

		using (new AssertionScope())
		{
			types.Should()
				.BeUnderNamespace(Configuration.Namespace);
		}
	}
}
