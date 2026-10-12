using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

public sealed class DealerFlowFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseFile = Path.Combine(Path.GetTempPath(), $"dealerflow-test-{Guid.NewGuid():N}.db");
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Dealers"] = $"Data Source={_databaseFile}"
            });
        });
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && File.Exists(_databaseFile)) File.Delete(_databaseFile);
    }
}

public class DealerApiTests : IClassFixture<DealerFlowFactory>
{
    private readonly HttpClient _client;

    public DealerApiTests(DealerFlowFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var result = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task InvalidDealer_ReturnsBadRequest()
    {
        var result = await _client.PostAsJsonAsync("/api/dealers", new { name = "", region = "London" });
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task MissingDealerForListing_ReturnsNotFound()
    {
        var result = await _client.PostAsJsonAsync("/api/listings",
            new { dealerId = Guid.NewGuid(), title = "Test listing", price = 150000 });
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DealerAndListing_CanBeCreatedAndPublished()
    {
        var dealerResponse = await _client.PostAsJsonAsync("/api/dealers",
            new { name = "Test dealer", region = "London" });
        Assert.Equal(HttpStatusCode.Created, dealerResponse.StatusCode);
        var dealerJson = await dealerResponse.Content.ReadFromJsonAsync<JsonElement>();
        var dealerId = dealerJson.GetProperty("id").GetGuid();

        var listingResponse = await _client.PostAsJsonAsync("/api/listings",
            new { dealerId, title = "Test home", price = 185000 });
        Assert.Equal(HttpStatusCode.Created, listingResponse.StatusCode);
        var listingJson = await listingResponse.Content.ReadFromJsonAsync<JsonElement>();
        var listingId = listingJson.GetProperty("id").GetGuid();

        var statusResponse = await _client.PatchAsJsonAsync($"/api/listings/{listingId}/status",
            new { status = "published" });
        Assert.Equal(HttpStatusCode.OK, statusResponse.StatusCode);
    }
}
