using CardService.Api.Models.Responses;
using CardService.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CardService.Tests
{
    public class CardActionsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public CardActionsApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["CardService:ExternalCallDelayMs"] = "0"
                    });
                });
            }).CreateClient();
        }

        #region Facts

        [Fact]
        public async Task GetAllowedActions_PrepaidClosed_Returns200_AndExpectedActions()
        {
            //Act
            var response = await _client.GetAsync("/api/cards/User1/Card17/actions");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<AllowedActionsResponse>(JsonOptions);

            Assert.NotNull(body);
            Assert.Equal(
                new[] { ActionType.ACTION3, ActionType.ACTION4, ActionType.ACTION9 },
                body!.Actions);
        }

        [Fact]
        public async Task GetAllowedActions_UnknownCard_Returns404()
        {
            //Act
            var response = await _client.GetAsync("/api/cards/User1/Card999/actions");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Health_Returns200()
        {
            //Act
            var response = await _client.GetAsync("/health");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllowedActions_Sets_CorrelationId_Header()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/cards/User1/Card17/actions");
            request.Headers.Add("X-Correlation-ID", "test-correlation-123");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var values));
            Assert.Equal("test-correlation-123", values.Single());
        }

        #endregion Facts
    }
}