<div align="center">
  <img width="128" height="128" src="images/icon.svg" alt="logo" />

  <h1>Changelog Manager Azure DevOps extension</h1>

  <p>Compiles the <i>change</i> files into CHANGELOG.md</p>
</div>

## 🌱 Introduction
This is the *Azure DevOps task* version of a [GitHub Action](https://github.com/Enterwell/ChangelogManager-GitHub-Action) developed for the convenience of managing a [changelog](https://keepachangelog.com/en/1.1.0/), indirectly, using special *change* files.

#### What are the *change* files?

*Change* files are just files located in the ***changes*** directory with the following naming scheme:

```
<change_type> [<change_category>] <change_description>
```

Acceptable entries for the `<change_type>` are:

+ Added
+ Changed
+ Deprecated
+ Removed
+ Fixed
+ Security

This decision was inspired by following the [principles](https://keepachangelog.com/en/1.0.0/#how) for keeping a good changelog.

To avoid incorrect naming and to ease this file creation process on the developer, following helper tools were created:
+ [CLI helper](../Enterwell.CI.Changelog.CLI)
+ [Visual Studio extension](../Enterwell.CI.Changelog.VSIX) 
+ [Visual Studio Code extension](../Enterwell.CI.Changelog.VSCodeExtension)

This *Action* internally calls our [Changelog Manager tool](https://github.com/Enterwell/ChangelogManager/tree/main/Enterwell.CI.Changelog) by forwarding action inputs to it, which then inserts the appropriate section to the `CHANGELOG.md` and deletes all the contents of the ***changes*** directory.
 
We **highly** recommend that you read up on how and what exactly is it doing behind the scenes, as well as, learn how to use the `.changelog.json` configuration file to customize the tool's behaviour.

## 📖 Table of contents
+ 🌱 [Introduction](#-introduction)
+ 🛠️ [Prerequisities](#-prerequisities)
+ 📝 [Usage](#-usage)
+ 📥 [Inputs](#-inputs)
+ 📤 [Outputs](#-outputs)
+ 🏗 [Development](#-development)
+ ☎️ [Support](#-support)
+ 🪪 [License](#-license)

## 🛠 Prerequisities

The following prerequisities need to be fulfilled in order for the action to work properly:

#### Changes directory

Automation agent where the action executes needs to have a ***changes*** directory containing the [*change*](#-what-are-the-change-files) files created manually or with one of our helpers. If the directory does not exist, action will throw an error.

#### Changelog file

The action also needs to be able to find `CHANGELOG.md` file (naming is case-insensitive), otherwise, it will throw and stop executing.

*Important note*. As it currently stands, the action is inserting the newly compiled section **above** the latest changelog entry. So, the `CHANGELOG.md` needs to contain **atleast**

```
_NOTE: This is an automatically generated file. Do not modify contents of this file manually._

## [Unreleased]
```

## 📝 Usage

### Basic
```yaml
task: MergeChangelog@3
```

### Advanced
```yaml
task: MergeChangelog@3
inputs:
  changelogLocation: ./
  changesInDifferentLocation: true
  changesLocation: ./somewhere-else/changes
  shouldBumpVersion: true
  pathToProjectFile: ./somewhere-else/[package.json | something.csproj]
```

### Tasks assistant menu

After installing the extension from the [Marketplace](https://marketplace.visualstudio.com/azuredevops), you can find the task in the assistant menu when editing pipeline `.yml` file.

![](../img/devOpsTask.png)


## 📥 Inputs

### `changelogLocation`
**Optional** Location of the directory containing the `CHANGELOG.md` file.
  + Defaults to `$(Build.SourcesDirectory)`

### `changesInDifferentLocation`
**Optional** Boolean representing that the *changes* directory exists in a different location than the `CHANGELOG.md` file.
  + Defaults to `false`

### `changesLocation`
**Optional** Location of the `changes` directory.
  + If the previous input is set to `false`, changes location is set to `<location containing the CHANGELOG.md>\changes`.

### `shouldBumpVersion`
**Optional** Boolean representing should the new version be set in the appropriate project file (`package.json` or `*.csproj`).
  + Defaults to `false`

### `pathToProjectFile`
**Optional** Path to the project file (`package.json` or `.csproj` with the `version` (case-insensitive) tag).
  + If the previous input is set to `true`, but this input is not passed in explicitly, the action will try to automatically determine the appropriate project file
  + If the previous input is set to `false`, this input is **ignored**

## 📤 Outputs

### `bumpedSemanticVersion`
Newly bumped semantic version based on the changes made.

### `bumpedMajorPart`
Major part of the newly bumped version.

### `bumpedMinorPart`
Minor part of the newly bumped version.

### `bumpedPatchPart`
Patch part of the newly bumped version.

### `newChanges`
Changes from the new changelog section.

### Usage

```yaml
# azure-pipelines.yml
...
  steps:
    ...
    - task: MergeChangelog@3
      name: MergeChangelog

    - script: echo $(MergeChangelog.bumpedSemanticVersion)
    ...
...
```

## 🏗 Development

In order to be able to run this code and its tests on your machine, you need to:

1. Position yourself into the **MergeChangelogTask** directory with `cd MergeChangelogTask`.
2. Position yourself into either `MergeChangelogTaskV1` or `MergeChangelogTaskV2` depending on which version of the task you want to develop in.
2. Run `npm install` to install all the dependencies used in the project.
3. Run `tsc` or `npx tsc` in order for *Typescript* to translate all the `.ts` files to the `.js` files.
4. Run the available npm script for running [Mocha](https://mochajs.org/) tests with `npm test`.

### Packaging extension for publish to Marketplace

Before packaging the extension for publishing, make sure that you installed all the dependencies for every task and that you translated all *Typescript* files into *Javascript* equivalents. For information on how to do that refer to the [previous](#-development) section.

In order to package extension for publishing, you need to use [Node CLI for Azure DevOps](https://github.com/microsoft/tfs-cli). You can install it by using `npm` by running `npm install -g tfx-cli`.

After having installed the CLI tool, you can follow the [Step 4 and Step 5](https://docs.microsoft.com/en-us/azure/devops/extend/develop/add-build-task?view=azure-devops#step-4-package-your-extension) of the official documentation on how to package and publish a custom task.

## ☎ Support
If you are having problems, please let us know by [raising a new issue](https://github.com/Enterwell/ChangelogManager/issues/new?title=[DevOpsExtension]).

## 🪪 License
This project is licensed with the [MIT License](../LICENSE).