using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Types;

namespace Wally.CleanArchitecture.MicroService.Tests.ConventionTests.Extensions;

public static class TypeSelectorAssertionsExtensions
{
	public static void HaveOnlyPrivateParameterlessConstructor(this TypeSelectorAssertions typeSelectorAssertions)
	{
		const string because = "entity '{0}' should have private parameterless constructor only.";
		
		foreach (var type in typeSelectorAssertions.Subject)
		{
			var constructors = type.GetConstructors();
			foreach (var constructor in constructors)
			{
				constructor.Should()
					.HaveAccessModifier(CSharpAccessModifier.Private, because, type);
				constructor.GetParameters()
					.Should()
					.BeEmpty(because, type);
			}
			
			type.Should()
				.HaveDefaultConstructor(because, type);
		}
	}
}
