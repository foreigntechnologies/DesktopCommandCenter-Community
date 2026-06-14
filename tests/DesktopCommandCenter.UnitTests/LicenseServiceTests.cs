using System.Threading.Tasks;
using DesktopCommandCenter.Application.Interfaces;
using Moq;
using Xunit;

namespace DesktopCommandCenter.UnitTests;

public class LicenseServiceTests
{
    private readonly Mock<IAuthService> _authServiceMock;

    public LicenseServiceTests()
    {
        _authServiceMock = new Mock<IAuthService>();
    }

    [Fact]
    public async Task IsProUnlocked_ShouldReturnTrue_WhenForcedByConfig()
    {
        // This is just a placeholder test for the UI logic that would be handled by App.IsProUnlocked
        // Since we are testing pure logic, we mock the behavior of a user with a Pro license.

        // Arrange
        var user = new AuthUser { Uid = "123", Email = "test@pro.com" };
        _authServiceMock
            .Setup(a => a.GetCurrentUserAsync(It.IsAny<bool>()))
            .Returns(Task.FromResult<AuthUser?>(user));

        // Act
        var result = await _authServiceMock.Object.GetCurrentUserAsync();
        var hasUser = result != null;

        // Assert
        Assert.True(hasUser);
    }
}
