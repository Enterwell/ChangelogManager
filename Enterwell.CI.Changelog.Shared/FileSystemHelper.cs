using System;
using System.IO;

namespace Enterwell.CI.Changelog.Shared
{
    /// <summary>
    /// Helper class for everything related to files and directories.
    /// </summary>
    public static class FileSystemHelper
    {
        public static string ChangeDirectoryName = "changes";

        /// <summary>
        /// Used to ensure that the changes directory exists. If it does not exist, the directory is created.
        /// </summary>
        public static void EnsureChangesDirectoryExists(string directoryPath)
        {
            var changesDirectoryPath = Path.Combine(directoryPath, ChangeDirectoryName);

            if (!Directory.Exists(changesDirectoryPath))
            {
                Directory.CreateDirectory(changesDirectoryPath);
            }
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

            if (string.IsNullOrWhiteSpace(inputCategory))
            {
                return $"{inputType} {description}";
            }
            else
            {
                var category = inputCategory.Trim();

                return $"{inputType} [{category}] {description}";
            }
        }
    }
}