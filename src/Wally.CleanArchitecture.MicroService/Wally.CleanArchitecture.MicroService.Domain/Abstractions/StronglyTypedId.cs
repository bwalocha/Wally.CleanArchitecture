using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

// TODO: https://github.com/dotnet/efcore/blob/release/8.0/src/EFCore/ValueGeneration/SequentialGuidValueGenerator.cs

[TypeConverter(typeof(StronglyTypedIdConverter))]
public abstract class StronglyTypedId<TStronglyTypedId, TValue> : IStronglyTypedId<TStronglyTypedId, TValue>
	where TStronglyTypedId : StronglyTypedId<TStronglyTypedId, TValue> where TValue : notnull, IComparable
{
	/// <summary>
	///     To ensure hashcode uniqueness, a carefully selected random number multiplier
	///     is used within the calculation.
	/// </summary>
	/// <remarks>
	///     See http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/
	/// </remarks>
	private const int HashMultiplier = 37;

	/*static StronglyTypedId()
	{
		Type valueType = typeof(TValue);
		bool isIdentifierType = valueType.IsNumeric() || valueType == typeof(string) || valueType == typeof(Guid);

		// Guard.Against.False(isIdentifierType, nameof(Value), "The value of a strongly-typed ID must be a numeric, string or Guid type.");
	}*/

	/// <summary>
	///     Initializes a new instance of the <see cref="StronglyTypedId{TStronglyTypedId,TValue}" /> type.
	/// </summary>
	/// <param name="value"></param>
	protected StronglyTypedId(TValue value)
	{
		Value = value;
	}

	public TValue Value { get; }

	public bool Equals(TStronglyTypedId other)
	{
		return Equals(other as object);
	}

	public int CompareTo(TStronglyTypedId? other)
	{
		return (Value, other.Value) switch
		{
			(null, null) => 0,
			(null, _) => -1,
			(_, null) => 1,
			(_, _) => Value.CompareTo(other.Value),
		};
	}

	public sealed override bool Equals(object? obj)
	{
		if (obj is null)
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		var other = obj as StronglyTypedId<TStronglyTypedId, TValue>;
		return other != null && GetType() == other.GetType() && GetEqualityComponents()
			.SequenceEqual(other.GetEqualityComponents());
	}

	/// <summary>
	///     Gets all components of the value object that are used for equality. <br />
	///     The default implementation get all properties via reflection. One
	///     can at any time override this behavior with a manual or custom implementation.
	/// </summary>
	/// <returns>The components to use for equality.</returns>
	private IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
	}

	/// <inheritdoc />
	public sealed override int GetHashCode()
	{
		unchecked
		{
			// It is possible for two objects to return the same hash code based on
			// identically valued properties, even if they are of different types,
			// so we include the value object type in the hash calculation.
			var hashCode = GetType()
				.GetHashCode();

			foreach (var component in GetEqualityComponents())
			{
				if (component != null)
				{
					hashCode = (hashCode * HashMultiplier) ^ component.GetHashCode();
				}
			}

			return hashCode;
		}
	}

	/// <summary>
	///     Checks if the given IDs are equal.
	/// </summary>
	public static bool operator ==(
		StronglyTypedId<TStronglyTypedId, TValue>? left,
		StronglyTypedId<TStronglyTypedId, TValue>? right)
	{
		if (left is null)
		{
			return right is null;
		}

		return left.Equals(right);
	}

	/// <summary>
	///     Checks if the given IDs are not equal.
	/// </summary>
	public static bool operator !=(
		StronglyTypedId<TStronglyTypedId, TValue>? left,
		StronglyTypedId<TStronglyTypedId, TValue>? right)
	{
		return !(left == right);
	}

	/*/// <summary>
	///     Converts a value implicitly to an instance of TStronglyTypedId.
	/// </summary>
	/// <param name="value">The value</param>
	public static explicit operator StronglyTypedId<TStronglyTypedId, TValue>(TValue value)
	{
		object instance = Activator.CreateInstance(typeof(TStronglyTypedId), new object[] { value });
		return (TStronglyTypedId)instance;
	}

	public static implicit operator TValue(StronglyTypedId<TStronglyTypedId, TValue> id)
	{
		return id.Value;
	}*/

	/// <summary>
	///     Converts a value implicitly to an instance of TStronglyTypedId.
	/// </summary>
	/// <param name="value">The value.</param>
	public static explicit operator StronglyTypedId<TStronglyTypedId, TValue>(TValue value)
	{
		var instance = Activator.CreateInstance(typeof(TStronglyTypedId), value) !;
		return (TStronglyTypedId)instance;
	}

	/*public static explicit operator TValue(StronglyTypedId<TStronglyTypedId, TValue> id)
	{
		return id.Value;
	}*/

	public override string? ToString()
	{
		return Value.ToString();
	}
}

public interface IStronglyTypedId
	<TStronglyTypedId, out TKey> : IComparable<TStronglyTypedId>, IEquatable<TStronglyTypedId>
	where TKey : notnull, IComparable
{
	/// <summary>
	///     Gets the underlying value of the strongly-typed ID.
	/// </summary>
	public TKey Value { get; }
}

public static class TypeExtensions
{
	private static readonly Dictionary<Type, bool> NumericTypes = new()
	{
		{
			typeof(sbyte), true
		},
		{
			typeof(byte), true
		},
		{
			typeof(short), true
		},
		{
			typeof(ushort), true
		},
		{
			typeof(int), true
		},
		{
			typeof(uint), true
		},
		{
			typeof(long), true
		},
		{
			typeof(ulong), true
		},
		{
			typeof(decimal), true
		},
		{
			typeof(float), true
		},
		{
			typeof(double), true
		}
	};

	/// <summary>
	///     Determines if the given type is numeric.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>
	///     <c>true</c> if the specified type is numeric; otherwise, <c>false</c>.
	/// </returns>
	public static bool IsNumeric(this Type type)
	{
		type = type.UnwrapNullableType();

		return NumericTypes.ContainsKey(type) && NumericTypes[type];
	}

	/// <summary>
	///     Gets the type without nullable if the type is a <see cref="Nullable{T}" />.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns></returns>
	public static Type UnwrapNullableType(this Type type)
	{
		return Nullable.GetUnderlyingType(type) ?? type;
	}
}