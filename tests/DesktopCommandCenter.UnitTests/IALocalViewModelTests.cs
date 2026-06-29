using Moq;
using Xunit;
using FluentAssertions;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.UI.ViewModels;

namespace DesktopCommandCenter.UnitTests;

public class IALocalViewModelTests
{
    private readonly Mock<IIAAgentService> _agentServiceMock;
    private readonly Mock<IWhisperTranscriptionService> _whisperServiceMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IWebSearchService> _webSearchServiceMock;

    public IALocalViewModelTests()
    {
        _agentServiceMock = new Mock<IIAAgentService>();
        _whisperServiceMock = new Mock<IWhisperTranscriptionService>();
        _authServiceMock = new Mock<IAuthService>();
        _webSearchServiceMock = new Mock<IWebSearchService>();
    }

    [Fact]
    public void IsChatEmpty_Should_Be_True_ByDefault()
    {
        // Arrange
        var vm = new IALocalViewModel(_agentServiceMock.Object, _whisperServiceMock.Object, _authServiceMock.Object, _webSearchServiceMock.Object);

        // Act & Assert
        vm.IsChatEmpty.Should().BeTrue();
        vm.IsChatNotEmpty.Should().BeFalse();
    }

    [Fact]
    public void IsWebSearchMode_Should_Be_False_ByDefault()
    {
        // Arrange
        var vm = new IALocalViewModel(_agentServiceMock.Object, _whisperServiceMock.Object, _authServiceMock.Object, _webSearchServiceMock.Object);

        // Act & Assert
        vm.IsWebSearchMode.Should().BeFalse();
    }

    [Fact]
    public void ToggleWebSearchModeCommand_Should_Toggle_IsWebSearchMode()
    {
        // Arrange
        var vm = new IALocalViewModel(_agentServiceMock.Object, _whisperServiceMock.Object, _authServiceMock.Object, _webSearchServiceMock.Object);

        // Act
        vm.ToggleWebSearchModeCommand.Execute(null);

        // Assert
        vm.IsWebSearchMode.Should().BeTrue();

        // Act 2
        vm.ToggleWebSearchModeCommand.Execute(null);

        // Assert 2
        vm.IsWebSearchMode.Should().BeFalse();
    }
}
