using System;
using System.Diagnostics.CodeAnalysis;

using Wally.CleanArchitecture.MicroService.Application.Contracts.Responses.Users;
using Wally.Lib.DDD.Abstractions.Queries;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Queries;

[ExcludeFromCodeCoverage]
public sealed class GetUserQuery : IQuery<GetUserResponse>
{
	public GetUserQuery(Guid id)
	{
		Id = id;
	}

	public Guid Id { get; }
}
