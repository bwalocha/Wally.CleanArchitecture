using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

public class VerifyChecksTests
{
	[Fact(Skip = "File location different for .gitignore")]
	public Task Run()
	{
		return VerifyChecks.Run();
	}
}
