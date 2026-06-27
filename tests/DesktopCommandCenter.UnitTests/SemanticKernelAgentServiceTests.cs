using DesktopCommandCenter.ProFeatures.Services;
using FluentAssertions;
using Xunit;

namespace DesktopCommandCenter.UnitTests.ProFeatures
{
    public class SemanticKernelAgentServiceTests
    {
        [Fact]
        public void ClearHistory_Should_Not_Throw_Exception()
        {
            // Arrange
            var service = new SemanticKernelAgentService();

            // Act & Assert
            // ClearHistory resets the internal ChatHistory. We just ensure it doesn't throw.
            var exception = Record.Exception(() => service.ClearHistory());
            exception.Should().BeNull();
        }
    }
}
