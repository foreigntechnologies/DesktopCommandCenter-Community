using System.Threading.Tasks;
using DesktopCommandCenter.Application.Features.Notes.Commands;
using DesktopCommandCenter.Application.Features.Notes.Queries;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;

namespace DesktopCommandCenter.IntegrationTests
{
    public class PipelineTests
    {
        private readonly ServiceProvider _serviceProvider;

        public PipelineTests()
        {
            var services = new ServiceCollection();
            // Assuming there are extension methods in the Application/Infrastructure for DI
            // If they don't exist, we just mock or register manually. But we saw DependencyInjection.cs
            // We'll manually register MediatR and Repositories if the extension methods fail, but let's assume they exist.
            
            // To make it run locally without side effects, we mock or use an in-memory DB.
            // For now, we will test MediatR wiring.
            services.AddLogging();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateNoteCommand).Assembly));
            
            // Registering mock implementations for integration test
            var mockNoteRepo = new Moq.Mock<DesktopCommandCenter.Application.Interfaces.INoteRepository>();
            mockNoteRepo.Setup(r => r.AddAsync(Moq.It.IsAny<DesktopCommandCenter.Domain.Entities.Note>()))
                .ReturnsAsync(new DesktopCommandCenter.Domain.Entities.Note { Title = "Integration Note", Content = "Test" });
                
            services.AddSingleton(mockNoteRepo.Object);
            
            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task CreateNoteCommand_Should_Be_Handled_By_Pipeline()
        {
            // Arrange
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            var command = new CreateNoteCommand("Integration Note", "Test", "TestCat");

            // Act
            var result = await mediator.Send(command);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Integration Note");
        }
    }
}
