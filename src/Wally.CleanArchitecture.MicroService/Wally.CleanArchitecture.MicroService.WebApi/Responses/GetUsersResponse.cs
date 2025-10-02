using System;
using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Responses;

/*
[ExcludeFromCodeCoverage]
public class GetUsersResult : IResult // TODO: consider to hide public ctor
{
	public Guid Id { get; private set; }

	public string? Name { get; private set; }
}
*/
/*
[ExcludeFromCodeCoverage]
public readonly record struct GetUsersResult(Guid Id, string? Name) : IResult; // ctor(Guid, string?), 2 public setters
*/

[ExcludeFromCodeCoverage]
public class GetUsersResponse : IResponse // TODO: consider to hide public ctor + convention tests
{
	/*private GetUsersResult()
	{
	}*/

	public required Guid Id { get; init; }

	public required string? Name { get; init; }
}
