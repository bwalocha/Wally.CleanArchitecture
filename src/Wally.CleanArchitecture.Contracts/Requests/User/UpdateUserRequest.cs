using System.Diagnostics.CodeAnalysis;
using Wally.Lib.DDD.Abstractions.Requests;

namespace Wally.CleanArchitecture.Contracts.Requests.User
{
	[ExcludeFromCodeCoverage]
	public class UpdateUserRequest : IRequest
	{
		public string Name { get; }

		public UpdateUserRequest(string name)
		{
			Name = name;
		}
	}
}
