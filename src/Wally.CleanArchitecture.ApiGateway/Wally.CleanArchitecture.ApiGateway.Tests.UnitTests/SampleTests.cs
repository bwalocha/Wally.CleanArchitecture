using FluentAssertions;
using Xunit;

namespace Wally.CleanArchitecture.ApiGateway.Tests.UnitTests;

public class SampleTests
{
	[Fact]
	public void SampleTest()
	{
		true.Should()
			.BeTrue();
	}
}
