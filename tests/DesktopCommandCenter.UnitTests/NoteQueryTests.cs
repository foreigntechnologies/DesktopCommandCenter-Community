using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DesktopCommandCenter.Application.Features.Notes.Queries;
using DesktopCommandCenter.Application.Interfaces;
using DesktopCommandCenter.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace DesktopCommandCenter.UnitTests.Features.Notes
{
    public class NoteQueryTests
    {
        [Fact]
        public async Task GetNotes_Should_Return_All_Notes()
        {
            // Arrange
            var repositoryMock = new Mock<INoteRepository>();
            var expectedNotes = new List<Note>
            {
                new Note { Title = "Note 1" },
                new Note { Title = "Note 2" }
            };
            repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedNotes);

            var handler = new GetNotesQueryHandler(repositoryMock.Object);
            var query = new GetNotesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedNotes);
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
