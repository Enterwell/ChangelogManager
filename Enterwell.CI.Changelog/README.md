<div align="center">
  <a style="display: inline-block;" href="https://enterwell.net" target="_blank">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://enterwell.net/wp-content/uploads/2023/05/ew-logomark-color-negative-128.x48680.png">
      <img width="128" height="128" alt="logo" src="https://enterwell.net/wp-content/uploads/2023/05/ew-logomark-color-positive-128.x48680.png">
    </picture>
  </a>
  
  <h1>Changelog Manager CLI Tool</h1>

  <p>Compiles the <i>change</i> files into CHANGELOG.md</p>
</div>

## 🌱 Introduction

`Changelog Manager` is a helper CLI tool that uses files (in the rest of the documentation called *change* files) describing changes to fill out the `CHANGELOG.md` file.

Tool automatically determines the next application's [semantic version](https://semver.org/) based on the current `CHANGELOG.md` file and the [*change* files](#what-are-these-change-files) generated in the **changes** directory.

These files can be generated in the following ways:

+ using our [CLI helper](../Enterwell.CI.Changelog.CLI/README.md)
+ using our [Visual Studio extension](../Enterwell.CI.Changelog.VSIX/README.md)
+ using our [Visual Studio Code extension](../Enterwell.CI.Changelog.VSCodeExtension/README.md)
+ manually. *This is not recommended as the user creating these can be prone to error*.

Tool then builds a section representing the newly determined semantic version containing the changes from these files. The section is inserted above the latest changelog entry and its heading is formatted as:

```
[<semantic_version>] - yyyy-MM-dd
```

*Important note*. As it currently stands, the helper is inserting the newly compiled section above the latest changelog entry. So, the `CHANGELOG.md` file needs to contain **atleast** 

```
_NOTE: This is an automatically generated file. Do not modify contents of this file manually._

## [Unreleased]
```

**For the convenience of using this tool to manage a changelog in an automated CI/CD environment we made a [GitHub Action](https://github.com/Enterwell/ChangelogManager-GitHub-Action) and an [Azure DevOps extension](https://github.com/Enterwell/ChangelogManager/tree/main/Enterwell.CI.Changelog.DevOpsExtension)! No need for running this manually!**

## 📖 Table of contents
+ 🌱 [Introduction](#-introduction)
+ 📝 [Usage](#-usage)
+ 🤔 [What are these *change* files?](#-what-are-these-change-files)
+ ⚙️ [Configuration file](#-configuration-file)
+ 📤 [Result / Output](#-result--output)
+ 🏗 [Development](#-development)
+ ☎️ [Support](#-support)
+ 🪪 [License](#-license)

## 📝 Usage

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

## 🤔 What are these *change* files?

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

`[<change_category>]` part of the naming is controlled and validated using the `.changelog.json` configuration file.

To avoid incorrect naming and to ease this file creation process on the developer, following helper tools were created:
+ [Visual Studio extension](../Enterwell.CI.Changelog.VSIX) 
+ [Visual Studio Code extension](../Enterwell.CI.Changelog.VSCodeExtension)
+ [CLI helper](../Enterwell.CI.Changelog.CLI)

## ⚙ Configuration file
`.changelog.json` is an optional [JSON file](https://www.json.org/json-en.html). It specifies which change categories are allowed in your project. File needs to be located in the same directory alongside the appropriate `CHANGELOG.md` file.

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

If the configuration exists, helper will ignore every change in the ***changes*** directory that does not concur to it. On the other hand, if the configuration file does not exist, every change will be accepted and written to the `CHANGELOG.md`.

### Default bumping rule

Another important feature that can be configured and overwritten by using the configuration file is the default bumping rule.

Default rules that apply when determining the application's next semantic version:

| Change type  | Bump major | Bump minor | Bump patch |
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
  // This bumping rule configuration is equivalent to the default behaviour.
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

*Important note*. The naming is **case-sensitive**.

## 📤 Result / Output

If the ***changes*** directory or the `CHANGELOG.md` file does not exist, application will log the error to the Console Error output (*stderr*) and stop with the execution.

Otherwise, appropriate section will be inserted in the `CHANGELOG.md` file and the change files that were used to build the new changelog section from the **changes** directory will be **deleted**.

Application logs the newly bumped semantic version to the Console Standard output (*stdout*).

## 🏗 Development

### Publishing

```bash
dotnet publish -c release -r win-x64 -p:PublishSingleFile=true
dotnet publish -c release -r linux-x64 -p:PublishSingleFile=true
```

## ☎ Support
If you are having problems, please let us know by [raising a new issue](https://github.com/Enterwell/ChangelogManager/issues/new?title=[CLM]).

## 🪪 License
This project is licensed with the [MIT License](../LICENSE).