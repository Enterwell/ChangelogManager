﻿using System;
using System.IO;
using Enterwell.CI.Changelog.Models;
using Newtonsoft.Json;

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
        protected readonly string TestFolderName = "ChangelogManagerTesting";

        /// <summary>
        /// Name of the folder inside the Testing folder which contains all of the changes with which our program works. Needs to be 'changes'.
        /// </summary>
        protected readonly string ChangesFolderName = "changes";

        /// <summary>
        /// Name of the changelog file.
        /// </summary>
        protected readonly string ChangelogFileName = "Changelog.md";

        /// <summary>
        /// Name of the configuration file we are using to configure Changelog program.
        /// </summary>
        protected readonly string ChangelogConfigFileName = ".changelog.json";

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
        /// Path got by combining the path to the system Temp directory with <see cref="ChangelogConfigFileName"/>
        /// </summary>
        protected readonly string ChangelogConfigFilePath;

        /// <summary>
        /// Project file path.
        /// </summary>
        protected string ProjectFilePath;

        /// <summary>
        /// <see cref="TestBase"/> constructor. It runs before every test in the test class that inherit the <see cref="TestBase"/>.
        /// </summary>
        public TestBase()
        {
            this.TestFolderPath = Path.Combine(this.TemporaryDirectoryPath, this.TestFolderName);
            this.ChangesFolderPath = Path.Combine(this.TestFolderPath, this.ChangesFolderName);
            this.ChangelogFilePath = Path.Combine(this.TestFolderPath, this.ChangelogFileName);
            this.ChangelogConfigFilePath = Path.Combine(this.TestFolderPath, this.ChangelogConfigFileName);

            Directory.CreateDirectory(this.TestFolderPath);
            Directory.SetCurrentDirectory(this.TestFolderPath);
        }

        /// <summary>
        /// Creates a directory on a given path. If the path starts with anything other than <see cref="TestFolderPath"/>, exception is thrown.
        /// </summary>
        /// <param name="folderPath">Path to the directory that needs to be created.</param>
        /// <exception cref="Exception">If the path starts with anything other than <see cref="TestFolderPath"/>.</exception>
        protected void CreateDirectory(string folderPath)
        {
            if (!folderPath.StartsWith(this.TestFolderPath))
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
            if (!filePath.StartsWith(this.TestFolderPath))
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
        /// <param name="fileNames">Names of the files to create.</param>
        protected void CreateChanges(string[] fileNames)
        {
            CreateDirectory(this.ChangesFolderPath);

            foreach (var file in fileNames)
            {
                CreateFile(Path.Combine(this.ChangesFolderPath, file));
            }
        }

        /// <summary>
        /// Creates a <see cref="ChangelogConfigFileName"/> file on a <see cref="ChangelogConfigFilePath"/> path and fills it with some configuration data.
        /// </summary>
        /// <param name="configuration">Configuration to write to the file.</param>
        protected void CreateConfiguration(Configuration configuration)
        {
            CreateFile(this.ChangelogConfigFilePath);

            WriteToFile(this.ChangelogConfigFilePath, JsonConvert.SerializeObject(configuration));
        }

        /// <summary>
        /// Creates the project file with the given project file name and with the given content.
        /// </summary>
        /// <param name="projectFileName">The project file name.</param>
        /// <param name="projectContent">Content of the project.</param>
        protected void CreateProjectFile(string projectFileName, string projectContent)
        {
            this.ProjectFilePath = Path.Combine(TestFolderPath, projectFileName);

            CreateFile(this.ProjectFilePath);
            WriteToFile(this.ProjectFilePath, projectContent);
        }

        /// <summary>
        /// Creates a <see cref="ChangelogFileName"/> file on a <see cref="ChangelogFilePath"/> path and fills it with some initial data.
        /// </summary>
        /// <param name="majorVersion">Major version number.</param>
        /// <param name="minorVersion">Minor version number.</param>
        /// <param name="patchVersion">Patch version number.</param>
        protected void CreateChangelog(int majorVersion, int minorVersion, int patchVersion)
        {
            CreateFile(this.ChangelogFilePath);

            string changelogContent = $"""
                                       # Changelog
                                       All notable changes to this project will be documented in this file.

                                       The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
                                       and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

                                       ## [{majorVersion}.{minorVersion}.{patchVersion}] - 2018-08-13
                                       ### Changed
                                       - Migrated from .NET Framework 4.5 to .NET Standard 2.0
                                       """;

            WriteToFile(this.ChangelogFilePath, changelogContent);
        }

        /// <summary>
        /// Clears the <see cref="TestFolderPath"/> from the system Temp that was used for testing. It is ran after every test in our test suite.
        /// </summary>
        public void Dispose()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Directory.Delete(this.TestFolderPath, true);
        }
    }
}