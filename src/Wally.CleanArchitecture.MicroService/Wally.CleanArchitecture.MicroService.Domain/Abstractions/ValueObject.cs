using System;
using System.Collections.Generic;
using System.Linq;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class ValueObject<TValueObject> : IEquatable<TValueObject>
	where TValueObject : ValueObject<TValueObject>
{
	protected ValueObject()
	{
	}

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
			GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
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

/*
public abstract class ValueObject<TValueObject, TValue> : ValueObject<TValue>
	where TValueObject : ValueObject<TValueObject, TValue>
{
	protected ValueObject()
	{
	}
	
	protected ValueObject(TValue value)
		: base(value)
	{
	}
	
	public static implicit operator TValue(ValueObject<TValueObject, TValue> value)
	{
		return value.Value;
	}
}

public abstract class ValueObject<TValue>
{
	protected ValueObject()
	{
		Value = default!;
	}
	
	protected ValueObject(TValue value)
	{
		Value = value;
		
		ExecuteValidation();
	}
	
	public TValue Value { get; private set; }
	
	protected abstract void Validate();
	
	private void ExecuteValidation()
	{
		Validate();
	}
}
*/
