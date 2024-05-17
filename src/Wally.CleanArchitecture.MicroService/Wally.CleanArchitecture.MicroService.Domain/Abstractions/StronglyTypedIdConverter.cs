using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

internal sealed class StronglyTypedIdConverter : TypeConverter
{
	private static readonly ConcurrentDictionary<Type, TypeConverter> ActualConverters = new();
	
	private readonly TypeConverter _innerConverter;
	
	public StronglyTypedIdConverter(Type stronglyTypedIdType)
	{
		_innerConverter = ActualConverters.GetOrAdd(stronglyTypedIdType, CreateActualConverter);
	}
	
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		return _innerConverter.CanConvertFrom(context, sourceType);
	}
	
	public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
	{
		return _innerConverter.CanConvertTo(context, destinationType);
	}
	
	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		return _innerConverter.ConvertFrom(context, culture, value);
	}
	
	public override object? ConvertTo(
		ITypeDescriptorContext? context,
		CultureInfo? culture,
		object? value,
		Type destinationType)
	{
		return _innerConverter.ConvertTo(context, culture, value, destinationType);
	}
	
	private static TypeConverter CreateActualConverter(Type stronglyTypedIdType)
	{
		if (!stronglyTypedIdType.IsStronglyTypedId())
		{
			throw new InvalidOperationException($"The type '{stronglyTypedIdType}' is not a strongly-typed ID.");
		}
		
		var idValueType = stronglyTypedIdType.GetStronglyTypedIdValueType() !;
		var actualConverterType = typeof(StronglyTypedIdConverter<,>).MakeGenericType(stronglyTypedIdType, idValueType);
		return (TypeConverter)Activator.CreateInstance(actualConverterType) !;
	}
}

internal sealed class StronglyTypedIdConverter<TStronglyTypedId, TValue> : TypeConverter
	where TStronglyTypedId : StronglyTypedId<TStronglyTypedId, TValue>
	where TValue : struct, IComparable<TValue>, IEquatable<TValue>
{
#pragma warning disable S2743
	
	// ReSharper disable once StaticMemberInGenericType
	private static TypeConverter IdValueConverter { get; } = GetIdValueConverter();
#pragma warning restore S2743
	
	private static TypeConverter GetIdValueConverter()
	{
		var converter = TypeDescriptor.GetConverter(typeof(TValue));
		
		if (!converter.CanConvertFrom(typeof(string)))
		{
			throw new InvalidOperationException(
				$"The type '{typeof(TValue)}' doesn't have a converter that can convert from string.");
		}
		
		return converter;
	}
	
	/// <inheritdoc />
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		return sourceType == typeof(string) || sourceType == typeof(TValue) || base.CanConvertFrom(context, sourceType);
	}
	
	/// <inheritdoc />
	public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
	{
		return destinationType == typeof(string) || destinationType == typeof(TValue) ||
			base.CanConvertTo(context, destinationType);
	}
	
	/// <inheritdoc />
	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		if (value is string str)
		{
			value = IdValueConverter.ConvertFrom(str) ?? throw GetConvertFromException(value);
		}
		
		if (value is TValue idValue)
		{
			var instance = Activator.CreateInstance(typeof(TStronglyTypedId), idValue) !;
			
			return instance;
		}
		
		return base.ConvertFrom(context, culture, value);
	}
	
	/// <inheritdoc />
	public override object? ConvertTo(
		ITypeDescriptorContext? context,
		CultureInfo? culture,
		object? value,
		Type destinationType)
	{
		if (value == null)
		{
			return null;
		}
		
		var stronglyTypedId = (StronglyTypedId<TStronglyTypedId, TValue>)value;
		
		var idValue = stronglyTypedId.Value;
		if (destinationType == typeof(string))
		{
			return idValue.ToString();
		}
		
		if (destinationType == typeof(TValue))
		{
			return idValue;
		}
		
		return base.ConvertTo(context, culture, value, destinationType);
	}
}
