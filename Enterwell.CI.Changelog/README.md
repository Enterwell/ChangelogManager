# Changelog Manager CLI Tool

`Changelog` is a helper CLI Console App project using .NET 6 that uses files describing changes made to fill out the `CHANGELOG.md` file.

Helper automatically determines the next application's [semantic version](https://semver.org/) based on the current `CHANGELOG.md` file and the files generated (either by hand, our [CLI helper](../Enterwell.CI.Changelog.CLI/README.md) or our [Visual Studio extension](../Enterwell.CI.Changelog.VSIX/README.md)) in the **changes** directory using the [default bumping rule](#default-bumping-rule). Then it builds a section representing the determined next semantic version containing the changes from the **changes** directory. The section is inserted above the latest changelog entry and its heading is formatted as:

```
[<semantic_version>] - yyyy-MM-dd
```

*Important note*. As it currently stands, the helper is inserting the newly compiled section above the latest changelog entry. So, the `CHANGELOG.md` file needs to contain **atleast** 
```
## [Unreleased]
```

If you want to manage `CHANGELOG.md` automatically using the [Azure Pipelines](https://azure.microsoft.com/en-us/services/devops/pipelines/) instead of running this helper tool manually, check out our [Azure DevOps extension](..\Enterwell.CI.Changelog.DevOpsExtension)!

## Table of contents

+ [How to use this?](#how-to-use-this)
  + [Example](#example)
+ [What are the changes in the changes directory?](#what-are-the-changes-in-the-changes-directory)
+ [Configuration file](#configuration-file)
  + [Default bumping rule](#default-bumping-rule)
+ [Result / Output](#result--output)

## How to use this?

Firstly, you can use our [GitHub Releases](https://github.com/Enterwell/ChangelogManager/releases/) tab to start using our tool. Link contains pre-built binaries for Windows and Linux, as well as [Visual Studio extension](../Enterwell.CI.Changelog.VSIX) which is also available on the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=Enterwell.EnterwellChangelogVsix).

In general, the helper takes two arguments and one optional input:

  + changelog location - required
  + changes location - required
  + set version flag with the optional project file path - optional (prefixed with either `-sv` or `--set-version`)

Example of a call:
```
clm [options] <changelog_location> <changes_location>
```

Arguments:
+ changelog_location:
  + Directory location containing the `CHANGELOG.md`, and possibly, the configuration `.changelog.json` file
  + Can be either an absolute or a relative path
+ changes_location
  + Directory location containing the changes.
  + Can be either an absolute or a relative path

Options:
+ -v | --version:
  + Shows version information.
+ -sv | --set-version[:<PROJECT_FILE_PATH>]:
  + Should the new application's version be set in the appropriate project file.
  + If set without a file path, the helper will try to automatically determine the project file.
  + Currently supported project types:
    + NPM (`package.json`)
    + .NET SDK (`.csproj` with the `Version` (case-insensitive) tag)
  + Can be either an absolute or a relative path
+ -? | -h | --help:
  + Shows help information.

### Example 

Let's say that your system directory structure looks something like this:

```
C
├── Projects                
│   ├── ExampleProject          
│   │   ├── ...
│   │   ├── ExampleProject.Application
|   |   |   ├── ...
|   |   |   └── ExampleProject.Application.csproj (contains the application's 'Version' (case-insensitive) tag)
│   │   ├── ExampleProject.Domain
│   │   ├── ExampleProject.API
│   │   ├── changes (directory containing the generated change files)
│   │   │   └── ...
│   │   ├── README.md
│   │   ├── package.json (contains the application's version information)
│   │   ├── CHANGELOG.md
│   │   └── .changelog.json (optional configuration file)
│   └── ...
└── ...
```

As previously said, the helper takes two arguments and one optional input:
+ directory location containing the `CHANGELOG.md`, and possibly, the configuration `.changelog.json` file
  + in this case `C:\Projects\ExampleProject`
+ directory location containing the changes
  + in this case `C:\Projects\ExampleProject\changes`

If you want to opt-out of automatically bumping the project file, you would call the helper as follows:

```
clm C:\Projects\ExampleProject C:\Projects\ExampleProject\changes
```

If you want to opt-in to automatically bumping the project file, you would call the helper as follows:

```
clm C:\Projects\ExampleProject C:\Projects\ExampleProject\changes --set-version (or -sv)
```
  + the helper will try to look for the correct project file in the same directory where the helper is running. In this case it will find the `package.json` and set its `version` key value to the new application's version.

If you want to explicitly pass the path to the project file that needs to be bumped, you would call the helper as follows:

```
clm C:\Projects\ExampleProject C:\Projects\ExampleProject\changes -sv:C:\Projects\ExampleProject\ExampleProject.Application\ExampleProject.Application.csproj
```
  + in this case the helper will find the `ExampleProject.Application.csproj` and set its `Version` (case-insensitive) tag value to the new application's version.

As mentioned earlier, you could have used relative paths instead of an absolute ones in each of these previous helper calls. Assuming you are positioned inside `C:\Projects\ExampleProject`, the last call where the project file was explicitly passed in would look like:

```
clm . .\changes -sv:.\ExampleProject.Application\ExampleProject.Application.csproj
```

## What are the changes in the changes directory?

Changes in the **changes** directory are just files with the following naming scheme:

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

`[<change_category>]` part of the naming is controlled and validated using the `.changelog.json` configuration file.

To avoid incorrect naming and to ease this file creation process on the developer, [Visual Studio extension](../Enterwell.CI.Changelog.VSIX) and the [CLI helper](../Enterwell.CI.Changelog.CLI) were made.

## Configuration file
`.changelog.json` is a [JSON file](https://www.json.org/json-en.html) that is optional. Configuration specifies which change categories are allowed in your project. File needs to be located in the same directory alongside the appropriate `CHANGELOG.md` file.

If we wanted to allow only 3 different change categories: `API`, `FE` (Frontend) and `BE` (Backend), the configuration would look like:

```json
{
  "categories": [
    "API",
    "FE",
    "BE"
  ]
}
```

If the configuration exists, helper will ignore every change in the **changes** directory that does not concur to it. On the other hand, if the configuration file does not exist, every change will be accepted and written to the `CHANGELOG.md`.

### Default bumping rule

Another important feature that can be configured and overwritten by using the configuration file is the default bumping rule.

Default rules that apply when determining the next application's semantic version:

| Changes directory contains the following change type  | Bump major | Bump minor | Bump patch |
|---|---|---|---|
| Deprecated   | ❌ | ✅ | ❌ |
| Added  | ❌ | ✅ | ❌  |
| Changed  | ❌ | ✅ | ❌  |
| Removed  | ❌ | ✅ | ❌  |
| (no changes)  | ❌  | ✅  | ❌  |
| Fixed  | ❌ | ❌ | ✅ |
| Security  | ❌ | ❌ | ✅ |
| Change description containing words `BREAKING CHANGE` (case-insensitive)  | ✅ | ❌ | ❌ |

You are free to overwrite these defaults by using the configuration file and updating it in the following way:

```json
{
  "categories": [
    "API",
    "FE",
    "BE"
  ],
  "bumpingRule": {
    "major": [],
    "minor": [
      "Added",
      "Changed",
      "Deprecated"
      "Removed",
      "NoChanges"
    ],
    "patch": [
      "Fixed",
      "Security"
    ],
    "breakingKeyword": "BREAKING CHANGE"  // this is case-insensitive
  }
}
```

This `bumpingRule` configuration is equivalent to the default rule.

*Important note*. The naming is **case-sensitive**.

## Result / Output

If the **changes** directory or `CHANGELOG.md` file does not exist, application will log the error to the Console Error output (*stderr*) and stop with the execution.

Otherwise, appropriate section will be inserted in the `CHANGELOG.md` file and the change files that were used to build the new changelog section from the **changes** directory will be deleted.

Application logs the newly bumped semantic version to the Console Standard output (*stdout*).

## Development

### Publishing

```bash
dotnet publish -c release -r win-x64 -p:PublishSingleFile=true
dotnet publish -c release -r linux-x64 -p:PublishSingleFile=true
```
