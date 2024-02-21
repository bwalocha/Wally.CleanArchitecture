using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

// https://github.com/dotnet/efcore/blob/release/8.0/src/EFCore/ValueGeneration/SequentialGuidValueGenerator.cs
// https://andrewlock.net/series/using-strongly-typed-entity-ids-to-avoid-primitive-obsession/
[TypeConverter(typeof(StronglyTypedIdConverter))]
public class StronglyTypedId<TStronglyTypedId, TValue> : IStronglyTypedId<TStronglyTypedId, TValue>
	where TStronglyTypedId : StronglyTypedId<TStronglyTypedId, TValue>
	where TValue : notnull, IComparable
{
	/// <summary>
	///     To ensure hashcode uniqueness, a carefully selected random number multiplier
	///     is used within the calculation.
	/// </summary>
	/// <remarks>
	///     See http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/
	/// </remarks>
	private const int _hashMultiplier = 37;

	/// <summary>
	///     Initializes a new instance of the <see cref="StronglyTypedId{TStronglyTypedId, TValue}" /> class.
	/// </summary>
	/// <param name="value">The Value.</param>
	protected StronglyTypedId(TValue value)
	{
		Value = value;
	}

	public TValue Value { get; }

	public int CompareTo(TStronglyTypedId? other)
	{
		if (other is null)
		{
			return 1;
		}

		return (Value, other.Value) switch
		{
			(null, null) => 0,
			(null, _) => -1,
			(_, null) => 1,
			_ => Value.CompareTo(other.Value),
		};
	}

	public bool Equals(TStronglyTypedId? other)
	{
		if (other is null)
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return GetType() == other.GetType() && GetEqualityComponents()
			.SequenceEqual(other.GetEqualityComponents());
	}

	public bool Equals(TStronglyTypedId? x, TStronglyTypedId? y)
	{
		if (x is null)
		{
			return y is null;
		}

		return x.Equals(y);
	}

	public int GetHashCode(TStronglyTypedId obj)
	{
		return obj.GetHashCode();
	}

	public sealed override bool Equals(object? obj)
	{
		return Equals(obj as TStronglyTypedId);
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
				hashCode = (hashCode * _hashMultiplier) ^ component.GetHashCode();
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

	/// <summary>
	///     Converts a value implicitly to an instance of TStronglyTypedId.
	/// </summary>
	/// <param name="value">The value.</param>
	public static explicit operator StronglyTypedId<TStronglyTypedId, TValue>(TValue value)
	{
		var instance = Activator.CreateInstance(typeof(TStronglyTypedId), value) !;
		return (TStronglyTypedId)instance;
	}

	public override string? ToString()
	{
		return Value.ToString();
	}
}

public interface IStronglyTypedId
	<TStronglyTypedId, out TKey> : IStronglyTypedId<TKey>, IComparable<TStronglyTypedId>, IEquatable<TStronglyTypedId>,
	IEqualityComparer<TStronglyTypedId>
	where TKey : notnull, IComparable
{
}

public interface IStronglyTypedId<out TKey>
	where TKey : notnull, IComparable
{
	/// <summary>
	///     Gets the underlying value of the strongly-typed ID.
	/// </summary>
	public TKey Value { get; }
}

public static class TypeExtensions
{
	private static readonly Dictionary<Type, bool> _numericTypes = new()
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
		},
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

		return _numericTypes.ContainsKey(type) && _numericTypes[type];
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
