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

    public IALocalViewModelTests()
    {
        _agentServiceMock = new Mock<IIAAgentService>();
        _whisperServiceMock = new Mock<IWhisperTranscriptionService>();
        _authServiceMock = new Mock<IAuthService>();
    }

    [Fact]
    public void IsChatEmpty_Should_Be_True_ByDefault()
    {
        // Arrange
        var vm = new IALocalViewModel(_agentServiceMock.Object, _whisperServiceMock.Object, _authServiceMock.Object);

        // Act & Assert
        vm.IsChatEmpty.Should().BeTrue();
        vm.IsChatNotEmpty.Should().BeFalse();
    }

    [Fact]
    public void IsWebSearchMode_Should_Be_False_ByDefault()
    {
        // Arrange
        var vm = new IALocalViewModel(_agentServiceMock.Object, _whisperServiceMock.Object, _authServiceMock.Object);

        // Act & Assert
        vm.IsWebSearchMode.Should().BeFalse();
    }

    [Fact]
    public void EnableWebSearchModeCommand_Should_Set_IsWebSearchMode_To_True()
    {
        // Arrange
        var vm = new IALocalViewModel(_agentServiceMock.Object, _whisperServiceMock.Object, _authServiceMock.Object);

        // Act
        vm.EnableWebSearchModeCommand.Execute(null);

        // Assert
        vm.IsWebSearchMode.Should().BeTrue();
    }
}
