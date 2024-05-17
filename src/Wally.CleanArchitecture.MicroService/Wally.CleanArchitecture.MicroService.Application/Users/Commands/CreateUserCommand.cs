using System.Diagnostics.CodeAnalysis;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

[ExcludeFromCodeCoverage]
public sealed class CreateUserCommand : ICommand
{
	public CreateUserCommand(UserId userId, string name)
	{
		UserId = userId;
		Name = name;
	}
	
	public CreateUserCommand(string name)
		: this(new UserId(), name)
	{
	}
	
	public UserId UserId { get; }
	
	public string Name { get; }
}
