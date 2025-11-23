// using System.Linq;
// using MassTransit.Internals;

using System.Linq;
using Microsoft.Extensions.DependencyInjection;
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

	[Fact(Skip =
		"TODO: Type 'Coverlet.Core.Instrumentation.Tracker.Wally.CleanArchitecture.MicroService.Domain_c075db3f-d24f-49b4-941c-7655310ece31' namespace should start with 'Wally.CleanArchitecture.MicroService'")]
	public void OnionArchitecture_AllNamespaces_ShouldBeConsistent()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Namespace?.StartsWith(typeof(MediatorDependencyInjectionExtensions).Namespace!) == false)
			.Where(a => a.Namespace?.StartsWith(typeof(Mediator.Mediator).Namespace!) == false);

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
