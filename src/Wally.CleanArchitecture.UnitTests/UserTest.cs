using FluentAssertions;
using Wally.CleanArchitecture.Domain.Users;
using Xunit;

namespace Wally.CleanArchitecture.UnitTests
{
	public class UserTest
	{
		[Fact]
		public void Create_ForSpecifiedUserName_SetsName()
		{
			var user = User.Create("testUserName");

			user.Name.Should().NotBeNullOrWhiteSpace();
		}
	}
}
