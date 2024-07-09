using System;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Responses;

[ExcludeFromCodeCoverage]
public class GetUsersResponse : IResponse
{
	public Guid Id { get; private set; }

	public string? Name { get; private set; }
}
