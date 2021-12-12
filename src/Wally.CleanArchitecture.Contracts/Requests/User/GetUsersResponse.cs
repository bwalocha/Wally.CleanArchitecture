using System;
using Wally.Lib.DDD.Abstractions.Requests;

namespace Wally.CleanArchitecture.Contracts.Requests.User
{
	[ExcludeFromCodeCoverage]
	public class GetUsersRequest : IRequest
	{
		public Guid Id { get; private set; }

		public string? Name { get; private set; }
	}
}
