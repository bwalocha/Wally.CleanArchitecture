using Wally.Lib.DDD.Abstractions.Requests;

namespace Wally.CleanArchitecture.MicroService.Contracts.Requests.Users;

[ExcludeFromCodeCoverage]
public class UpdateUserRequest : IRequest
{
	public UpdateUserRequest(string name)
	{
		Name = name;
	}

	public string Name { get; }
}
