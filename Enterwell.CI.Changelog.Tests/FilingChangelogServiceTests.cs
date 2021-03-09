using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Enterwell.CI.Changelog.Tests
{
    /// <summary>
    /// Test suite that inherits from the <see cref="TestBase"/> and contains high-level integration tests of the application.
    /// Only the inputs and outputs are tested, without testing each and every service.
    /// </summary>
    public class FilingChangelogServiceTests : TestBase
    {
        private const string ValidVersion = "1.2.3";
        private const string InvalidVersion = "1.2.3.";

        private readonly FillingChangelogService changelogService;

        /// <summary>
        /// Initializes the test with services that are needed.
        /// </summary>
        public FilingChangelogServiceTests()
        {
            var parseInputService = new ParseInputService();
            var changeGatheringService = new ChangeGatheringService();
            var markdownTextService = new MarkdownTextService();
            var fileWriterService = new FileWriterService();

            changelogService = new FillingChangelogService(parseInputService, changeGatheringService,
                markdownTextService, fileWriterService);
        }

        /// <summary>
        /// Testing the application when no inputs are passed. <see cref="ArgumentException"/> should be thrown with correct message.
        /// </summary>
        [Fact]
        public void FillingChangelogService_NoInputs_ThrowsArgumentExceptionWithCorrectMessage()
        {
            // Arrange
            var inputs = Array.Empty<string>();

            // Act
            Func<Task> act = async () => await changelogService.Run(inputs);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithMessage("Correct usage: <version[major.minor.patch]> <changelog location> <changes location>");
        }

        /// <summary>
        /// Testing the application when wrong inputs are passed. <see cref="ArgumentException"/> should be thrown with correct message.
        /// </summary>
        [Fact]
        public void FillingChangelogService_WrongInputs_ThrowsArgumentExceptionWithCorrectMessage()
        {
            // Arrange
            string[] inputs = {InvalidVersion, TestFolderPath, ChangesFolderPath};
            
            // Act
            Func<Task> act = async () => await changelogService.Run(inputs);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithMessage(
                $"Expected input format: <major.minor.patch>. Got: '{InvalidVersion}'. Check your separation dots!");
        }

        /// <summary>
        /// Testing the application when valid version and changelog location are passed, but the changes directory input is invalid and does not exist.
        /// <see cref="ArgumentException"/> should be thrown with correct message.
        /// </summary>
        [Fact]
        public void FillingChangelogService_ValidInputWithNoChangesDirectory_ThrowsArgumentExceptionWithCorrectMessage()
        {
            // Arrange
            string[] inputs = { ValidVersion, TestFolderPath, ChangesFolderPath };

            // Act
            Func<Task> act = async () => await changelogService.Run(inputs);

            // Assert
            act.Should().ThrowExactly<ArgumentException>().WithMessage($"Changes location does not exist on path: {ChangesFolderPath}");
        }

        /// <summary>
        /// Testing the application when changelog location does not contain a 'Changelog.md' file.
        /// <see cref="FileNotFoundException"/> should be thrown with correct message.
        /// </summary>
        [Fact]
        public void FillingChangelogService_ValidInputWithChangesDirectoryNoChangelog_ThrowsFileNotFoundExceptionWithCorrectMessage()
        {
            // Arrange
            string[] inputs = { ValidVersion, TestFolderPath, ChangesFolderPath };
            CreateChanges();

            // Act
            Func<Task> act = async () => await changelogService.Run(inputs);

            // Assert
            act.Should().ThrowExactly<FileNotFoundException>().WithMessage("Changelog file is not found.");
        }

        /// <summary>
        /// Testing the application when valid inputs are passed.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section.
        /// </summary>
        [Fact]
        public async void FillingChangelogService_ValidInputWithChangesDirectoryAndChangelog_ActsAsExpected()
        {
            // Arrange
            string[] inputs = { ValidVersion, TestFolderPath, ChangesFolderPath };
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