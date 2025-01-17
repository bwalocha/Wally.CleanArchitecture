using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonNet.ContractResolvers;
using Newtonsoft.Json;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;

[SuppressMessage("Major Code Smell", "S4005:\"System.Uri\" arguments should be used instead of strings")]
public static class HttpClientExtensions
{
	private const string JsonMediaType = "application/json";

	private static readonly JsonSerializerSettings JsonSettings =
		new()
		{
			ContractResolver = new PrivateSetterContractResolver(),
		};

	public static Task<HttpResponseMessage> PutAsync<TPayload>(
		this HttpClient client,
		[StringSyntax(StringSyntaxAttribute.Uri)]string requestUri,
		TPayload payload,
		CancellationToken cancellationToken)
	{
		return PutAsync(client, CreateUri(requestUri), payload, cancellationToken);
	}
	
	public static Task<HttpResponseMessage> PutAsync<TPayload>(
		this HttpClient client,
		Uri? requestUri,
		TPayload payload,
		CancellationToken cancellationToken)
	{
		var content = CreateContent(payload);

		return client.PutAsync(requestUri, content, cancellationToken);
	}

	public static Task<HttpResponseMessage> PostAsync<TPayload>(
		this HttpClient client,
		[StringSyntax(StringSyntaxAttribute.Uri)]string? requestUri,
		TPayload payload,
		CancellationToken cancellationToken)
	{
		return PostAsync(client, CreateUri(requestUri), payload, cancellationToken);
	}
	
	public static Task<HttpResponseMessage> PostAsync<TPayload>(
		this HttpClient client,
		Uri? requestUri,
		TPayload payload,
		CancellationToken cancellationToken)
	{
		var content = CreateContent(payload);

		return client.PostAsync(requestUri, content, cancellationToken);
	}

	public static async Task<TResponse> ReadAsync<TResponse>(this HttpResponseMessage response,
		CancellationToken cancellationToken)
	{
		var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

		using TextReader textReader = new StreamReader(stream);
		using JsonReader jsonReader = new JsonTextReader(textReader);

		return JsonSerializer.Create(JsonSettings)
			.Deserialize<TResponse>(jsonReader) !;
	}
	
	private static Uri? CreateUri(string? uri) =>
		string.IsNullOrEmpty(uri) ? null : new Uri(uri, UriKind.RelativeOrAbsolute);

	private static StringContent CreateContent<TPayload>(TPayload payload)
	{
		return new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, JsonMediaType);
	}
}
