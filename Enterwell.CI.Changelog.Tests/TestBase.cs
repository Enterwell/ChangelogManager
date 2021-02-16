using System;
using System.IO;

namespace Enterwell.CI.Changelog.Tests
{
    /// <summary>
    /// Base class for our tests. Contains logic to help setup almost every test, by calling its methods in the Arrange part of the test. (Following AAA pattern).
    /// By inheriting the <see cref="IDisposable"/> and implementing its <see cref="Dispose"/> method, we can designate a method to run after every test.
    /// </summary>
    public class TestBase : IDisposable
    {
        /// <summary>
        /// Gets the path to the system Temp directory which we are using for testing purposes.
        /// </summary>
        protected readonly string TemporaryDirectoryPath = Path.GetTempPath();
        
        /// <summary>
        /// Name of the folder which is going to be created inside system Temp directory for testing purposes.
        /// </summary>
        protected readonly string TestFolderName = "Testing";

        /// <summary>
        /// Name of the folder inside the Testing folder which contains all of the changes with which our program works. Needs to be 'changes'.
        /// </summary>
        protected readonly string ChangesFolderName = "changes";

        /// <summary>
        /// Name of the configuration file we are using to configure Changelog program.
        /// </summary>
        protected readonly string ChangelogFileName = "Changelog.md";

        /// <summary>
        /// Path got by combining the path to the system Temp directory with <see cref="TestFolderName"/>
        /// </summary>
        protected readonly string TestFolderPath;

        /// <summary>
        /// Path got by combining the path to the system Temp directory with <see cref="ChangesFolderName"/>
        /// </summary>
        protected readonly string ChangesFolderPath;

        /// <summary>
        /// Path got by combining the path to the system Temp directory with <see cref="ChangelogFileName"/>
        /// </summary>
        protected readonly string ChangelogFilePath;

        /// <summary>
        /// <see cref="TestBase"/> constructor. It runs before every test in the test class that inherit the <see cref="TestBase"/>.
        /// </summary>
        public TestBase()
        {
            TestFolderPath = Path.Combine(TemporaryDirectoryPath, TestFolderName);
            ChangesFolderPath = Path.Combine(TestFolderPath, ChangesFolderName);
            ChangelogFilePath = Path.Combine(TestFolderPath, ChangelogFileName);

            Directory.CreateDirectory(TestFolderPath);
        }

        /// <summary>
        /// Creates a directory on a given path. If the path starts with anything other than <see cref="TestFolderPath"/>, exception is thrown.
        /// </summary>
        /// <param name="folderPath">Path to the directory that needs to be created.</param>
        /// <exception cref="Exception">If the path starts with anything other than <see cref="TestFolderPath"/>.</exception>
        protected void CreateDirectory(string folderPath)
        {
            if (!folderPath.StartsWith(TestFolderPath))
            {
                throw new Exception("Cannot create directories on given path!");
            }

            Directory.CreateDirectory(folderPath);
        }

        /// <summary>
        /// Creates a file on a given path. If the path starts with anything other than <see cref="TestFolderPath"/>, exception is thrown.
        /// </summary>
        /// <param name="filePath">Path to the file that needs to be created.</param>
        /// <exception cref="Exception">If the path starts with anything other than <see cref="TestFolderPath"/>.</exception>
        protected void CreateFile(string filePath)
        {
            if (!filePath.StartsWith(TestFolderPath))
            {
                throw new Exception("Cannot create files on given path!");
            }

            var file = File.Create(filePath);
            file.Close();
        }

        /// <summary>
        /// Writes arbitrary content to the file on a given path.
        /// </summary>
        /// <param name="filePath">Path to the file that needs to be written to.</param>
        /// <param name="text">Content that will be written to the file.</param>
        protected void WriteToFile(string filePath, string text)
        {
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// Creates directory on a <see cref="ChangesFolderPath"/> path and fills it with various files
        /// containing different names that could be an edge-case for our application.
        /// </summary>
        protected void CreateChanges()
        {
            CreateDirectory(ChangesFolderPath);

            string[] fileNames =
            {
                "   Added [API] Added example 1",
                "   s  Removed [BE] this should not be accepted",
                "Added [API] Added example 2",
                "Removed [BE] Removed example 2",
                "added [API] Added example 3",
                "    s  aDDEd [Api] this should not be accepted",
                "Fixed [BE] Fixed example 1",
                "Deprecated [FE] Deprecated example 1",
                "   deprecated [HUH] Description",
                "Changed [API] Changed example 1",
                "Security [API] Security example 1",
                "This should not be accepted as change",
                "Added [ThisIsFalseCategory] This should not be accepted as change if this category is not in configuration",
                " s",
                "a ",
                "-"
            };

            foreach (var file in fileNames)
            {
                CreateFile(Path.Combine(ChangesFolderPath, file));
            }
        }

        /// <summary>
        /// Creates a <see cref="ChangelogFileName"/> file on a <see cref="ChangelogFilePath"/> path and fills it with some initial data.
        /// </summary>
        protected void CreateChangelog()
        {
            CreateFile(ChangelogFilePath);

            string changelogContent = @"# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2018-08-13
### Changed
- Migrated from .NET Framework 4.5 to .NET Standard 2.0
";

            WriteToFile(ChangelogFilePath, changelogContent);
        }

        /// <summary>
        /// Clears the <see cref="TestFolderPath"/> from the system Temp that was used for testing. It is ran after every test in our test suite.
        /// </summary>
        public void Dispose()
        {
            Directory.Delete(TestFolderPath, true);
        }
    }
}