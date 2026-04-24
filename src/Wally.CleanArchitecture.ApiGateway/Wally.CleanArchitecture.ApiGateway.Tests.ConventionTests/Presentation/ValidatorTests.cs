using System.Linq;
using FluentValidation;
using Wally.CleanArchitecture.ApiGateway.Tests.ConventionTests.Extensions;

namespace Wally.CleanArchitecture.ApiGateway.Tests.ConventionTests.Presentation;

public class ValidatorTests
{
	[Fact(Skip = "There is no Validator class")]
	public void Presentation_Validator_ShouldBeInternal()
	{
		// Arrange
		IArchRule rule = Classes()
			.That()
			.Are(Configuration.PresentationProvider)
			.And()
			.AreAssignableTo(typeof(AbstractValidator<>))
			.Should()
			.BeInternal()
			.Because("Presentation Validators should be internal.");

		// Act

		// Assert
		rule.Check(Configuration.Architecture);
	}

	[Fact]
	public void Presentation_Validator_ShouldHaveNamingConvention()
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
