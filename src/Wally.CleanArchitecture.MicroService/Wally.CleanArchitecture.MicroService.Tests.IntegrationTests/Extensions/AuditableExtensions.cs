using System;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;

public static class AuditableExtensions
{
	public static TAuditable SetCreatedAt<TAuditable>(this TAuditable auditable, DateTimeOffset createdAt)
		where TAuditable : IAuditable
	{
		return auditable.Set(a => a.CreatedAt, createdAt);
	}

	public static TAuditable SetCreatedById<TAuditable>(this TAuditable auditable, UserId createdByUserId)
		where TAuditable : IAuditable
	{
		return auditable.Set(a => a.CreatedById, createdByUserId);
	}
}
