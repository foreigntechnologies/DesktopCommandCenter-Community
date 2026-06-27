using System.Threading.Tasks;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.ProFeatures.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace DesktopCommandCenter.UnitTests.ProFeatures
{
    public class FirestoreLicenseServiceTests
    {
        [Fact]
        public async Task GetCurrentPlanAsync_Should_Return_Free_If_User_Not_Authenticated()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(a => a.GetCurrentUserAsync(false)).ReturnsAsync((AuthUser)null);
            
            var service = new FirestoreLicenseService(authServiceMock.Object);

            // Act
            var plan = await service.GetCurrentPlanAsync();

            // Assert
            plan.Should().Be("free");
        }

        [Fact]
        public async Task IsProUserAsync_Should_Return_False_If_Plan_Is_Free()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(a => a.GetCurrentUserAsync(false)).ReturnsAsync((AuthUser)null);
            
            var service = new FirestoreLicenseService(authServiceMock.Object);

            // Act
            var isPro = await service.IsProUserAsync();

            // Assert
            isPro.Should().BeFalse();
        }
    }
}
