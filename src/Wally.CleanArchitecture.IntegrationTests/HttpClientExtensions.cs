using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Wally.CleanArchitecture.IntegrationTests;

public static class HttpClientExtensions
{
	private const string _jsonMediaType = "application/json";

	public static Task<HttpResponseMessage> PutAsync(
		this HttpClient client,
		string url,
		object payload,
		CancellationToken cancellationToken)
	{
		var content = CreateContent(payload);

		return client.PutAsync(url, content, cancellationToken);
	}

	private static StringContent CreateContent<T>(T item)
	{
		return new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, _jsonMediaType);
	}
}
