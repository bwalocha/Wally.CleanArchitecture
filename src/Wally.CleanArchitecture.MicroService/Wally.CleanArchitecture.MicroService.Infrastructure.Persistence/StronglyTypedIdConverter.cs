using System;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

public sealed class StronglyTypedIdConverter<TStronglyTypedId, TValue> : ValueConverter<TStronglyTypedId, TValue>
	where TStronglyTypedId : StronglyTypedId<TStronglyTypedId, TValue> where TValue : notnull, IComparable
{
	/// <summary>
	///     Initializes a new instance of the <see cref="StronglyTypedIdConverter{TStronglyTypedId,TValue}" /> type.
	/// </summary>
	public StronglyTypedIdConverter()
		: base(valueObject => Serialize(valueObject), value => Deserialize(value))
	{
	}

	private static TValue Serialize(TStronglyTypedId valueObject)
	{
		var value = valueObject.Value;
		return value;
	}

	private static TStronglyTypedId Deserialize(TValue value)
	{
		if (value is null)
		{
			return null;
		}

		var instance = Activator.CreateInstance(typeof(TStronglyTypedId), new object[] { value, });
		return (TStronglyTypedId)instance;
	}
}
