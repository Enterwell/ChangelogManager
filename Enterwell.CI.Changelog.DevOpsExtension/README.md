# Changelog Manager extension for Azure DevOps

`Changelog.DevOpsExtension` is a project that contains the extension developed for the Azure DevOps containing one task called **Merge Changelog**. The task to merge files describing changes made into a changelog file.

## Table of contents

+ [Pre-requisites for the task](#pre-requisites-for-the-task)
  + [Changes directory](#changes-directory)
  + [Changelog file](#changelog-file)
+ [Task inputs](#task-inputs)
+ [Tasks assistant menu](#tasks-assistant-menu)
+ [YAML pipeline task definition](#yaml-pipeline-task-definition)
+ [Result / Output](#result--output)
+ [Development](#development)
+ [Packaging extension for publish to Marketplace](#packaging-extension-for-publish-to-marketplace)

## Pre-requisites for the task

The following pre-requisites need to be fulfilled in order for the task to work properly:

### **Changes directory**

Automation agent where the task executes needs to have a **changes** directory containing files created with our helper [`Visual Studio extension`](../Enterwell.CI.Changelog.VSIX) and/or [`CLI tool`](../Enterwell.CI.Changelog.CLI). If the directory does not exist, task will throw an error.

### **Changelog file**

The task also needs to be able to find `CHANGELOG.md` file (naming is case-insensitive) in order to not throw and stop executing.

*Important note*. As it currently stands, the application is inserting the newly compiled section above the latest changelog entry. So, the `CHANGELOG.md` file needs to contain **atleast**

```
## [Unreleased]
```

## MergeChangelog@3 task

New features:

+ changed input naming from `setVersionFlag` to `shouldBumpVersion`
+ task no longer updates major version if the `Deprecated` change type was added, rather if **any** change files contains `BREAKING CHANGE` message in case-insensitive form.
+ new outputs

Task takes five inputs:

+ directory location containing the `CHANGELOG.md` file (*optional*):
  + default value is `$(Build.SourcesDirectory)`.
+ boolean representing that the 'changes' directory exists in a different location than the `CHANGELOG.md` file (*optional*):
  + default value is `false`.
+ **changes** directory location that contains all of the changes to be compiled into the `CHANGELOG.md` (*required* if previous boolean is set to `true`):
  + if the previous boolean is set to `false` changes location is set to `<location containing the CHANGELOG.md>\changes`.
+ boolean representing if the new semantic version should be set in the appropriate project file (`package.json` or `*.csproj` file with the `Version` tag) (*optional*):
  + default value is `false`.
+ location of the project file (`package.json` or `.csproj` file with the `Version` (case-insensitive) tag) (*optional*):
  + if the previous boolean is set to `true`, but this input is not passed in explicitly, the task will try to automatically determine the appropriate project file
  + if the previous boolean is set to `false`, this input is ignored

### YAML pipeline task definition

Example of a task call in `.yml` pipeline file:

```yml
task: MergeChangelog@3
inputs:
  changelogLocation: path
  changesInDifferentLocation: boolean
  changesLocation: path
  shouldBumpVersion: boolean
  pathToProjectFile: path
```

### Result / Output

During the task execution, before and after doing changes to the `CHANGELOG.md` file, the task prints out all of the files found at the locations passed to the task and the `CHANGELOG.md` contents, for debugging purposes.

If the **changes** directory or `CHANGELOG.md` file does not exist at their respected locations, task will log an error to the pipeline job output and set its status to **FAILED**.

Otherwise, task executes the [Changelog Manager](../Enterwell.CI.Changelog) which inserts the appropriate section to the `CHANGELOG.md` file and deletes the change files that were used to build the new changelog section from the **changes** directory.

Finally, the task sets multiple new [outputs](https://docs.microsoft.com/en-us/azure/devops/pipelines/process/variables?view=azure-devops&tabs=yaml%2Cbatch#use-output-variables-from-tasks) which you can then consume in downstream steps, jobs and stages.



For example:

```yml
# azure-pipelines.yml
...
- task: MergeChangelog@2
  name: MergeChangelog
  
- script: echo $(MergeChangelog.bumpedFullVersion)
- script: echo $(MergeChangelog.bumpedMajorPart)
- script: echo $(MergeChangelog.bumpedMinorPart)
- script: echo $(MergeChangelog.bumpedPatchPart)
- script: echo $(MergeChangelog.newChanges)
...
```

## Tasks assistant menu

After installing the extension from the Marketplace, you can find the task in the assistant menu when editing pipeline `.yml` file.

![](../img/devOpsTask.png)

## Development

In order to be able to run this code and its tests on your machine, you need to:

1. Position yourself into the **MergeChangelogTask** directory with `cd MergeChangelogTask`.
2. Position yourself into either `MergeChangelogTaskV1` or `MergeChangelogTaskV2` depending on which version of the task you want to develop in.
2. Run `npm install` to install all the dependencies used in the project.
3. Run `tsc` or `npx tsc` in order for *Typescript* to translate all the `.ts` files to the `.js` files.
4. Run the available npm script for running [Mocha](https://mochajs.org/) tests with `npm test`.

## Packaging extension for publish to Marketplace

Before packaging the extension for publishing, make sure that you installed all the dependencies for every task and that you translated all *Typescript* files into *Javascript* equivalents. For information on how to do that refer to the [previous](#development) section.

In order to package extension for publishing, you need to use [Node CLI for Azure DevOps](https://github.com/microsoft/tfs-cli). You can install it by using `npm` by running `npm install -g tfx-cli`.

After having installed the CLI tool, you can follow the [Step 4 and Step 5](https://docs.microsoft.com/en-us/azure/devops/extend/develop/add-build-task?view=azure-devops#step-4-package-your-extension) of the official documentation on how to package and publish a custom task.
