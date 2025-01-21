using System;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Requests;

[ExcludeFromCodeCoverage]
public class GetUsersRequest : IRequest
{
	public Guid? Id { get; private set; } = null;

	public string? Name { get; private set; } = null;
}
