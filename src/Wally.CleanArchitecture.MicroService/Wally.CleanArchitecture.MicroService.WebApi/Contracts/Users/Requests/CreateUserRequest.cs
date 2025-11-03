using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Requests;

/// <summary>
/// Create User Request
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class CreateUserRequest : IRequest
{
	/// <summary>
	/// Gets the Name of the User
	/// </summary>
	public required string Name { get; init; }
}
