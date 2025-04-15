using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Enterwell.CI.Changelog.Shared
{
    /// <summary>
    /// Helper class for everything related to files and directories.
    /// </summary>
    public static class FileSystemHelper
    {
        public static string ChangeDirectoryName = "changes";
        public static string ChangelogFileName = "CHANGELOG.md";

        /// <summary>
        /// Used to ensure that the changes directory exists. If it does not exist, the directory is created.
        /// </summary>
        /// <returns><see cref="string"/> representing the path to case-insensitive changes directory.</returns>
        public static string EnsureChangesDirectoryExists(string directoryPath)
        {
            var currentDir = new DirectoryInfo(directoryPath);

            var changesDirectoryPath = currentDir.EnumerateDirectories()
                .FirstOrDefault(d => d.Name.Equals(ChangeDirectoryName, StringComparison.OrdinalIgnoreCase));

            if (changesDirectoryPath is not { Exists: true })
            {
                var newChangesDirectoryPath = Path.Combine(currentDir.FullName, ChangeDirectoryName);
                Directory.CreateDirectory(newChangesDirectoryPath);

                return newChangesDirectoryPath;
            }

            return changesDirectoryPath.FullName;
        }

        /// <summary>
        /// Creates the file on the given file path.
        /// </summary>
        /// <param name="filePath">Path to the file that needs to be created.</param>
        /// <returns>A value <see cref="Tuple"/> with 2 components. A <see cref="bool"/> that represents if the file was successfully created and a
        /// <see cref="string"/> that specifies the reason if not.</returns>
        public static (bool isSuccessfull, string reason) CreateFile(string filePath)
        {
            try
            {
                var file = File.Create(filePath);
                file.Close();

                return (true, string.Empty);
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        /// <summary>
        /// Constructs the file name using the arguments passed in by the user.
        /// </summary>
        /// <param name="inputType">Change type got by the user.</param>
        /// <param name="inputCategory">Change category got by the user.</param>
        /// <param name="inputDescription">Change description got by the user.</param>
        /// <returns>Name of the file to be saved in the folder where changes are stored.</returns>
        public static string ConstructFileName(string inputType, string inputCategory, string inputDescription)
        {
            var description = inputDescription.Trim();

            string fileName;
            if (string.IsNullOrWhiteSpace(inputCategory))
            {
                fileName = $"{inputType} {description}";
            }
            else
            {
                var category = inputCategory.Trim();

                fileName = $"{inputType} [{category}] {description}";
            }

            // Replace multiple spaces with a single space for consistency
            return Regex.Replace(fileName, @"\s+", " ");
        }

        /// <summary>
        /// Tries to find the nearest `changes` folder.
        /// </summary>
        /// <returns><see cref="string"/> representing the path to the nearest `changes` folder.</returns>
        public static string FindNearestChangesFolder(string directoryPath)
        {
            var changelogPath = FindNearestChangelogDirectoryPath(directoryPath);
            var changesDirectoryPath = EnsureChangesDirectoryExists(changelogPath);

            return changesDirectoryPath;
        }

        /// <summary>
        /// Tries to find the nearest 'CHANGELOG.md' file directory path.
        /// </summary>
        /// <returns><see cref="string" /> representing the path to the nearest 'CHANGELOG.md' file.</returns>
        public static string FindNearestChangelogDirectoryPath(string directoryPath)
        {
            var currentDir = new DirectoryInfo(directoryPath);
            while (currentDir != null)
            {
                var changelogFileInfo = currentDir.EnumerateFiles()
                    .FirstOrDefault(f => f.Name.Equals(ChangelogFileName, StringComparison.OrdinalIgnoreCase));

                if (changelogFileInfo != null)
                {
                    return currentDir.FullName;
                }

                currentDir = currentDir.Parent;
            }

            return directoryPath;
        }

        /// <summary>
        /// Tries to find the project file (either 'package.json' or a '*.csproj' file with a 'Version' tag) in the current directory.
        /// </summary>
        /// <returns>Project file path if one exists; <see cref="string.Empty"/> otherwise.</returns>
        public static string GetTheProjectFile()
        {
            var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            var currentDirFiles = currentDir.EnumerateFiles().ToList();

            var packageJsonFile = currentDirFiles.FirstOrDefault(f => f.Name == "package.json");

            // If the package.json file exists, return it
            if (packageJsonFile != null)
            {
                return packageJsonFile.FullName;
            }

            var csprojFile = currentDirFiles.FirstOrDefault(f => f.Extension == ".csproj");

            // If the .csproj file exists check if it contains the 'Version' tag
            if (csprojFile != null)
            {
                var xmlVersionTag = XElement.Load(csprojFile.FullName)
                    .Descendants()
                    .FirstOrDefault(e =>
                        e.Name.ToString().ToLowerInvariant() == "version" &&
                        e.Parent?.Name.ToString().ToLowerInvariant() == "propertygroup");

                if (xmlVersionTag != null)
                {
                    return csprojFile.FullName;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the correctly-cased file path equivalent.
        /// </summary>
        /// <param name="filePath">File path for which to find the equivalent.</param>
        /// <returns>Correctly-cased file path.</returns>
        public static string GetFilePathCaseInsensitive(string filePath)
        {
            var absolutePath = Path.GetFullPath(filePath);
            var parentDirectory = Directory.GetParent(absolutePath)?.FullName ?? absolutePath;

            return Directory.GetFiles(parentDirectory).FirstOrDefault(fp => string.Equals(fp, absolutePath, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}