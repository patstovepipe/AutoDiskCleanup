using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutoDiskCleanup.Tests
{
    public class WorkerTests
    {
        private Mock<ILogger<Worker>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;

        public WorkerTests()
        {
            _mockLogger = new Mock<ILogger<Worker>>();
            _mockConfiguration = new Mock<IConfiguration>();
        }

        [Fact]
        public void Test_ExecuteAsync_WithRunAtStart_True()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["RunAtStart"]).Returns("true");
            var worker = new Worker(_mockLogger.Object, _mockConfiguration.Object);

            // Act
            var task = worker.StartAsync(CancellationToken.None);

            // Assert
            // Add your assertions here

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == "Running once-off folder cleanup at start of service."),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
                )
            );
        }

        [Fact]
        public void Test_ExecuteAsync_WithRunAtStart_False()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["RunAtStart"]).Returns("false");
            var worker = new Worker(_mockLogger.Object, _mockConfiguration.Object);

            // Act
            var task = worker.StartAsync(CancellationToken.None);

            // Assert
            // Add your assertions here
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() != "Running once-off folder cleanup at start of service."),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
                )
            );
        }
    }
}