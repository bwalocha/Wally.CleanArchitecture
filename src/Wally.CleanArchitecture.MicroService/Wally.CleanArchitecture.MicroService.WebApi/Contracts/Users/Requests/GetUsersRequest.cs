using System;
using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Requests;

[ExcludeFromCodeCoverage]
public sealed class GetUsersRequest : IRequest
{
	public required Guid? Id { get; init; }
	
	public required string? Name { get; init; }
}
