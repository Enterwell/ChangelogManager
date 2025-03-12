<h1 align="center">
  <a style="display: inline-block;" href="https://enterwell.net" target="_blank">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://enterwell.net/wp-content/uploads/2023/05/ew-logomark-color-negative-128.x48680.png">
      <img width="128" height="128" alt="logo" src="https://enterwell.net/wp-content/uploads/2023/05/ew-logomark-color-positive-128.x48680.png">
    </picture>
  </a>
  <p>Changelog Manager</p>
</h1>

This repository contains:

- [`Changelog`](Enterwell.CI.Changelog) - Changelog Manager (CLI for merging changes to the `CHANGELOG.md`).
- [`Changelog.CLI`](Enterwell.CI.Changelog.CLI) - Changelog Create CLI.
- [`Changelog.VSIX`](Enterwell.CI.Changelog.VSIX) - Changelog Create extension for Visual Studio.
- [`Changelog.VSCodeExtension`](Enterwell.CI.Changelog.VSCodeExtension) - Changelog Create extension for Visual Studio Code.
- [`Changelog.DevOpsExtension`](Enterwell.CI.Changelog.DevOpsExtension) project that contains the extension developed for the Azure DevOps containing **Merge Changelog** task. The task to merge files describing changes made into a changelog file. 
- [`Changelog.Shared`](Enterwell.CI.Changelog.Shared) class library project which holds all the shared logic between [`Changelog.CLI`](Enterwell.CI.Changelog.CLI) and [`Changelog.VSIX`](Enterwell.CI.Changelog.VSIX) projects.
- [`Changelog.Tests`](Enterwell.CI.Changelog.Tests) project that contains [xUnit.net](https://xunit.net/) tests for the [`Changelog`](Enterwell.CI.Changelog).

Other related repositories:

- [`Changelog.GitHubAction`](https://github.com/Enterwell/ChangelogManager-GitHub-Action) project that contains the extension developed for the GitHub Actions containing **Merge Changelog** task. The task to merge files describing changes made into a changelog file. 
