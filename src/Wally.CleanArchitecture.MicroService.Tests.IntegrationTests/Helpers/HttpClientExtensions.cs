using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using JsonNet.ContractResolvers;

using Newtonsoft.Json;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;

public static class HttpClientExtensions
{
	private const string _jsonMediaType = "application/json";

	private static readonly JsonSerializerSettings _jsonSettings =
		new() { ContractResolver = new PrivateSetterContractResolver(), };

	public static Task<HttpResponseMessage> PutAsync(
		this HttpClient client,
		string url,
		object payload,
		CancellationToken cancellationToken)
	{
		var content = CreateContent(payload);

		return client.PutAsync(url, content, cancellationToken);
	}

	public static Task<HttpResponseMessage> PostAsync(
		this HttpClient client,
		string url,
		object payload,
		CancellationToken cancellationToken)
	{
		var content = CreateContent(payload);

		return client.PostAsync(url, content, cancellationToken);
	}

	public static async Task<T> ReadAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken)
	{
		var json = await response.Content.ReadAsStringAsync(
			cancellationToken); // TODO: get Stream instead of loading String
		return JsonConvert.DeserializeObject<T>(json, _jsonSettings) !;
	}

	private static StringContent CreateContent<T>(T item)
	{
		return new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, _jsonMediaType);
	}
}
