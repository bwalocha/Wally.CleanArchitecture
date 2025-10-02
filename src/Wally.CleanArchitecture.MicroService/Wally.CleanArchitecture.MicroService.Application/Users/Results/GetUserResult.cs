using System;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Results;

[ExcludeFromCodeCoverage]
public class GetUserResult : IResult
{
	public Guid Id { get; private set; } = Guid.Empty;

	public string Name { get; private set; } = null!;
}
