using System;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Responses;

/*
[ExcludeFromCodeCoverage]
public class GetUsersResponse : IResponse // TODO: consider to hide public ctor
{
	public Guid Id { get; private set; }

	public string? Name { get; private set; }
}
*/
/*
[ExcludeFromCodeCoverage]
public readonly record struct GetUsersResponse(Guid Id, string? Name) : IResponse; // ctor(Guid, string?), 2 public setters
*/

[ExcludeFromCodeCoverage]
public class GetUsersResponse : IResponse // TODO: consider to hide public ctor + convention tests
{
	/*private GetUsersResponse()
	{
	}*/

	public required Guid Id { get; init; }

	public required string? Name { get; init; }
}
