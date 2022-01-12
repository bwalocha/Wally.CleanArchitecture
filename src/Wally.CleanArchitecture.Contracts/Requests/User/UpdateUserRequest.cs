using Wally.Lib.DDD.Abstractions.Requests;

namespace Wally.CleanArchitecture.Contracts.Requests.User;

[ExcludeFromCodeCoverage]
public class UpdateUserRequest : IRequest
{
	public UpdateUserRequest(string name)
	{
		Name = name;
	}

	public string Name { get; }
}
