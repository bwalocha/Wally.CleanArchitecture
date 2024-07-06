using System;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;

public static class AuditableExtensions
{
	public static TAuditable SetCreatedAt<TAuditable>(this TAuditable auditable, DateTimeOffset createdAt)
		where TAuditable : IAuditable
	{
		var type = auditable.GetType();

		do
		{
			var propertyInfo = type.GetProperty(nameof(IAuditable.CreatedAt));
			if (propertyInfo?.CanWrite == true)
			{
				propertyInfo.SetValue(auditable, createdAt);
				return auditable;
			}

			type = type.BaseType;
		}
		while (type != null);

		throw new ArgumentException(nameof(auditable));
	}

	public static TAuditable SetCreatedById<TAuditable>(this TAuditable auditable, UserId createdByUserId)
		where TAuditable : IAuditable
	{
		var type = auditable.GetType();

		do
		{
			var propertyInfo = type.GetProperty(nameof(IAuditable.CreatedById));
			if (propertyInfo?.CanWrite == true)
			{
				propertyInfo.SetValue(auditable, createdByUserId);
				return auditable;
			}

			type = type.BaseType;
		}
		while (type != null);

		throw new ArgumentException(nameof(auditable));
	}
}
