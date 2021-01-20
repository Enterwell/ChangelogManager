using System;
using System.IO;

namespace Enterwell.CI.Changelog.Tests
{
    public class TestBase : IDisposable
    {
        protected readonly string TemporaryDirectoryPath = Path.GetTempPath();
        protected readonly string TestFolderName = "Testing";
        protected readonly string ChangesFolderName = "changes";
        

        protected readonly string TestFolderPath;
        protected readonly string ChangesFolderPath;

        public TestBase()
        {
            TestFolderPath = Path.Combine(TemporaryDirectoryPath, TestFolderName);
            ChangesFolderPath = Path.Combine(TestFolderPath, ChangesFolderName);

            Directory.CreateDirectory(TestFolderPath);
        }

        protected void CreateDirectory(string folderPath)
        {
            if (!folderPath.StartsWith(TestFolderPath))
            {
                throw new Exception("Cannot create directories on given path!");
            }

            Directory.CreateDirectory(folderPath);
        }

        protected void CreateFile(string filePath)
        {
            if (!filePath.StartsWith(TestFolderPath))
            {
                throw new Exception("Cannot create files on given path!");
            }

            var file = File.Create(filePath);
            file.Close();
        }

        protected void WriteToFile(string filePath, string text)
        {
            File.WriteAllText(filePath, text);
        }

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

        public void Dispose()
        {
            Directory.Delete(TestFolderPath, true);
        }
    }
}