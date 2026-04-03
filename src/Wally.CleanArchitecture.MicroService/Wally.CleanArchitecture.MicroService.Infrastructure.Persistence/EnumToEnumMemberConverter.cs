using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence;

public sealed class EnumToEnumMemberConverter<TEnum>
	: ValueConverter<TEnum?, string?>
	where TEnum : Enum
{
	/// <summary>
	///     Initializes a new instance of the <see cref="EnumToEnumMemberConverter{TEnum}" /> class.
	/// </summary>
	public EnumToEnumMemberConverter()
		: base(enumValue => Serialize(enumValue), stringValue => Deserialize(stringValue))
	{
	}

	private static string? Serialize(TEnum? enumValue)
	{
		if (enumValue == null)
		{
			return null;
		}
		
		var member = typeof(TEnum).GetMember(enumValue.ToString()).FirstOrDefault();
		var attr = member?.GetCustomAttribute<EnumMemberAttribute>();

		return attr?.Value;
	}

	private static TEnum? Deserialize(string? value)
	{
		foreach (var field in typeof(TEnum).GetFields())
		{
			var attr = field.GetCustomAttribute<EnumMemberAttribute>();
			if (attr?.Value == value)
			{
				return (TEnum?)field.GetValue(null);
			}
		}

		return default;
	}
}
