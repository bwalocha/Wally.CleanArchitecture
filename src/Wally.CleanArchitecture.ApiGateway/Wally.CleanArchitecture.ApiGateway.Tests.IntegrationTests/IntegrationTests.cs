using Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests.Helpers;
using Wally.CleanArchitecture.ApiGateway.WebApi;

namespace Wally.CleanArchitecture.ApiGateway.Tests.IntegrationTests;

[CollectionDefinition(nameof(IntegrationTests))]
public class IntegrationTests : ICollectionFixture<ApiWebApplicationFactory<Startup>>
{
}
