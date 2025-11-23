using System;
using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Contracts.Users.Responses;

/// <summary>
///     Get Users Response
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class GetUsersResponse : IResponse
{
	/// <summary>
	///     Gets th User Id
	/// </summary>
	public required Guid Id { get; init; }

	/// <summary>
	///     Gets the Name of the User
	/// </summary>
	public required string Name { get; init; }
}
