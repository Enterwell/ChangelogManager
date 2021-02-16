# Introduction 

This repository containes:
+ `Changelog` Console App project using .NET 5 that uses files in the **changes** folder in the solution root to fill out the `CHANGELOG.md` file.

+ `Changelog.Tests` project that contains [xUnit.net](https://xunit.net/) tests for the `Changelog`.

+ `Changelog.VSIX` VSIX project that is a Visual Studio Extension which helps developers automatically create needed files from the IDE itself.

+ `Changelog.CLI` Console App project using .NET 5 which servers as a CLI version of VSIX project, helping developers automatically create needed files from the command line / terminal.

+ `Changelog.Shared` class library project which holds all the shared logic between `Changelog.CLI` and `Changelog.VSIX` projects that helps with file creation and checking for configuration.