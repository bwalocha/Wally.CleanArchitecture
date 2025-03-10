﻿using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

public sealed class StronglyTypedIdConverter<TStronglyTypedId, TValue>
	: ValueConverter<TStronglyTypedId, TValue>
	where TStronglyTypedId : StronglyTypedId<TStronglyTypedId, TValue>
	where TValue : /*struct,*/ IComparable<TValue>, IEquatable<TValue>
{
	/// <summary>
	///     Initializes a new instance of the <see cref="StronglyTypedIdConverter{TStronglyTypedId, TValue}" /> class.
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
		var instance = Activator.CreateInstance(typeof(TStronglyTypedId), value) !;
		return (TStronglyTypedId)instance;
	}
}
