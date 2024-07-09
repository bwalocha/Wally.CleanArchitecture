using System.Diagnostics.CodeAnalysis;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Responses;
using Wally.CleanArchitecture.MicroService.Domain.Users;

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
