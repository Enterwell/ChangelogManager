# Changelog Manager

This repository containes:
+ [`Changelog`](Enterwell.CI.Changelog) Console App project using .NET 6 that uses files in the **changes** folder in the solution root to fill out the `CHANGELOG.md` file.

+ [`Changelog.Tests`](Enterwell.CI.Changelog.Tests) project that contains [xUnit.net](https://xunit.net/) tests for the [`Changelog`](Enterwell.CI.Changelog).

+ [`Changelog.VSIX`](Enterwell.CI.Changelog.VSIX) VSIX project that is a Visual Studio Extension which helps developers automatically create needed files from the IDE itself.

+ [`Changelog.CLI`](Enterwell.CI.Changelog.CLI) Console App project using .NET 6 which servers as a CLI version of VSIX project, helping developers automatically create needed files from the command line / terminal.

+ [`Changelog.Shared`](Enterwell.CI.Changelog.Shared) class library project which holds all the shared logic between [`Changelog.CLI`](Enterwell.CI.Changelog.CLI) and [`Changelog.VSIX`](Enterwell.CI.Changelog.VSIX) projects that helps with file creation and checking for configuration.

+ [`Changelog.DevOpsTask`](Enterwell.CI.Changelog.DevOpsTask) project that contains the extension developed for the Azure DevOps containing one task. Task to merge changes into a changelog file.