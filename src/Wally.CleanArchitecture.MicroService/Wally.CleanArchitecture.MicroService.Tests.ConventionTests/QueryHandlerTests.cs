using System.Linq;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class QueryHandlerTests
{
	[Fact]
	public void Application_AllClassesEndsWithQueryHandler_ShouldImplementIQueryHandler()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.Name.EndsWith("QueryHandler"));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.ImplementsGenericInterface(typeof(IQueryHandler<,>))
					.ShouldBeTrue($"Query Handler '{type}' should implement 'IQueryHandler' interface");
			}
		});
	}

	[Fact]
	public void Application_AllClassesImplementedIQueryHandler_ShouldHaveNameSuffix()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.GetAllTypes()
			.Where(a => a.IsClass)
			.Where(a => !a.IsAbstract)
			.Where(a => a.ImplementsGenericInterface(typeof(IQueryHandler<,>)));

		types.ShouldSatisfyAllConditions(() =>
		{
			foreach (var type in types)
			{
				type.Name
					.ShouldEndWith("QueryHandler", Case.Sensitive,
						$"Query Handler '{type}' name should end with 'QueryHandler'");
			}
		});
	}
}
