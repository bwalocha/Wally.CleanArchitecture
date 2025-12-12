using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Jobs;

public class TypeConverterJsonConverter<T> : JsonConverter<T>
{
	private static readonly TypeConverter Converter = TypeDescriptor.GetConverter(typeof(T));

	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return default;
		}

		if (reader.TokenType != JsonTokenType.String)
		{
			throw new JsonException($"Cannot convert {typeToConvert.Name}: expected string.");
		}

		var str = reader.GetString();
		return (T?)Converter.ConvertFrom(str!);
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		var str = Converter.ConvertToInvariantString(value);
		writer.WriteStringValue(str);
	}
}
