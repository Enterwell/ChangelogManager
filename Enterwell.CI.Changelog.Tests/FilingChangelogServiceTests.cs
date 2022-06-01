using System;
using System.IO;
using System.Threading.Tasks;
using Enterwell.CI.Changelog.Models;
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
        private readonly ChangelogManagerCommand changelogService;

        /// <summary>
        /// Initializes the test with services that are needed.
        /// </summary>
        public FilingChangelogServiceTests()
        {
            var changeGatheringService = new ChangeGatheringService();
            var markdownTextService = new MarkdownTextService();
            var fileWriterService = new FileWriterService();

            changelogService = new ChangelogManagerCommand(changeGatheringService, markdownTextService, fileWriterService)
            {
                ChangelogLocation = TestFolderPath,
                ChangesLocation = ChangesFolderPath
            };
        }

        /// <summary>
        /// Testing the application when changelog location does not contain a 'Changelog.md' file.
        /// <see cref="FileNotFoundException"/> should be thrown with correct message.
        /// </summary>
        [Fact]
        public void FillingChangelogService_ValidInputWithChangesDirectoryNoChangelog_ThrowsFileNotFoundExceptionWithCorrectMessage()
        {
            // Arrange
            var fileNames = Array.Empty<string>();
            CreateChanges(fileNames);

            // Act
            Func<Task> act = async () => await changelogService.OnExecute();

            // Assert
            act.Should().ThrowExactly<FileNotFoundException>().WithMessage("Changelog file is not found.");
        }

        /// <summary>
        /// Testing the application when valid inputs are passed.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the major version part is updated.
        /// </summary>
        [Fact]
        public async void FillingChangelogService_ValidInputs_UpdatesMajorCorrectly()
        {
            // Arrange
            string[] fileNames =
            {
                "Deprecated [FE] Deprecated example 1",
                "   Added [API] Added example 1",
                "   s  Removed [BE] this should not be accepted",
                "Added [API] Added example 2",
                "Removed [BE] Removed example 2",
                "added [API] Added example 3",
                "    s  aDDEd [Api] this should not be accepted",
                "Fixed [BE] Fixed example 1",
                
                "   deprecated [HUH] Description",
                "Changed [API] Changed example 1",
                "Security [API] Security example 1",
                "This should not be accepted as change",
                "Added [ThisIsFalseCategory] This should not be accepted as change if this category is not in configuration",
                " s",
                "a ",
                "-"
            };
            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            const int shouldBeMajor = initialMajor + 1;

            var expectedHeading = $"## [{shouldBeMajor}.0.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = async () => await changelogService.OnExecute();

            // Assert
            act.Should().NotThrow();
            Directory.GetFiles(ChangesFolderPath).Should().BeEmpty();

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the minor version part is updated.
        /// </summary>
        [Fact]
        public async void FillingChangelogService_ValidInputs_UpdatesMinorCorrectly()
        {
            // Arrange
            string[] fileNames =
            {
                "   Added [API] Added example 1",
                "   s  Removed [BE] this should not be accepted",
                "Added [API] Added example 2",
                "Removed [BE] Removed example 2",
                "added [API] Added example 3",
                "    s  aDDEd [Api] this should not be accepted",
                "Fixed [BE] Fixed example 1",
                "Changed [API] Changed example 1",
                "Security [API] Security example 1",
                "This should not be accepted as change",
                "Added [ThisIsFalseCategory] This should not be accepted as change if this category is not in configuration",
                " s",
                "a ",
                "-"
            };
            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            const int shouldBeMinor = initialMinor + 1;

            var expectedHeading = $"## [{initialMajor}.{shouldBeMinor}.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = async () => await changelogService.OnExecute();

            // Assert
            act.Should().NotThrow();
            Directory.GetFiles(ChangesFolderPath).Should().BeEmpty();

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the patch version part is updated.
        /// </summary>
        [Fact]
        public async void FillingChangelogService_ValidInputs_UpdatesPatchCorrectly()
        {
            // Arrange
            string[] fileNames =
            {
                "    s  aDDEd [Api] this should not be accepted",
                "Fixed [BE] Fixed example 1",
                "Security [API] Security example 1",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            const int shouldBePatch = initialPatch + 1;

            var expectedHeading = $"## [{initialMajor}.{initialMinor}.{shouldBePatch}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = async () => await changelogService.OnExecute();

            // Assert
            act.Should().NotThrow();
            Directory.GetFiles(ChangesFolderPath).Should().BeEmpty();

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed and the custom 'changelog' configuration exists.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the major version part is updated.
        /// </summary>
        [Fact]
        public async void FillingChangelogService_ValidInputsWithCustomConfiguration_UpdatesMajorCorrectly()
        {
            // Arrange
            string[] fileNames =
            {
                "   Added [API] Added example 1",
                "    s  aDDEd [Api] this should not be accepted",
                "Fixed [BE] Fixed example 1",
                "Security [API] Security example 1",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            var configuration = new Configuration
            {
                BumpingRule = new BumpingRule
                {
                    Major = new[] { "Added" },
                    Minor = new[] { "Fixed" },
                    Patch = new[] { "Security" }
                }
            };
            CreateConfiguration(configuration);

            const int shouldBeMajor = initialMajor + 1;

            var expectedHeading = $"## [{shouldBeMajor}.0.0] - {DateTime.Now:yyyy-MM-dd}";
            
            // Act
            Func<Task> act = async () => await changelogService.OnExecute();

            // Assert
            act.Should().NotThrow();
            Directory.GetFiles(ChangesFolderPath).Should().BeEmpty();

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed and the custom 'changelog' configuration exists.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the minor version part is updated.
        /// </summary>
        [Fact]
        public async void FillingChangelogService_ValidInputsWithCustomConfiguration_UpdatesMinorCorrectly()
        {
            // Arrange
            string[] fileNames =
            {
                "   Added [API] Added example 1",
                "    s  aDDEd [Api] this should not be accepted",
                "Fixed [BE] Fixed example 1",
                "Security [API] Security example 1",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            var configuration = new Configuration
            {
                BumpingRule = new BumpingRule
                {
                    Major = new[] { "Deprecated" },
                    Minor = new[] { "Added" },
                    Patch = new[] { "Security" }
                }
            };
            CreateConfiguration(configuration);

            const int shouldBeMinor = initialMinor + 1;

            var expectedHeading = $"## [{initialMajor}.{shouldBeMinor}.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = async () => await changelogService.OnExecute();

            // Assert
            act.Should().NotThrow();
            Directory.GetFiles(ChangesFolderPath).Should().BeEmpty();

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed and the custom 'changelog' configuration exists.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the patch version part is updated.
        /// </summary>
        [Fact]
        public async void FillingChangelogService_ValidInputsWithCustomConfiguration_UpdatesPatchCorrectly()
        {
            // Arrange
            string[] fileNames =
            {
                "   Added [API] Added example 1",
                "    s  aDDEd [Api] this should not be accepted",
                "Fixed [BE] Fixed example 1",
                "Security [API] Security example 1",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            var configuration = new Configuration
            {
                BumpingRule = new BumpingRule
                {
                    Major = new[] { "Deprecated" },
                    Minor = new[] { "Removed" },
                    Patch = new[] { "Fixed" }
                }
            };
            CreateConfiguration(configuration);

            const int shouldBePatch = initialPatch + 1;

            var expectedHeading = $"## [{initialMajor}.{initialMinor}.{shouldBePatch}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = async () => await changelogService.OnExecute();

            // Assert
            act.Should().NotThrow();
            Directory.GetFiles(ChangesFolderPath).Should().BeEmpty();

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }
    }
}