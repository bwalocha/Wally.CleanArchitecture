using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Requests;

[ExcludeFromCodeCoverage]
public class CreateUserRequest : IRequest
{
	public CreateUserRequest(string name)
	{
		Name = name;
	}

	public string Name { get; }
}
