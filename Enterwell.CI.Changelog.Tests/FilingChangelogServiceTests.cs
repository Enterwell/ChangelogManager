using System;
using System.IO;
using System.Linq;
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
        public async Task FillingChangelogService_ValidInputWithChangesDirectoryNoChangelog_ThrowsFileNotFoundExceptionWithCorrectMessage()
        {
            // Arrange
            var fileNames = Array.Empty<string>();
            CreateChanges(fileNames);

            // Act
            Func<Task> act = () => changelogService.OnExecute();

            // Assert
            await act.Should().ThrowExactlyAsync<FileNotFoundException>().WithMessage("Changelog file is not found.");
        }

        /// <summary>
        /// Testing the application when valid inputs are passed.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the major version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ValidInputs_UpdatesMajorCorrectly()
        {
            // Arrange
            var invalidFileNames = new[]
            {
                "   s  Removed [BE] this should not be accepted",
                "    s  aDDEd [Api] this should not be accepted",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            var validFileNames = new[]
            {
                "Deprecated [FE] BREAKING CHANGE 1",
                "   Added [API] Added example 1",
                "Added [API] Added example 2",
                "Removed [BE] Removed example 2",
                "added [API] Added example 3",
                "Fixed [BE] Fixed example 1",
                "   deprecated [HUH] Description",
                "Changed [API] Changed example 1",
                "Security [API] Security example 1",
                "Added [ThisIsFalseCategory] This should not be accepted as change if this category is not in configuration",
            };
            var fileNames = validFileNames.Concat(invalidFileNames).ToArray();
            
            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            const int shouldBeMajor = initialMajor + 1;

            var expectedHeading = $"## [{shouldBeMajor}.0.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = () => changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the minor version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ValidInputs_UpdatesMinorCorrectly()
        {
            // Arrange
            var invalidFileNames = new[]
            {
                "   s  Removed [BE] this should not be accepted",
                "    s  aDDEd [Api] this should not be accepted",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            var validFileNames = new[]
            {
                "Deprecated [FE] Deprecated example 1",
                "   Added [API] Added example 1",
                "Added [API] Added example 2",
                "Removed [BE] Removed example 2",
                "added [API] Added example 3",
                "Fixed [BE] Fixed example 1",
                "Changed [API] Changed example 1",
                "Security [API] Security example 1",
                "Added [ThisIsFalseCategory] This should not be accepted as change if this category is not in configuration",
            };
            var fileNames = validFileNames.Concat(invalidFileNames).ToArray();

            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            const int shouldBeMinor = initialMinor + 1;

            var expectedHeading = $"## [{initialMajor}.{shouldBeMinor}.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = () => changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the patch version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ValidInputs_UpdatesPatchCorrectly()
        {
            // Arrange
            var invalidFileNames = new[]
            {
                "    s  aDDEd [Api] this should not be accepted",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            var validFileNames = new[]
            {
                "Fixed [BE] Fixed example 1",
                "Security [API] Security example 1"
            };
            var fileNames = validFileNames.Concat(invalidFileNames).ToArray();

            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            const int shouldBePatch = initialPatch + 1;

            var expectedHeading = $"## [{initialMajor}.{initialMinor}.{shouldBePatch}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = () => changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed and the custom 'changelog' configuration exists.
        /// Testing that the application bumps major version correctly based on a custom major category.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the major version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ValidInputsWithCustomMajorCategory_UpdatesMajorCorrectly()
        {
            // Arrange
            var invalidFileNames = new[]
            {
                "    s  aDDEd [Api] this should not be accepted",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            var validFileNames = new[]
            {
                "   Added [API] Added example 1",
                "Fixed [BE] Fixed example 1",
                "Security [API] Security example 1"
                
            };
            var fileNames = validFileNames.Concat(invalidFileNames).ToArray();

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
            Func<Task> act = () => changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed and the custom 'changelog' configuration exists.
        /// Testing that the application bumps major version correctly based on a custom breaking keyword.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the major version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ValidInputsWithCustomBreakingKeyword_UpdatesMajorCorrectly()
        {
            // Arrange
            var invalidFileNames = new[]
            {
                "    s  aDDEd [Api] this should not be accepted",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            var validFileNames = new[]
            {
                "   Added [API] SoMEtHing CuSTOm example 1",
                "Fixed [BE] Fixed example 1",
                "Security [API] Security example 1"

            };
            var fileNames = validFileNames.Concat(invalidFileNames).ToArray();

            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            var configuration = new Configuration
            {
                BumpingRule = new BumpingRule
                {
                    BreakingKeyword = "Something Custom",
                    Minor = new[] { "Fixed" },
                    Patch = new[] { "Security" }
                }
            };
            CreateConfiguration(configuration);

            const int shouldBeMajor = initialMajor + 1;

            var expectedHeading = $"## [{shouldBeMajor}.0.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = () => changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed and the custom 'changelog' configuration exists.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the minor version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ValidInputsWithCustomConfiguration_UpdatesMinorCorrectly()
        {
            // Arrange
            var invalidFileNames = new[]
            {
                "    s  aDDEd [Api] this should not be accepted",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            var validFileNames = new[]
            {
                "   Added [API] Added example 1",
                "Fixed [BE] Fixed example 1",
                "Security [API] Security example 1"
            };
            var fileNames = validFileNames.Concat(invalidFileNames).ToArray();

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
            Func<Task> act = () => changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when valid inputs are passed and the custom 'changelog' configuration exists.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the patch version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ValidInputsWithCustomConfiguration_UpdatesPatchCorrectly()
        {
            // Arrange
            var invalidFileNames = new[]
            {
                "    s  aDDEd [Api] this should not be accepted",
                "This should not be accepted as change",
                " s",
                "a ",
                "-"
            };
            var validFileNames = new[]
            {
                "   Added [API] Added example 1",
                "Fixed [BE] Fixed example 1",
                "Security [API] Security example 1"
            };
            var fileNames = validFileNames.Concat(invalidFileNames).ToArray();

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
            Func<Task> act = () => changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }
    }
}