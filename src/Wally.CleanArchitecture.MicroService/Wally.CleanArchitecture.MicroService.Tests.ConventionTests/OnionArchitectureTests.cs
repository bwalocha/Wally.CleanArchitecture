// using System.Linq;
// using MassTransit.Internals;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class OnionArchitectureTests
{
	/*[Fact]
	public void Domain_ShouldNotUseReferenceToApplication()
	{
		var domainAssemblies = Configuration.Assemblies.Domain;
		var types = Configuration.Assemblies.Application.GetAllExportedTypes();

		types.ShouldSatisfyAllConditions(() => {
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
		});
	}*/

	/*[Fact]
	public void Domain_ShouldNotUseReferenceToInfrastructure()
	{
		var domainAssemblies = Configuration.Assemblies.Domain;
		var infrastructureTypes = Configuration.Assemblies.Infrastructure.GetAllExportedTypes();

		using var scope = new AssertionScope(new AssertionStrategy());
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
	}*/

	/*[Fact]
	public void Domain_ShouldNotUseReferenceToPresentation()
	{
		var domainAssemblies = Configuration.Assemblies.Domain;
		var types = Configuration.Assemblies.Presentation.GetAllTypes();

		types.ShouldSatisfyAllConditions(() =>
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
			});
	}*/

	[Fact]
	public void OnionArchitecture_AllNamespaces_ShouldBeConsistent()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllExportedTypes();

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Namespace.ShouldStartWith(Configuration.Namespace, Case.Sensitive,
					$"Type '{type}' namespace should start with '{Configuration.Namespace}'");
			}
		});
	}
}
