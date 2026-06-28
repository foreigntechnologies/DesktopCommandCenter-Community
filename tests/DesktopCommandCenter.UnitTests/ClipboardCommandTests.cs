using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Features.Clipboard.Commands;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace DesktopCommandCenter.UnitTests.Features.Clipboard
{
    public class ClipboardCommandTests
    {
        [Fact]
        public async Task CreateClipboardItem_Should_Add_Item_To_Repository()
        {
            // Arrange
            var repositoryMock = new Mock<IClipboardRepository>();
            var itemToReturn = new ClipboardItem { Content = "Test Content", Type = "Text" };
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<ClipboardItem>())).ReturnsAsync(itemToReturn);

            var handler = new CreateClipboardItemCommandHandler(repositoryMock.Object);
            var command = new CreateClipboardItemCommand("Test Content", "Text", null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Content.Should().Be("Test Content");
            result.Type.Should().Be("Text");
            repositoryMock.Verify(r => r.AddAsync(It.Is<ClipboardItem>(i => i.Content == "Test Content")), Times.Once);
        }
        
        [Fact]
        public async Task ClearClipboard_Should_Call_Repository_Clear()
        {
            // Arrange
            var repositoryMock = new Mock<IClipboardRepository>();
            repositoryMock.Setup(r => r.ClearAsync()).Returns(Task.CompletedTask);

            var handler = new ClearClipboardCommandHandler(repositoryMock.Object);
            var command = new ClearClipboardCommand();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            repositoryMock.Verify(r => r.ClearAsync(), Times.Once);
        }
    }
}
