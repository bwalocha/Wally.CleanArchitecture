using System;
using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Responses;

/// <summary>
///     Get User Response
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class GetUserResponse : IResponse
{
	/// <summary>
	///     Gets the User Id
	/// </summary>
	public required Guid Id { get; init; }

	/// <summary>
	///     Gets the Name of the User
	/// </summary>
	public required string Name { get; init; }
}
