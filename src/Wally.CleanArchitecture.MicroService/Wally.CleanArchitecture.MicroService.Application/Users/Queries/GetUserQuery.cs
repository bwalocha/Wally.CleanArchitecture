using System.Diagnostics.CodeAnalysis;

using Wally.CleanArchitecture.MicroService.Application.Contracts.Responses.Users;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.Lib.DDD.Abstractions.Queries;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Queries;

[ExcludeFromCodeCoverage]
public sealed class GetUserQuery : IQuery<GetUserResponse>
{
	public GetUserQuery(UserId userId)
	{
		UserId = userId;
	}

	public UserId UserId { get; }
}
