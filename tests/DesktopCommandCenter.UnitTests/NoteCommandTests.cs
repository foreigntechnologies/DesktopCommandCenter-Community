using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Features.Notes.Commands;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DesktopCommandCenter.UnitTests.Features.Notes
{
    public class NoteCommandTests
    {
        [Fact]
        public async Task CreateNote_Should_Add_Note_To_Repository()
        {
            // Arrange
            var repositoryMock = new Mock<INoteRepository>();
            var expectedNote = new Note { Title = "Test Title", Content = "Test Content", Category = "General" };
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<Note>())).ReturnsAsync(expectedNote);

            var handler = new CreateNoteCommandHandler(repositoryMock.Object);
            var command = new CreateNoteCommand("Test Title", "Test Content", "General");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Test Title");
            result.Content.Should().Be("Test Content");
            repositoryMock.Verify(r => r.AddAsync(It.Is<Note>(n => n.Title == "Test Title" && n.Content == "Test Content")), Times.Once);
        }
    }
}
