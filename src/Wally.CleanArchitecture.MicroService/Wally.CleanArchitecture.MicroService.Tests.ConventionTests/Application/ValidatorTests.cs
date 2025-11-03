using System.Linq;
using FluentValidation;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Application;

public class ValidatorTests
{
	[Fact]
	public void Application_Validator_ShouldHaveNamingConvention()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();
		var types = assemblies.SelectMany(a => a.GetTypes())
			.Where(a => a.ImplementsInterface(typeof(IValidator)));

		types.ShouldSatisfyAllConditions(() => 
		{
			foreach (var type in types)
			{
				var genericInterface = type.GetGenericInterface(typeof(IValidator<>));
				var genericArgument = genericInterface?.GenericTypeArguments.SingleOrDefault();

				type.Name.ShouldBe(
						$"{genericArgument?.Name}Validator",
						$"every Validator '{type}' should have naming convention");
			}
		});
	}
}
