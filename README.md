# Introduction

This repository containes:
+ [`Changelog`](Enterwell.CI.Changelog) Console App project using .NET 6. Changelog Manager (CLI for merging changes to the `CHANGELOG.md`).

+ [`Changelog.CLI`](Enterwell.CI.Changelog.CLI) Console App project using .NET 6. Changelog Create (CLI for creating change entries).

+ [`Changelog.VSIX`](Enterwell.CI.Changelog.VSIX) VSIX project. Changelog Create extension for Visual Studio.

+ [`Changelog.DevOpsExtension`](Enterwell.CI.Changelog.DevOpsExtension) project that contains the extension developed for the Azure DevOps containing **Merge Changelog** task. The task to merge files describing changes made into a changelog file. Changelog Manager extension for Azure DevOps.

+ [`Changelog.Shared`](Enterwell.CI.Changelog.Shared) class library project which holds all the shared logic between [`Changelog.CLI`](Enterwell.CI.Changelog.CLI) and [`Changelog.VSIX`](Enterwell.CI.Changelog.VSIX) projects.

+ [`Changelog.Tests`](Enterwell.CI.Changelog.Tests) project that contains [xUnit.net](https://xunit.net/) tests for the [`Changelog`](Enterwell.CI.Changelog).