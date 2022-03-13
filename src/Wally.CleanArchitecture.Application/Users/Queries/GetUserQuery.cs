﻿using System;
using System.Diagnostics.CodeAnalysis;

using MediatR;

using Wally.CleanArchitecture.Contracts.Responses.Users;
using Wally.Lib.DDD.Abstractions.Queries;

namespace Wally.CleanArchitecture.Application.Users.Queries;

[ExcludeFromCodeCoverage]
public class GetUserQuery : IQuery<GetUserResponse>, IRequest<GetUserResponse>
{
	public GetUserQuery(Guid id)
	{
		Id = id;
	}

	public Guid Id { get; }
}
