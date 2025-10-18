using System;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Results;

[ExcludeFromCodeCoverage]
public class GetUserResult : IResult
{
	public required Guid Id { get; init; }

	public required string Name { get; init; }
}
