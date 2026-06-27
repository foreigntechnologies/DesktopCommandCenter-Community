using System.Threading.Tasks;
using DesktopCommandCenter.ProFeatures.Services;
using FluentAssertions;
using Xunit;

namespace DesktopCommandCenter.UnitTests.ProFeatures
{
    public class FirebaseAuthServiceTests
    {
        [Fact]
        public void Logout_Should_Set_IsAuthenticated_To_False()
        {
            // Arrange
            var authService = new FirebaseAuthService();

            // Act
            authService.Logout();

            // Assert
            authService.IsAuthenticated.Should().BeFalse();
            authService.CurrentUserUid.Should().BeNull();
        }
    }
}
