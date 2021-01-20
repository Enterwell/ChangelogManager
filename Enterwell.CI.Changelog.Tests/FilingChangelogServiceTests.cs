using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Enterwell.CI.Changelog.Tests
{
    public class FilingChangelogServiceTests : TestBase
    {
        private const string ValidVersion = "1.2.3";
        private const string InvalidVersion = "1.2.3.";

        private readonly FillingChangelogService changelogService;

        public FilingChangelogServiceTests()
        {
            var parseInputService = new ParseInputService();
            var changeGatheringService = new ChangeGatheringService();
            var markdownTextService = new MarkdownTextService();
            var fileWriterService = new FileWriterService();

            changelogService = new FillingChangelogService(parseInputService, changeGatheringService,
                markdownTextService, fileWriterService);
        }

        [Fact]
        public void FillingChangelogService_NoInputs_ThrowsArgumentExceptionWithCorrectMessage()
        {
            // Arrange

            // Act
            Func<Task> act = async () => await changelogService.Run(Array.Empty<string>());

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithMessage("Correct usage: <version[major.minor.patch]> <repository location>");
        }
    }
}