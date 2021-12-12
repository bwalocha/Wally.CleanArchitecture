using System.Diagnostics.CodeAnalysis;
using MediatR;
using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.Application.Users.Commands
{
	[ExcludeFromCodeCoverage]
	public class UpdateUserCommand : ICommand, IRequest
	{
		public UpdateUserCommand(Guid id, string name)
		{
			Id = id;
			Name = name;
		}

		public Guid Id { get; }

		public string Name { get; }
	}
}
