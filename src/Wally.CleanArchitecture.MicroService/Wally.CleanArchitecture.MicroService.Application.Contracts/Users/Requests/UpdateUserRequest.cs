using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Requests;

[ExcludeFromCodeCoverage]
public class UpdateUserRequest : IRequest
{
	public UpdateUserRequest(string name)
	{
		Name = name;
	}

	public string Name { get; }
}
