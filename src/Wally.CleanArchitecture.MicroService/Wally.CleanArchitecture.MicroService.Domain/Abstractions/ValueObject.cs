using System;
using System.Collections.Generic;
using System.Linq;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class ValueObject<TValueObject> : IEquatable<TValueObject>
	where TValueObject : ValueObject<TValueObject>
{
	public bool Equals(TValueObject? other)
	{
		if (other is null)
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return GetType() == other.GetType() &&
			GetEqualityComponents()
				.SequenceEqual(other.GetEqualityComponents());
	}

	protected virtual void Validate()
	{
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as TValueObject);
	}

	public override int GetHashCode()
	{
		return GetEqualityComponents()
			.Select(a => a?.GetHashCode() ?? 0)
			.Aggregate((a, b) => a ^ b);
	}

	protected abstract IEnumerable<object?> GetEqualityComponents();
}

public abstract class ValueObject<TValueObject, TValue> : ValueObject<TValueObject>
	where TValueObject : ValueObject<TValueObject>
{
	protected ValueObject(TValue value)
	{
		Value = value;
	}

	public TValue Value { get; }

	public static implicit operator TValue(ValueObject<TValueObject, TValue> value)
	{
		return value.Value;
	}
}
