using System;
using System.Diagnostics.CodeAnalysis;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.Contracts.Responses.Users
{
	[ExcludeFromCodeCoverage]
	public class GetUserResponse : IResponse
	{
		public Guid Id { get; private set; }

		public string Name { get; private set; }
	}
}
