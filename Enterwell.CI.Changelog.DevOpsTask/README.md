# Introduction 
`Changelog.DevOpsTask` is a project that contains the extension developed for the Azure DevOps containing one task called **Merge Changelog**. Task to merge changes into a changelog file.

# Pre-requisites for the task
The following pre-requisites need to be fulfilled in order for the task to work properly:

### **Changes folder**

Repository or the directory on the automation agent where the task executes needs to have a **changes** folder containing files created with our helper applications [`Changelog.VSIX`](../Enterwell.CI.Changelog.VSIX) and/or [`Changelog.CLI`](../Enterwell.CI.Changelog.CLI). If the folder does not exist, task will throw an error.

### **Changelog file**

The task also needs to be able to find `Changelog.md` file (naming is case-insensitive) in the root directory in order to not throw and stop executing.

# Task inputs
Task takes two inputs:
+ semantic version:
  + can be a string (ex. major.minor.patch) or an environmental variable (ex. $(semanticVersion)).
+ the directory root/location with the **changes** folder and the `CHANGELOG.md` file itself:
  + default value is $(Build.SourcesDirectory).

# YAML pipeline task definition
Example of a task call in `.yml` pipeline file:

> task: MergeChangelog@<version_identifier> \
> inputs: \
> &nbsp; &nbsp; semanticVersion: <major.minor.patch> | environmental variable \
> &nbsp; &nbsp; repositoryLocation: path | environmental variable

# Result / Output
If **changes** directory or `CHANGELOG.md` file does not exist in the directory, task will log the error to the pipeline output and set its status to **FAILED**.

Otherwise, task executes [`our Changelog application`](../Enterwell.CI.Changelog) which inserts appropriate section in the `CHANGELOG.md` file and delets all the contents of the **changes** folder.

# Development
In order to be able to run this code and its tests on your machine, you need to:

1. Position yourself into the **task** directory with `cd task`.
2. Run `npm install` to install all the dependencies used in the project.
3. Run `tsc` or `npx tsc` in order for Typescript to translate all the `.ts` files to the `.js` files.
4. Run the available npm script for running [Mocha](https://mochajs.org/) tests with `npm test`.