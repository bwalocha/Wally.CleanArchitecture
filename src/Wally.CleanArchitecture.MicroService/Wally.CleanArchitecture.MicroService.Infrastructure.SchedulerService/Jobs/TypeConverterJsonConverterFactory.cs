using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.SchedulerService.Jobs;

public class TypeConverterJsonConverterFactory : JsonConverterFactory
{
	public override bool CanConvert(Type typeToConvert)
	{
		var converter = TypeDescriptor.GetConverter(typeToConvert);
		return converter.CanConvertFrom(typeof(string));
	}

	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		var converterType = typeof(TypeConverterJsonConverter<>).MakeGenericType(typeToConvert);
		return (JsonConverter)Activator.CreateInstance(converterType) !;
	}
}
