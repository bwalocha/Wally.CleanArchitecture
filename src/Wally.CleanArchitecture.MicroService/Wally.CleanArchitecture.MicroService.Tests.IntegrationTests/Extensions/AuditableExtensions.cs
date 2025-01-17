using System;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;

public static class AuditableExtensions
{
	public static TAuditable SetCreatedAt<TAuditable>(this TAuditable auditable, DateTimeOffset createdAt)
		where TAuditable : IAuditable
	{
		return auditable.Set(nameof(IAuditable.CreatedAt), createdAt);
	}

	public static TAuditable SetCreatedById<TAuditable>(this TAuditable auditable, UserId createdByUserId)
		where TAuditable : IAuditable
	{
		return auditable.Set(nameof(IAuditable.CreatedById), createdByUserId);
	}

	private static T Set<T>(this T @object, string propertyName, object value)
	{
		var type = typeof(T);
		
		do
		{
			var propertyInfo = type.GetProperty(propertyName);
			if (propertyInfo?.CanWrite == true)
			{
				propertyInfo.SetValue(@object, value);
				return @object;
			}

			type = type.BaseType;
		}
		while (type != null);

		throw new ArgumentException(nameof(@object));
	}
}
