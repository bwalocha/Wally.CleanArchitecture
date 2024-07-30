using System;
using System.ComponentModel;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

// https://github.com/dotnet/efcore/blob/release/8.0/src/EFCore/ValueGeneration/SequentialGuidValueGenerator.cs
// https://andrewlock.net/series/using-strongly-typed-entity-ids-to-avoid-primitive-obsession/
[TypeConverter(typeof(StronglyTypedIdConverter))]
public class StronglyTypedId<TStronglyTypedId, TValue> : IStronglyTypedId<TStronglyTypedId, TValue>
	where TStronglyTypedId : StronglyTypedId<TStronglyTypedId, TValue>
	where TValue : struct, IComparable<TValue>, IEquatable<TValue>
{
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
		return other is null ? 1 : Value.CompareTo(other.Value);
	}

	public virtual bool Equals(TStronglyTypedId? other)
	{
		if (other is null)
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return GetType() == other.GetType() && Value.Equals(other.Value);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as TStronglyTypedId);
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override string? ToString()
	{
		return Value.ToString();
	}
}

public interface IStronglyTypedId<TStronglyTypedId, TValue>
	: IStronglyTypedId<TValue>, IEquatable<TStronglyTypedId>, IComparable<TStronglyTypedId>
	where TValue : struct, IEquatable<TValue>
{
}

public interface IStronglyTypedId<TValue>
	where TValue : struct, IEquatable<TValue>
{
	/// <summary>
	///     Gets the underlying value of the strongly-typed ID.
	/// </summary>
	public TValue Value { get; }
}
