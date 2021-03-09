# Introduction 
`Changelog.DevOpsTask` is a project that contains the extension developed for the Azure DevOps containing one task called **Merge Changelog**. Task to merge changes into a changelog file.

# Pre-requisites for the task
The following pre-requisites need to be fulfilled in order for the task to work properly:

### **Changes folder**

Automation agent where the task executes needs to have a **changes** folder containing files created with our helper applications [`Changelog.VSIX`](../Enterwell.CI.Changelog.VSIX) and/or [`Changelog.CLI`](../Enterwell.CI.Changelog.CLI). If the folder does not exist, task will throw an error.

### **Changelog file**

The task also needs to be able to find `CHANGELOG.md` file (naming is case-insensitive) in order to not throw and stop executing.

# Task inputs
Task takes four inputs:
+ semantic version (required):
  + can be a string (ex. major.minor.patch) or an environmental variable (ex. `$(semanticVersion)`).
+ directory location containing the `CHANGELOG.md` file (optional):
  + default value is `$(Build.SourcesDirectory)`.
+ boolean representing that the 'changes' directory exists in a different location than the `CHANGELOG.md` file (optional):
  + default value is `false`.
+ **changes** folder location that contains all of the changes to be compiled into the `CHANGELOG.md` (required if previous boolean is set to `true`):
  + if the boolean is set to `false` changes location is set to `<location containing the CHANGELOG.md>\changes`.

# Tasks assistant menu
After installing the extension from the Marketplace, you can find the task in the assistant menu when editing pipeline `.yml` file.

![](../img/DevOpsTask.png)

# YAML pipeline task definition
Example of a task call in `.yml` pipeline file:

> task: MergeChangelog@<version_identifier> \
> inputs: \
> &nbsp; &nbsp; semanticVersion: <major.minor.patch> | environmental variable \
> &nbsp; &nbsp; changelogLocation: path | environmental variable \
> &nbsp; &nbsp; changesInDifferentLocation: boolean \
> &nbsp; &nbsp; changesLocation: path | environmental variable

# Result / Output
During the task execution, before and after doing changes to the `CHANGELOG.md` file, the task prints out all of the files found at the locations passed to the task and the `CHANGELOG.md` contents, for debugging purposes.

If **changes** directory or `CHANGELOG.md` file does not exist at respected locations, task will log an error to the pipeline job output and set its status to **FAILED**.

Otherwise, task executes [Changelog application](../Enterwell.CI.Changelog) which inserts appropriate section to the `CHANGELOG.md` file and deletes all the contents of the **changes** folder.

# Development
In order to be able to run this code and its tests on your machine, you need to:

1. Position yourself into the **task** directory with `cd task`.
2. Run `npm install` to install all the dependencies used in the project.
3. Run `tsc` or `npx tsc` in order for Typescript to translate all the `.ts` files to the `.js` files.
4. Run the available npm script for running [Mocha](https://mochajs.org/) tests with `npm test`.