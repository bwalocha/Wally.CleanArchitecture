using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Helpers;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests;

public class ValidatorTests
{
	[Fact]
	public void Application_Validator_ShouldHaveNamingConvention()
	{
		var assemblies = Configuration.Assemblies.GetAllAssemblies();

		using (new AssertionScope())
		{
			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly.GetTypes()
							.ThatImplement<IValidator>())
				{
					var genericInterface = type.GetGenericInterface(typeof(IValidator<>));
					var genericArgument = genericInterface?.GenericTypeArguments.SingleOrDefault();

					type.Name.Should()
						.Be(
							$"{genericArgument?.Name}Validator",
							"every Validator '{0}' should have naming convention",
							type);
				}
			}
		}
	}
}
