using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonNet.ContractResolvers;
using Newtonsoft.Json;

namespace Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests.Helpers;

[SuppressMessage("Major Code Smell", "S4005:\"System.Uri\" arguments should be used instead of strings")]
public static class HttpClientExtensions
{
	private const string _jsonMediaType = "application/json";

	private static readonly JsonSerializerSettings _jsonSettings =
		new()
		{
			ContractResolver = new PrivateSetterContractResolver(),
		};

	public static Task<HttpResponseMessage> PutAsync<TPayload>(
		this HttpClient client,
		string url,
		TPayload payload,
		CancellationToken cancellationToken)
	{
		var content = CreateContent(payload);

		return client.PutAsync(url, content, cancellationToken);
	}

	public static Task<HttpResponseMessage> PostAsync<TPayload>(
		this HttpClient client,
		string url,
		TPayload payload,
		CancellationToken cancellationToken)
	{
		var content = CreateContent(payload);

		return client.PostAsync(url, content, cancellationToken);
	}

	public static async Task<TResponse> ReadAsync<TResponse>(this HttpResponseMessage response,
		CancellationToken cancellationToken)
	{
		var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

		using TextReader textReader = new StreamReader(stream);
		await using JsonReader jsonReader = new JsonTextReader(textReader);

		return JsonSerializer.Create(_jsonSettings)
			.Deserialize<TResponse>(jsonReader) !;
	}

	private static StringContent CreateContent<TPayload>(TPayload payload)
	{
		return new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, _jsonMediaType);
	}
}
