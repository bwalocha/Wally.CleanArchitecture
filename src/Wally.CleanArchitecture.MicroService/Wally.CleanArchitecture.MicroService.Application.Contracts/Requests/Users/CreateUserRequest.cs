using Wally.Lib.DDD.Abstractions.Requests;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts.Requests.Users;

[ExcludeFromCodeCoverage]
public class CreateUserRequest : IRequest
{
	public CreateUserRequest(string name)
	{
		Name = name;
	}

	public string Name { get; }
}
