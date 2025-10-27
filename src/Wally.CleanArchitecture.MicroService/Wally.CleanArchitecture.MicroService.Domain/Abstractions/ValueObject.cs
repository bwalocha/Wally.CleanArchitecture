using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class ValueObject<TValueObject> : IEquatable<TValueObject>
	where TValueObject : ValueObject<TValueObject>
{
	public virtual bool Equals(TValueObject? other)
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

	protected virtual void Validate()
	{
	}

	protected abstract IEnumerable<object?> GetEqualityComponents();
}

[DebuggerDisplay("{Value}")]
public abstract class ValueObject<TValueObject, TValue> : ValueObject<TValueObject>
	where TValueObject : ValueObject<TValueObject>
{
	protected ValueObject(TValue value)
	{
		Value = value;

		Validate();
	}

	public TValue Value { get; private set; }

	/*public static implicit operator TValue(ValueObject<TValueObject, TValue> value)
	{
		return value.Value;
	}*/

	protected override IEnumerable<object?> GetEqualityComponents()
	{
		yield return Value;
	}
}
