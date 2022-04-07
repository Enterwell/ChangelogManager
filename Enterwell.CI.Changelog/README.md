# Introduction 

`Changelog` is a Console App project using .NET 6 that uses files in the **changes** folder to fill out the `CHANGELOG.md` file.

Application takes two inputs:
+ directory location containing the `CHANGELOG.md` and possibly configuration file and
+ **changes** directory location;

Application automatically determines the next application's [semantic version](https://semver.org/) based on the files generated (either by hand, our [CLI application](../Enterwell.CI.Changelog.CLI/README.md) or our [Visual Studio Extension](../Enterwell.CI.Changelog.VSIX/README.md)) in the **changes** using the [default bumping rule](#default-bumping-rule). Then it builds a section representing the determined next semantic version containing the changes from the **changes** folder. The section is inserted above the latest changelog entry and its heading is formatted as:

```
[<semantic_version>] - yyyy-MM-dd
```

*Important note*. As it currently stands, the application is inserting the newly compiled section above the latest changelog entry. So, the `CHANGELOG.md` file needs to contain **atleast** 
```
## [Unreleased]
```

## Table of contents

+ [How to use this?](#how-to-use-this)
+ [What are the changes in the changes directory?](#what-are-the-changes-in-the-changes-directory)
+ [Configuration file](#configuration-file)
  + [Default bumping rule](#default-bumping-rule)
+ [Result / Output](#result--output)

## How to use this?

Firstly, you can use our [GitHub Releases](https://github.com/Enterwell/ChangelogManager/releases/) tab to start using our tool. Link contains pre-built binaries for Windows and Linux, as well as VS extension which is also available on the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=Enterwell.EnterwellChangelogVsix).

For example, let's say that your system directory structure looks something like this:

```
C
├── Projects                
│   ├── ExampleProject          
│   │   ├── ...
│   │   ├── ExampleProject.Application
│   │   ├── ExampleProject.Domain
│   │   ├── ExampleProject.API
│   │   ├── changes (directory containing the generated change files)
│   │   │   └── ...
│   │   ├── README.md
│   │   ├── CHANGELOG.md
│   │   └── .changelog.json (optional configuration file)
│   └── ...
└── ...
```

As said in the previous section, application takes two inputs:
+ directory location with the `CHANGELOG.md` and possibly the configuration file
  + in this case `C:/Projects/ExampleProject`
+ **changes** directory location
  + in this case `C:/Projects/ExampleProject/changes`

You would then call the application as following:

```
clm "C:/Projects/ExampleProject" "C:/Projects/ExampleProject/changes"
```

## What are the changes in the changes directory?

Changes in the **changes** folder are just files with the following naming scheme:

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

To avoid incorrect naming and to ease this file creation process on the developer, [Visual Studio Extension](../Enterwell.CI.Changelog.VSIX) and [CLI](../Enterwell.CI.Changelog.CLI) were made.

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

If the configuration exists, application will ignore every change in the **changes** folder that does not concur to it. On the other hand, if the configuration file does not exist, every change will be accepted and written to the `CHANGELOG.md`.

### Default bumping rule

Another important feature that can be configured and overwritten by using the configuration file is the default bumping rule.

Default rules that apply when determining the next application's semantic version:

| Changes folder contains the following change type  | Bump major | Bump minor | Bump patch |
|---|---|---|---|
| Deprecated   | ✅ | ❌ | ❌ |
| Added  | ❌ | ✅ | ❌  |
| Changed  | ❌ | ✅ | ❌  |
| Removed  | ❌ | ✅ | ❌  |
| (no changes)  | ❌  | ✅  | ❌  |
| Fixed  | ❌ | ❌ | ✅ |
| Security  | ❌ | ❌ | ✅ |

You are free to overwrite these defaults by using the configuration file and updating it in the following way:

```json
{
  "categories": [
    "API",
    "FE",
    "BE"
  ],
  "bumpingRule": {
        "major": [
            "Deprecated"
        ],
        "minor": [
            "Added",
            "Changed",
            "Removed",
            "NoChanges"
        ],
        "patch": [
            "Fixed",
            "Security"
        ]
    }
}
```

This `bumpingRule` configuration is equivalent to the default rule.

*Important note*. The naming is **case-sensitive**.

## Result / Output

If the **changes** directory or `CHANGELOG.md` file does not exist, application will log the error to the Console Error output and stop with the execution.

Otherwise, appropriate section will be inserted in the `CHANGELOG.md` file and the **changes** folder will be cleared empty to be ready for the next iteration.
