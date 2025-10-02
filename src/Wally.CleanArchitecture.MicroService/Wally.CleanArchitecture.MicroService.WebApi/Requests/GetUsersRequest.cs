// using System;
using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Requests;

[ExcludeFromCodeCoverage]
public class GetUsersRequest : Application.Users.Requests.GetUsersRequest, IRequest
{
	// public Guid? Id { get; private set; } = null;
	//
	// public string? Name { get; private set; } = null;
}
