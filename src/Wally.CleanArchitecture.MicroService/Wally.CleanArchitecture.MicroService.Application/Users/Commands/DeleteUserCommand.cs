using System;
using System.Diagnostics.CodeAnalysis;

using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

[ExcludeFromCodeCoverage]
public sealed class DeleteUserCommand : ICommand
{
	public DeleteUserCommand(Guid id)
	{
		Id = id;
	}

	public Guid Id { get; }
}
