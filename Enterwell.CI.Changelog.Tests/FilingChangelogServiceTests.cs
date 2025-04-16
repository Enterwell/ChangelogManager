using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Enterwell.CI.Changelog.Models;
using FluentAssertions;
using Newtonsoft.Json;
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

            this.changelogService = new ChangelogManagerCommand(changeGatheringService, markdownTextService, fileWriterService)
            {
                ChangelogLocation = this.TestFolderPath,
                ChangesLocation = this.ChangesFolderPath
            };
        }

        /// <summary>
        /// Testing the application when changelog location does not contain a 'Changelog.md' file.
        /// <see cref="FileNotFoundException"/> should be thrown with correct message.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ChangesDirectoryNoChangelog_ThrowsFileNotFoundExceptionWithCorrectMessage()
        {
            // Arrange
            var fileNames = Array.Empty<string>();
            CreateChanges(fileNames);

            // Act
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().ThrowExactlyAsync<FileNotFoundException>().WithMessage("Changelog file is not found.");
        }

        /// <summary>
        /// Testing the application when change with a breaking keyword exists.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the major version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_BreakingKeyword_UpdatesMajorCorrectly()
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
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application with changes that bump minor by default.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the minor version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_DefaultMinorBumpingChanges_UpdatesMinorCorrectly()
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
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application with changes that bump patch by default.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the patch version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_DefaultPatchBumpingChanges_UpdatesPatchCorrectly()
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
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when the flag to not merge changelog is explicitly set.
        /// Test should not throw any exception, 'changes' directory should not be emptied and 'Changelog.md' file should not contain new version section.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ShouldNotMergeFlag_DoesNothingCorrectly()
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

            var expectedHeading = $"## [{initialMajor}.{initialMinor}.{initialPatch}]";

            // Act
            this.changelogService.ShouldMergeChangelog = false;
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(fileNames.Length);

            var changelogText = await File.ReadAllTextAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when the flags to not merge changelog and to version bump project file are set.
        /// Test should not throw any exception, 'changes' directory should not be emptied, 'Changelog.md' file should not contain new version section but the project file will be bumped.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ShouldNotMergeFlagWithVersionBump_UpdatesProjectFileCorrectly()
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

            var packageJsonContent = JsonConvert.SerializeObject(new
            {
                name = "tasks",
                version = $"{initialMajor}.{initialMinor}.{initialPatch}",
                description = ""
            }, Formatting.Indented);
            CreateProjectFile("package.json", packageJsonContent);

            const int shouldBePatch = initialPatch + 1;

            var expectedHeading = $"## [{initialMajor}.{initialMinor}.{initialPatch}]";
            var expectedPackageJsonVersion = $"{initialMajor}.{initialMinor}.{shouldBePatch}";

            // Act
            this.changelogService.ShouldMergeChangelog = false;
            this.changelogService.SetVersion = (true, null);
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(fileNames.Length);

            var changelogText = await File.ReadAllTextAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);

            var packageJsonText = await File.ReadAllTextAsync(this.ProjectFilePath);
            packageJsonText.Should().Contain(expectedPackageJsonVersion);
        }

        /// <summary>
        /// Testing the application bumps minor correctly when no changes have been made.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the minor version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_NoChanges_UpdatesMinorCorrectly()
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
            var validFileNames = Array.Empty<string>();
            var fileNames = validFileNames.Concat(invalidFileNames).ToArray();

            const int initialMajor = 1;
            const int initialMinor = 2;
            const int initialPatch = 3;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            const int shouldBeMinor = initialMinor + 1;

            var expectedHeading = $"## [{initialMajor}.{shouldBeMinor}.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application with custom 'changelog' configuration.
        /// The application bumps major version correctly based on a custom major category.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the major version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_CustomMajorCategory_UpdatesMajorCorrectly()
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
                    Major = ["Added"],
                    Minor = ["Fixed"],
                    Patch = ["Security"]
                }
            };
            CreateConfiguration(configuration);

            const int shouldBeMajor = initialMajor + 1;

            var expectedHeading = $"## [{shouldBeMajor}.0.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application with custom 'changelog' configuration.
        /// The application bumps major version correctly based on a custom breaking keyword.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the major version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_CustomBreakingKeyword_UpdatesMajorCorrectly()
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
                    Minor = ["Fixed"],
                    Patch = ["Security"]
                }
            };
            CreateConfiguration(configuration);

            const int shouldBeMajor = initialMajor + 1;

            var expectedHeading = $"## [{shouldBeMajor}.0.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application with custom 'changelog' configuration.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the minor version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_CustomConfiguration_UpdatesMinorCorrectly()
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
                    Major = ["Deprecated"],
                    Minor = ["Added"],
                    Patch = ["Security"]
                }
            };
            CreateConfiguration(configuration);

            const int shouldBeMinor = initialMinor + 1;

            var expectedHeading = $"## [{initialMajor}.{shouldBeMinor}.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application with custom 'changelog' configuration and no changes.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the minor version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_CustomConfigurationNoChanges_UpdatesMinorCorrectly()
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
            var validFileNames = Array.Empty<string>();
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
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            const int shouldBeMinor = initialMinor + 1;

            var expectedHeading = $"## [{initialMajor}.{shouldBeMinor}.0] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application with custom 'changelog' configuration.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section where the patch version part is updated.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_CustomConfiguration_UpdatesPatchCorrectly()
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
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            const int shouldBePatch = initialPatch + 1;

            var expectedHeading = $"## [{initialMajor}.{initialMinor}.{shouldBePatch}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);
        }

        /// <summary>
        /// Testing the application when trying to bump a project file that does not exist.
        /// <see cref="FileNotFoundException"/> should be thrown with correct message.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_VersionBumpNoProjectFile_ThrowsFileNotFoundExceptionWithCorrectMessage()
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
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            // Act
            this.changelogService.SetVersion = (true, null);
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().ThrowExactlyAsync<FileNotFoundException>().WithMessage("Could not find a 'package.json' file or a '.csproj' file with a 'Version' tag.");
        }

        /// <summary>
        /// Testing the application when package.json is automatically determined for version bumping.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section and the package.json's version should be bumped.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ImplicitPackageJson_BumpsVersionCorrectly()
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
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            var packageJsonContent = JsonConvert.SerializeObject(new
            {
                name = "tasks",
                version = $"{initialMajor}.{initialMinor}.{initialPatch}",
                description = ""
            }, Formatting.Indented);
            CreateProjectFile("package.json", packageJsonContent);

            const int shouldBePatch = initialPatch + 1;

            var expectedVersion = $"{initialMajor}.{initialMinor}.{shouldBePatch}";
            var expectedHeading = $"## [{expectedVersion}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            this.changelogService.SetVersion = (true, null);
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);

            var packageJsonText = await File.ReadAllTextAsync(this.ProjectFilePath);
            packageJsonText.Should().Contain(expectedVersion);
        }

        /// <summary>
        /// Testing the application when the package.json is explicitly set for version bumping.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section and the package.json's version should be bumped.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ExplicitPackageJson_BumpsVersionCorrectly()
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
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            var subFolderPath = Path.Combine(this.TestFolderPath, "subfolder");
            Directory.CreateDirectory(subFolderPath);

            var packageJsonFilePath = Path.Combine(subFolderPath, "package.json");
            var packageJsonContent = JsonConvert.SerializeObject(new
            {
                name = "tasks",
                version = $"{initialMajor}.{initialMinor}.{initialPatch}",
                description = ""
            }, Formatting.Indented);
            CreateProjectFile(packageJsonFilePath, packageJsonContent);

            const int shouldBePatch = initialPatch + 1;

            var expectedVersion = $"{initialMajor}.{initialMinor}.{shouldBePatch}";
            var expectedHeading = $"## [{expectedVersion}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            this.changelogService.SetVersion = (true, packageJsonFilePath);
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);

            var packageJsonText = await File.ReadAllTextAsync(this.ProjectFilePath);
            packageJsonText.Should().Contain(expectedVersion);
        }

        /// <summary>
        /// Testing the application when a .json file is explicitly set for version bumping.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section and the .json's version should be bumped.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ExplicitGeneralJson_BumpsVersionCorrectly()
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
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            var subFolderPath = Path.Combine(this.TestFolderPath, "subfolder");
            Directory.CreateDirectory(subFolderPath);

            var jsonFilePath = Path.Combine(subFolderPath, "composer.json");
            var jsonContent = JsonConvert.SerializeObject(new
            {
                name = "tasks",
                version = $"{initialMajor}.{initialMinor}.{initialPatch}",
                description = ""
            }, Formatting.Indented);
            CreateProjectFile(jsonFilePath, jsonContent);

            const int shouldBePatch = initialPatch + 1;

            var expectedVersion = $"{initialMajor}.{initialMinor}.{shouldBePatch}";
            var expectedHeading = $"## [{expectedVersion}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            this.changelogService.SetVersion = (true, jsonFilePath);
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);

            var jsonText = await File.ReadAllTextAsync(this.ProjectFilePath);
            jsonText.Should().Contain(expectedVersion);
        }

        /// <summary>
        /// Testing the application when a file with version in its documentation is explicitly set for version bumping.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section and the file's documented version should be bumped.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ExplicitDocumentationFile_BumpsVersionCorrectly()
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
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            var subFolderPath = Path.Combine(this.TestFolderPath, "subfolder");
            Directory.CreateDirectory(subFolderPath);

            var documentationFilePath = Path.Combine(subFolderPath, "test.php");
            var documentationFileContent = $"""
                                 <?php
                                 /**
                                  * The plugin bootstrap file
                                  *
                                  * This file is read by WordPress to generate the plugin information in the plugin
                                  * admin area. This file also includes all of the dependencies used by the plugin,
                                  * registers the activation and deactivation functions, and defines a function
                                  * that starts the plugin.
                                  *
                                  * @link              http://example.com
                                  * @since             1.0.0
                                  * @package           PluginName
                                  *
                                  * @wordpress-plugin
                                  * Plugin Name:       PluginName
                                  * Plugin URI:        http://enterwell.net
                                  * Description:       Administration plugin.
                                  * Version:           {initialMajor}.{initialMinor}.{initialPatch}
                                  * Author:            Enterwell
                                  * Author URI:        http://enterwell.net/
                                  * License:           GPL-2.0+
                                  * License URI:       http://www.gnu.org/licenses/gpl-2.0.txt
                                  * Text Domain:       PluginName
                                  * Domain Path:       /languages
                                  * Version:           1.1.1
                                  */
                                 """;
            CreateProjectFile(documentationFilePath, documentationFileContent);

            const int shouldBePatch = initialPatch + 1;

            var expectedVersion = $"{initialMajor}.{initialMinor}.{shouldBePatch}";
            var expectedHeading = $"## [{expectedVersion}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            this.changelogService.SetVersion = (true, documentationFilePath);
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);

            var fileText = await File.ReadAllTextAsync(this.ProjectFilePath);
            fileText.Should().Contain(expectedVersion);
            fileText.Should().Contain("1.1.1");
        }

        /// <summary>
        /// Testing the application when a manifest file with version is explicitly set for version bumping.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section and the file's version should be bumped.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ExplicitVsixManifestFile_BumpsVersionCorrectly()
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
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            var subFolderPath = Path.Combine(this.TestFolderPath, "subfolder");
            Directory.CreateDirectory(subFolderPath);

            var manifestFilePath = Path.Combine(subFolderPath, "test.vsixmanifest");
            var manifestFileContent = $"""
                                 <?xml version="1.0" ?>
                                 <PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011" xmlns:d="http://schemas.microsoft.com/developer/vsx-schema-design/2011">
                                     <Metadata>
                                         <Identity Id="SomethingSomething" Version="{initialMajor}.{initialMinor}.{initialPatch}" Language="en-US" Publisher="Something" />
                                         <DisplayName>Changelog Create</DisplayName>
                                     </Metadata>
                                     <Installation>
                                         <InstallationTarget Id="Microsoft.VisualStudio.Community" Version="[16.0, 17.0)" />
                                         <InstallationTarget Id="Microsoft.VisualStudio.Community" Version="[17.0, 18.0)" >
                                             <ProductArchitecture>amd64</ProductArchitecture>
                                         </InstallationTarget>
                                     </Installation>
                                     <Dependencies>
                                     </Dependencies>
                                     <Prerequisites>
                                         <Prerequisite Id="Microsoft.VisualStudio.Component.CoreEditor" Version="[16.0,)" DisplayName="Visual Studio core editor" />
                                     </Prerequisites>
                                     <Assets>
                                         <Asset Type="Microsoft.VisualStudio.VsPackage" d:Source="Project" d:ProjectName="%CurrentProject%" Path="|%CurrentProject%;PkgdefProjectOutputGroup|" />
                                     </Assets>
                                 </PackageManifest>
                                 """;
            CreateProjectFile(manifestFilePath, manifestFileContent);

            const int shouldBePatch = initialPatch + 1;

            var expectedVersion = $"{initialMajor}.{initialMinor}.{shouldBePatch}";
            var expectedHeading = $"## [{expectedVersion}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            this.changelogService.SetVersion = (true, manifestFilePath);
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);

            var fileText = await File.ReadAllTextAsync(this.ProjectFilePath);
            fileText.Should().Contain("2.0.0");
            fileText.Should().Contain(expectedVersion);
        }

        /// <summary>
        /// Testing the application when an assembly info file is explicitly set for version bumping and there is no explicit revision number given.
        /// Test should not throw any exception, 'changes' directory should be emptied, 'Changelog.md' file should contain new version section and the assembly info's version should be bumped with revision number unchanged.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ExplicitAssemblyInfoFileNoExplicitRevision_BumpsVersionCorrectly()
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
            const int initialRevision = 12345;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            var configuration = new Configuration
            {
                BumpingRule = new BumpingRule
                {
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            var subFolderPath = Path.Combine(this.TestFolderPath, "subfolder");
            Directory.CreateDirectory(subFolderPath);

            var assemblyInfoFilePath = Path.Combine(subFolderPath, "AssemblyInfo.cs");
            var assemblyInfoFileContent = $"""
                                 ﻿using System.Reflection;
                                 using System.Runtime.InteropServices;
                                 using System.Runtime.Serialization;
                                 
                                 // General Information about an assembly is controlled through the following 
                                 // set of attributes. Change these attribute values to modify the information
                                 // associated with an assembly.
                                 [assembly: AssemblyTitle("ServiceStack.Examples.ServiceModel")]
                                 [assembly: AssemblyDescription("")]
                                 [assembly: AssemblyConfiguration("")]
                                 [assembly: AssemblyCompany("")]
                                 [assembly: AssemblyProduct("ServiceStack.Examples.ServiceModel")]
                                 [assembly: AssemblyCopyright("Copyright ©  2010")]
                                 [assembly: AssemblyTrademark("")]
                                 [assembly: AssemblyCulture("")]
                                 
                                 // Setting ComVisible to false makes the types in this assembly not visible 
                                 // to COM components.  If you need to access a type in this assembly from 
                                 // COM, set the ComVisible attribute to true on that type.
                                 [assembly: ComVisible(false)]
                                 
                                 // The following GUID is for the ID of the typelib if this project is exposed to COM
                                 [assembly: Guid("8870d9fc-01e9-46e9-a89f-e3194f965096")]
                                 
                                 // Version information for an assembly consists of the following four values:
                                 //
                                 //      Major Version
                                 //      Minor Version 
                                 //      Build Number
                                 //      Revision
                                 //
                                 // You can specify all the values or you can default the Build and Revision Numbers 
                                 // by using the '*' as shown below:
                                 // [assembly: AssemblyVersion("1.0.*")]
                                 [assembly: AssemblyVersion("{initialMajor}.{initialMinor}.{initialPatch}.{initialRevision}")]
                                 [assembly: AssemblyFileVersion("1.1.1205.1040")]
                                 """;
            CreateProjectFile(assemblyInfoFilePath, assemblyInfoFileContent);

            const int shouldBePatch = initialPatch + 1;

            var expectedChangelogVersion = $"{initialMajor}.{initialMinor}.{shouldBePatch}";
            var expectedChangelogHeading = $"## [{expectedChangelogVersion}] - {DateTime.Now:yyyy-MM-dd}";
            var expectedAssemblyVersion = $"{expectedChangelogVersion}.{initialRevision}";

            // Act
            this.changelogService.SetVersion = (true, assemblyInfoFilePath);
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedChangelogHeading);

            var fileText = await File.ReadAllTextAsync(this.ProjectFilePath);
            fileText.Should().Contain(expectedAssemblyVersion);
        }

        /// <summary>
        /// Testing the application when an assembly info file is explicitly set for version bumping and there is an explicit revision given.
        /// Test should not throw any exception, 'changes' directory should be emptied, 'Changelog.md' file should contain new version section and the assembly info's version should be bumped with revision number that was explicitly given.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ExplicitAssemblyInfoFileAndExplicitRevision_BumpsVersionCorrectly()
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
            const int initialRevision = 12345;

            CreateChanges(fileNames);
            CreateChangelog(initialMajor, initialMinor, initialPatch);

            var configuration = new Configuration
            {
                BumpingRule = new BumpingRule
                {
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            var subFolderPath = Path.Combine(this.TestFolderPath, "subfolder");
            Directory.CreateDirectory(subFolderPath);

            var assemblyInfoFilePath = Path.Combine(subFolderPath, "AssemblyInfo.cs");
            var assemblyInfoFileContent = $"""
                                 ﻿using System.Reflection;
                                 using System.Runtime.InteropServices;
                                 using System.Runtime.Serialization;
                                 
                                 // General Information about an assembly is controlled through the following 
                                 // set of attributes. Change these attribute values to modify the information
                                 // associated with an assembly.
                                 [assembly: AssemblyTitle("ServiceStack.Examples.ServiceModel")]
                                 [assembly: AssemblyDescription("")]
                                 [assembly: AssemblyConfiguration("")]
                                 [assembly: AssemblyCompany("")]
                                 [assembly: AssemblyProduct("ServiceStack.Examples.ServiceModel")]
                                 [assembly: AssemblyCopyright("Copyright ©  2010")]
                                 [assembly: AssemblyTrademark("")]
                                 [assembly: AssemblyCulture("")]
                                 
                                 // Setting ComVisible to false makes the types in this assembly not visible 
                                 // to COM components.  If you need to access a type in this assembly from 
                                 // COM, set the ComVisible attribute to true on that type.
                                 [assembly: ComVisible(false)]
                                 
                                 // The following GUID is for the ID of the typelib if this project is exposed to COM
                                 [assembly: Guid("8870d9fc-01e9-46e9-a89f-e3194f965096")]
                                 
                                 // Version information for an assembly consists of the following four values:
                                 //
                                 //      Major Version
                                 //      Minor Version 
                                 //      Build Number
                                 //      Revision
                                 //
                                 // You can specify all the values or you can default the Build and Revision Numbers 
                                 // by using the '*' as shown below:
                                 // [assembly: AssemblyVersion("1.0.*")]
                                 [assembly: AssemblyVersion("{initialMajor}.{initialMinor}.{initialPatch}.{initialRevision}")]
                                 [assembly: AssemblyFileVersion("1.1.1205.1040")]
                                 """;
            CreateProjectFile(assemblyInfoFilePath, assemblyInfoFileContent);

            const int shouldBePatch = initialPatch + 1;
            const int explicitRevision = 54321;

            var expectedChangelogVersion = $"{initialMajor}.{initialMinor}.{shouldBePatch}";
            var expectedChangelogHeading = $"## [{expectedChangelogVersion}] - {DateTime.Now:yyyy-MM-dd}";
            var expectedAssemblyVersion = $"{expectedChangelogVersion}.{explicitRevision}";

            // Act
            this.changelogService.SetVersion = (true, assemblyInfoFilePath);
            this.changelogService.RevisionNumber = explicitRevision;
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedChangelogHeading);

            var fileText = await File.ReadAllTextAsync(this.ProjectFilePath);
            fileText.Should().Contain(expectedAssemblyVersion);
        }

        /// <summary>
        /// Testing the application when .csproj is automatically determined for version bumping.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section and the .csproj's version should be bumped.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ImplicitCsproj_BumpsVersionCorrectly()
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
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            var csprojContent = $"""
                                 <Project Sdk="Microsoft.NET.Sdk">
                                 	<PropertyGroup>
                                 		<Version>{initialMajor}.{initialMinor}.{initialPatch}</Version>
                                 	</PropertyGroup>
                                 </Project>
                                 """;
            CreateProjectFile("test.csproj", csprojContent);

            const int shouldBePatch = initialPatch + 1;

            var expectedVersion = $"{initialMajor}.{initialMinor}.{shouldBePatch}";
            var expectedHeading = $"## [{expectedVersion}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            this.changelogService.SetVersion = (true, null);
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);

            var csprojText = await File.ReadAllTextAsync(this.ProjectFilePath);
            csprojText.Should().Contain(expectedVersion);
        }

        /// <summary>
        /// Testing the application when .csproj file is explicitly set for version bumping.
        /// Test should not throw any exception, 'changes' directory should be emptied and 'Changelog.md' file should contain new version section and the .csproj's version should be bumped.
        /// </summary>
        [Fact]
        public async Task FillingChangelogService_ExplicitCsproj_BumpsVersionCorrectly()
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
                    Major = ["Deprecated"],
                    Minor = ["Removed"],
                    Patch = ["Fixed"]
                }
            };
            CreateConfiguration(configuration);

            var subFolderPath = Path.Combine(this.TestFolderPath, "subfolder");
            Directory.CreateDirectory(subFolderPath);

            var csprojFilePath = Path.Combine(subFolderPath, "test.csproj");
            var csprojContent = $"""
                                 <Project Sdk="Microsoft.NET.Sdk">
                                 	<PropertyGroup>
                                 		<Version>{initialMajor}.{initialMinor}.{initialPatch}</Version>
                                 	</PropertyGroup>
                                 </Project>
                                 """;
            CreateProjectFile(csprojFilePath, csprojContent);

            const int shouldBePatch = initialPatch + 1;

            var expectedVersion = $"{initialMajor}.{initialMinor}.{shouldBePatch}";
            var expectedHeading = $"## [{expectedVersion}] - {DateTime.Now:yyyy-MM-dd}";

            // Act
            this.changelogService.SetVersion = (true, csprojFilePath);
            Func<Task> act = () => this.changelogService.OnExecute();

            // Assert
            await act.Should().NotThrowAsync();

            var changeFilesRemaining = Directory.GetFiles(this.ChangesFolderPath);

            changeFilesRemaining.Should().NotBeEmpty();
            changeFilesRemaining.Should().HaveCount(invalidFileNames.Length);

            var changelogText = await File.ReadAllLinesAsync(this.ChangelogFilePath);
            changelogText.Should().Contain(expectedHeading);

            var csprojText = await File.ReadAllTextAsync(this.ProjectFilePath);
            csprojText.Should().Contain(expectedVersion);
        }
    }
}