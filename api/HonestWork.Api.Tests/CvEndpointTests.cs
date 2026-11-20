using Microsoft.AspNetCore.Mvc.Testing;

namespace HonestWork.Api.Tests;

public class CvEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Serves_the_pdf_for_viewing_in_the_browser()
    {
        var response = await client.GetAsync("/api/cv");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("inline", response.Content.Headers.ContentDisposition?.DispositionType);
        Assert.Equal("CV-en.pdf", response.Content.Headers.ContentDisposition?.FileName);
    }
}
