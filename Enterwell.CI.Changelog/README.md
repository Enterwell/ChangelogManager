# Introduction 
`Changelog` is a Console App project using .NET 5 that uses files in the **changes** folder to fill out the `CHANGELOG.md` file.

Application takes three inputs: 
+ semantic version, 
+ directory location containing the `CHANGELOG.md` and possibly configuration file and
+ **changes** directory location;

Then it builds a section representing the semantic version user passed in as an input containing the changes contained in the **changes** folder. The section is inserted above the latest changelog entry and its heading is formatted as:

```
[<semantic_version>] - yyyy-MM-dd
```

*Important note*. As it currently stands, the application is inserting the newly compiled section above the latest changelog entry. So, the `CHANGELOG.md` file needs to contain **atleast** 
```
## [Unreleased]
```

## How to use this?

Firstly, you can use our [GitHub Releases](https://github.com/Enterwell/ChangelogManager/releases/) tab to start using our tool. Link contains pre-built binaries for Windows and Linux, as well as VS extension which is also available on VS Marketplace.

For example, let's say that your system directory structure looks something like this:

```
C
├── Projects                
│   ├── ExampleProject          
│   │   ├── ...
│   │   ├── ExampleProject.Application
│   │   ├── ExampleProject.Domain
│   │   ├── ExampleProject.API
│   │   ├── changes
│   │   │   └── ...
│   │   ├── README.md
│   │   ├── CHANGELOG.md
│   │   └── .changelog.json
│   └── ...
└── ...
```

As said in the previous section, application takes three inputs:
+ semantic version
  + ex. 1.0.0
+ directory location with the `CHANGELOG.md` and possibly the configuration file
  + in this case `C:/Projects/ExampleProject`
+ **changes** directory location
  + in this case `C:/Projects/ExampleProject/changes`

You would then call the application as following:

```
clm 1.0.0 "C:/Projects/ExampleProject" "C:/Projects/ExampleProject/changes"
```

## What are the changes in the changes directory?

Changes in the **changes** folder are just files with the following naming scheme:

```
<change_type> <[change_category]> <change_description>
```

Acceptable entries for `<change_type>` are:

+ Added
+ Changed
+ Deprecated
+ Removed
+ Fixed
+ Security

This decision was inspired by following the [principles](https://keepachangelog.com/en/1.0.0/#how) for keeping a good changelog.

`<[change_category]>` part of the naming is controlled and validated using the `.changelog.json` configuration file.

To avoid incorrect naming and to ease this file creation process on the developer, [Visual Studio Extension](../Enterwell.CI.Changelog.VSIX) and [CLI](../Enterwell.CI.Changelog.CLI) were made.

## Configuration file
`.changelog.json` is a [JSON file](https://www.json.org/json-en.html) which is optional. Configuration specifies which change categories are allowed in your project. File needs to be located in the same directory alongside the appropriate `CHANGELOG.md` file.

If we wanted to allow only 3 different change categories: API, FE (Frontend) and BE (Backend), the configuration would look like:

```json
{
  "categories": [
    "API",
    "FE",
    "BE"
  ]
}
```

If the configuration exist, application will ignore every change in the **changes** folder which does not concur to it. On the other hand, if the configuration file does not exist, every change will be accepted and written to the `CHANGELOG.md`.

## Result / Output

If the **changes** directory or `CHANGELOG.md` file does not exist, application will log the error to the Console Error output and stop with the execution.

Otherwise, appropriate section will be inserted in the `CHANGELOG.md` file and the **changes** folder will be cleared empty to be ready for the next iteration.
