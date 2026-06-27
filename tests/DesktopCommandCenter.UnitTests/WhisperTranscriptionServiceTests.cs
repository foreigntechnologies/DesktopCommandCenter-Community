using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.ProFeatures.Services;
using FluentAssertions;
using Xunit;

namespace DesktopCommandCenter.UnitTests.ProFeatures
{
    public class WhisperTranscriptionServiceTests
    {
        [Fact]
        public async Task TranscribeFileAsync_Should_Return_Mocked_String()
        {
            // Arrange
            var service = new WhisperTranscriptionService();

            // Act
            var result = await service.TranscribeFileAsync("test.mp3", CancellationToken.None);

            // Assert
            result.Should().Contain("test.mp3");
            result.Should().Contain("transcrito com sucesso");
        }

        [Fact]
        public async Task StopRecordingAndTranscribeAsync_Should_Return_Mocked_Text()
        {
            // Arrange
            var service = new WhisperTranscriptionService();
            service.StartRecording();

            // Act
            var result = await service.StopRecordingAndTranscribeAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("Integração Whisper em andamento");
        }
    }
}
