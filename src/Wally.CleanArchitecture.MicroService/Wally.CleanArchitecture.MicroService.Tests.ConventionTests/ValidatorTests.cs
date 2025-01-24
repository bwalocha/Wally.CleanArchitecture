using System.Linq;
using FluentValidation;
using Shouldly;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

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
