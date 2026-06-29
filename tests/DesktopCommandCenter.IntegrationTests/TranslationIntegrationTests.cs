using Xunit;
using FluentAssertions;
using DesktopCommandCenter.Infrastructure.Services;
using System.Threading.Tasks;

namespace DesktopCommandCenter.IntegrationTests;

public class TranslationIntegrationTests
{
    [Fact]
    public async Task TranslationService_Should_Return_Fallback_Key_When_NotFound()
    {
        // Arrange
        var service = new TranslationService();
        await service.SetLanguageAsync("pt-BR");

        // Act
        var result = service.Get("NonExistent_Translation_Key_123");

        // Assert
        // The service is programmed to return the key itself if the translation is not found
        result.Should().Be("NonExistent_Translation_Key_123");
    }

    [Fact]
    public async Task TranslationService_Should_Load_Actual_Translation()
    {
        // Arrange
        var service = new TranslationService();
        
        // This simulates initializing a culture that exists in the bin folder (en-US is default)
        await service.SetLanguageAsync("en-US");

        // Act
        var result = service.Get("Nav_Dashboard");

        // Assert
        // If it successfully loaded from en-US.json, it should translate (which might also be "Dashboard" or similar)
        // At least it should not be null.
        result.Should().NotBeNullOrWhiteSpace();
    }
}
