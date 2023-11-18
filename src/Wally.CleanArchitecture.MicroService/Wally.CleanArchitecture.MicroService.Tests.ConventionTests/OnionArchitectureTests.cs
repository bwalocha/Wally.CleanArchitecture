using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class OnionArchitectureTests
{
	[Fact]
	public void Domain_IsNotReferencedByApplication()
	{
		var domainAssemblies = Configuration.Assemblies.Domain;
		var applicationTypes = Configuration.Assemblies.Application.SelectMany(a => a.GetTypes());

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
	public void Domain_IsNotReferencedByInfrastructure()
	{
		var domainAssemblies = Configuration.Assemblies.Domain;
		var infrastructureTypes = Configuration.Assemblies.Infrastructure.SelectMany(a => a.GetTypes());

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
	public void Domain_IsNotReferencedByPersistence()
	{
		var domainAssemblies = Configuration.Assemblies.Domain;
		var persistenceTypes = Configuration.Assemblies.Infrastructure.SelectMany(a => a.GetTypes());

		using (new AssertionScope())
		{
			domainAssemblies.Should()
				.SatisfyRespectively(
					a =>
					{
						foreach (var type in persistenceTypes)
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
		var applicationNamespace = "Wally.CleanArchitecture.MicroService";
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllExportedTypes();

		using (new AssertionScope())
		{
			types.Should()
				.BeUnderNamespace(applicationNamespace);
		}
	}
}
