using Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;
using Wally.CleanArchitecture.MicroService.WebApi;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests;

[CollectionDefinition(nameof(IntegrationTests))]
public class IntegrationTests : ICollectionFixture<ApiWebApplicationFactory<Startup>>
{
}
