using System;
using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Responses;

[ExcludeFromCodeCoverage]
public class GetUsersResponse : IResponse
{
	public required Guid Id { get; init; }

	public required string Name { get; init; }
}
