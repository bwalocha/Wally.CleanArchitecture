using Wally.Lib.DDD.Abstractions.DomainModels;

namespace Wally.CleanArchitecture.Domain.Users
{
	public class User : AggregateRoot
	{
		public string Name { get; private set; }

		// Hide public .ctor
#pragma warning disable CS8618
		private User()
#pragma warning restore CS8618
		{
		}

		public static User Create(string name)
		{
			return new User { Name = name };
		}

		public void Update(string name)
		{
			Name = name;
		}
	}
}
