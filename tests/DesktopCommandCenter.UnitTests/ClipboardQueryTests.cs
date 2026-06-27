using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Features.Clipboard.Queries;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DesktopCommandCenter.UnitTests.Features.Clipboard
{
    public class ClipboardQueryTests
    {
        [Fact]
        public async Task GetClipboardItems_Should_Return_All_Items()
        {
            // Arrange
            var repositoryMock = new Mock<IClipboardRepository>();
            var expectedItems = new List<ClipboardItem>
            {
                new ClipboardItem { Content = "Item 1", Type = "Text" },
                new ClipboardItem { Content = "Item 2", Type = "Text" }
            };
            repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedItems);

            var handler = new GetClipboardItemsQueryHandler(repositoryMock.Object);
            var query = new GetClipboardItemsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedItems);
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
