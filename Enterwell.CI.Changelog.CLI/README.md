<div align="center">
  <a style="display: inline-block;" href="https://enterwell.net" target="_blank">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://enterwell.net/wp-content/uploads/2023/05/ew-logomark-color-negative-128.x48680.png">
      <img width="128" height="128" alt="logo" src="https://enterwell.net/wp-content/uploads/2023/05/ew-logomark-color-positive-128.x48680.png">
    </picture>
  </a>
  
  <h1>Changelog Create CLI Tool</h1>

  <p>This extension helps developers make easy notes on changes made to their codebase.</p>
</div>

## 🌱 Introduction
This is the *CLI* alternative to a [Visual Studio](../Enterwell.CI.Changelog.VSIX) and [Visual Studio Code](../Enterwell.CI.Changelog.VSCodeExtension) extensions developed for the convenience of creating special *change* files that are used to manage a [changelog](https://keepachangelog.com/en/1.1.0/).

#### What are the *change* files? 🤔

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

To avoid incorrect file naming and to ease file creation process on the developer, this CLI tool was made for general usage alongside our other helpers:
 + [Visual Studio extension](../Enterwell.CI.Changelog.VSIX)
 + [Visual Studio Code extension](../Enterwell.CI.Changelog.VSCodeExtension)

These files are then used with our [Changelog Manager tool](https://github.com/Enterwell/ChangelogManager/tree/main/Enterwell.CI.Changelog) to compile changes and insert a new changelog sections and thus versioning the application. 

**For the convenience of using this tool to manage a changelog in an automated CI/CD environment we made a [GitHub Action](https://github.com/Enterwell/ChangelogManager-GitHub-Action) and an [Azure DevOps extension](https://github.com/Enterwell/ChangelogManager/tree/main/Enterwell.CI.Changelog.DevOpsExtension).**

*We **highly** recommend that you read up on how and what exactly is it doing behind the scenes, as well as, learn how to use the `.changelog.json` configuration file to customize the tool's behaviour.*


## 📖 Table of contents
+ 🌱 [Introduction](#-introduction)
+ 📝 [Usage](#-usage)
+ ⚙️ [Configuration file](#-configuration-file)
+ 🏗 [Development](#-development)
+ ☎️ [Support](#-support)
+ 🪪 [License](#-license)

## 📝 Usage

Firstly, you can use our [GitHub Releases](https://github.com/Enterwell/ChangelogManager/releases/) tab to start using this CLI. Link contains pre-built binaries for Windows and Linux.

You can run the binary anywhere in the target project's workspace directory.

If the helper is used in the directory that doesn't contain ***changes*** directory to create the change file in, it will automatically search current working directory tree upwards for the nearest **changes** directory.

The helper takes two arguments and one optional input:
+ change type - required
+ change description - required
+ change category - optional (prefixed with either `-c` or `--category`)

Example of a call:

```
cc [options] <change_type> <change_description>
```

Arguments:
+ change_type: 
   + Change type following the ['Keep a Changelog' guiding principles](https://keepachangelog.com/en/1.0.0/#how).
   + Acceptable entries for the `<change_type>` are:
      + Added
      + Changed
      + Deprecated
      + Removed
      + Fixed
      + Security
      + each of the previous entries can also be specified using only their first letters
   + Acceptable entries are case-insensitive.
+ change_description:
   + Change description that describes the changes made (if the text is longer than a single word, it needs to be quoted).
   + Excess whitespace will be removed.

Options:
+ -v | --version:
   + Shows version information.
+ -c | --category <change_category>:
   + One of the valid change categories (case-insensitive) defined in the [configuration file](#-configuration-file) if it exists.
   + Arbitrary if the configuration does not exist or is empty.
      + Can be empty.
      + Excess whitespace will be removed.
   + Needs to be quoted if its longer than a single word.
+ -? | -h | --help:
   + Shows help information.

Changes in the **changes** directory are just files with the following naming scheme:

```
<change_type> [<change_category>] <change_description>
```

`[<change_category>]` part of the naming is controlled and validated using the `.changelog.json` configuration file.

And that's it! You should now have a new file created inside the **changes** folder in your workspace directory! 🎉

You can now let this folder accumulate change entries which will be bundled up into your `CHANGELOG.md` when you want to create a new application release using our [Changelog Manager tool](../Enterwell.CI.Changelog) or one of our CI/CD tasks mentioned in the [introduction section](#-introduction).

## ⚙ Configuration file
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

If the configuration exists, helper will deny the creation of change files which ignore every change in the **changes** directory that does not concur to it. On the other hand, if the configuration file does not exist, every change will be accepted and written to the `CHANGELOG.md`.

For more features that can be configured using the configuration file, see the [Changelog Manager's README](../Enterwell.CI.Changelog/README.md/#configuration-file).

## 🏗 Development

### Publishing

```bash
dotnet publish -c release -r win-x64
dotnet publish -c release -r linux-x64
```

## ☎ Support
If you are having problems, please let us know by [raising a new issue](https://github.com/Enterwell/ChangelogManager/issues/new?title=[CC]).

## 🪪 License
This project is licensed with the [MIT License](../LICENSE).