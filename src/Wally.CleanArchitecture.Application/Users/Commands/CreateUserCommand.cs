using System;
using System.Diagnostics.CodeAnalysis;

using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.Application.Users.Commands;

[ExcludeFromCodeCoverage]
public class CreateUserCommand : ICommand
{
	public CreateUserCommand(Guid id, string name)
	{
		Id = id;
		Name = name;
	}

	public Guid Id { get; }

	public string Name { get; }
}
