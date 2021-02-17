using System;
using System.IO;
using System.Reflection;
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
            var inputs = Array.Empty<string>();

            // Act
            Func<Task> act = async () => await changelogService.Run(inputs);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithMessage("Correct usage: <version[major.minor.patch]> <repository location>");
        }

        [Fact]
        public void FillingChangelogService_WrongInputs_ThrowsArgumentExceptionWithCorrectMessage()
        {
            // Arrange
            string[] inputs = {InvalidVersion, TestFolderPath};
            
            // Act
            Func<Task> act = async () => await changelogService.Run(inputs);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithMessage(
                $"Expected input format: <major.minor.patch>. Got: '{InvalidVersion}'. Check your separation dots!");
        }

        [Fact]
        public void FillingChangelogService_ValidInputWithNoChangesDirectory_ThrowsDirectoryNotFoundExceptionWithCorrectMessage()
        {
            // Arrange
            string[] inputs = { ValidVersion, TestFolderPath };

            // Act
            Func<Task> act = async () => await changelogService.Run(inputs);

            // Assert
            act.Should().ThrowExactly<DirectoryNotFoundException>().WithMessage("Directory 'changes' not found.");
        }

        [Fact]
        public void FillingChangelogService_ValidInputWithChangesDirectoryNoChangelog_ThrowsFileNotFoundExceptionWithCorrectMessage()
        {
            // Arrange
            string[] inputs = { ValidVersion, TestFolderPath };
            CreateChanges();

            // Act
            Func<Task> act = async () => await changelogService.Run(inputs);

            // Assert
            act.Should().ThrowExactly<FileNotFoundException>().WithMessage("Changelog file is not found.");
        }

        [Fact]
        public async void FillingChangelogService_ValidInputWithChangesDirectoryAndChangelog_ActsAsExpected()
        {
            // Arrange
            string[] inputs = { ValidVersion, TestFolderPath };
            CreateChanges();
            CreateChangelog();
            
            var expectedHeading = $"## [{ValidVersion}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = async () => await changelogService.Run(inputs);

            // Assert
            act.Should().NotThrow();
            Directory.GetFiles(ChangesFolderPath).Should().BeEmpty();

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }
    }
}