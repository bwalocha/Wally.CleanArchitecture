using FluentAssertions;

using Xunit;

namespace Wally.CleanArchitecture.ApiGateway.UnitTests;

public class SampleTests
{
	[Fact]
	public void SampleTest()
	{
		true.Should()
			.BeTrue();
	}
}
