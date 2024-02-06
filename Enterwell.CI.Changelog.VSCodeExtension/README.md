<h1 align="center">
  <a style="display: inline-block;" href="https://enterwell.net" target="_blank">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://enterwell.net/wp-content/uploads/2023/05/ew-logomark-color-negative-128.x48680.png">
      <img width="128" height="128" alt="logo" src="https://enterwell.net/wp-content/uploads/2023/05/ew-logomark-color-positive-128.x48680.png">
    </picture>
  </a>
  
  <p>Changelog Create extension for Visual Studio Code</p>
</h1>

<div align="center">
  <p>This extension helps developers make easy notes on changes made to their codebase.</p>
</div>

## 🌱 Introduction
This is the *Visual Studio Code* version of an [Visual Studio extension](https://github.com/Enterwell/ChangelogManager/tree/main/Enterwell.CI.Changelog.VSIX) developed for the convenience of creating special *change* files that are used to manage a [changelog](https://keepachangelog.com/en/1.1.0/).

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

To avoid incorrect file naming and to ease file creation process on the developer, this extension was made for Visual Studio Code alongside our other helpers:
 + [Visual Studio extension](https://github.com/Enterwell/ChangelogManager/tree/main/Enterwell.CI.Changelog.VSIX)
 + [CLI helper](https://github.com/Enterwell/ChangelogManager/tree/main/Enterwell.CI.Changelog.CLI)

These files are then used with our [Changelog Manager tool](https://github.com/Enterwell/ChangelogManager/tree/main/Enterwell.CI.Changelog) to compile changes and insert a new changelog sections and thus versioning the application.

*We **highly** recommend that you read up on how and what exactly is it doing behind the scenes, as well as, learn how to use the `.changelog.json` configuration file to customize the tool's behaviour.*

For the convenience of using this tool to manage a changelog in an automated CI/CD environments we made a [GitHub Action](https://github.com/Enterwell/ChangelogManager-GitHub-Action) and an [Azure DevOps extension](https://github.com/Enterwell/ChangelogManager/tree/main/Enterwell.CI.Changelog.DevOpsExtension).

## 📖 Table of contents
+ 🌱 [Introduction](#-introduction)
+ 🛠️ [Prerequisites](#-prerequisites)
+ 📝 [Usage](#-usage)
+ ⚙️ [Configuration file](#-configuration-file)
+ ☎️ [Support](#-support)
+ 🪪 [License](#-license)

## 🛠 Prerequisities

Simple, just [Visual Studio Code](https://code.visualstudio.com/) editor 🎉

## 📝 Usage

You can trigger the extension by using the `ALT + C` shortcut or by using the command palette (`Ctrl + Shift + P`) and searching for the command

![Image showing the command in the command palette](https://github.com/Enterwell/ChangelogManager/blob/main/Enterwell.CI.Changelog.VSCodeExtension/assets/commandPalette.png)

In the first step, user will be prompted to choose a type of the change he is documenting

![Image showing the change type prompt](https://github.com/Enterwell/ChangelogManager/blob/main/Enterwell.CI.Changelog.VSCodeExtension/assets/changeType.png)

Based on the users workspace one of the two things will happen next:

+ If there are more than 1 workspaces open in the editor, user will be asked to choose a workspace he would like to create a change file for

  ![Image showing the workspace prompt](https://github.com/Enterwell/ChangelogManager/blob/main/Enterwell.CI.Changelog.VSCodeExtension/assets/workspacePicker.png)

+ If the user has only 1 workspace open, the extension will take the current workspace folder as the working directory and proceed to the next step

In the next step, one of the two things will happen next depending on if the user has defined a `.changelog.json` [configuration file](#-configuration-file):

+ If the `.changelog.json` exists with the categories listed inside, the user will be asked to choose one of the categories for the change

  ![Image showing the change category picker prompt](https://github.com/Enterwell/ChangelogManager/blob/main/Enterwell.CI.Changelog.VSCodeExtension/assets/changeCategory.png)

+ If the `.changelog.json` does **not** exist or there are no explicit categories defined inside

    + input can be any empty value and all excess whitespace will be removed

  ![Image showing the change category input](https://github.com/Enterwell/ChangelogManager/blob/main/Enterwell.CI.Changelog.VSCodeExtension/assets/changeCategoryInput.png)

And in the final step, the user is prompted for a change description which is validated for empty and whitespace input

  ![Image showing the change description input](https://github.com/Enterwell/ChangelogManager/blob/main/Enterwell.CI.Changelog.VSCodeExtension/assets/changeDescription.png)

And that's it! You should now have a new file created inside the **changes** folder in your workspace directory! 🎉

You can now let this folder accumulate change entries which will be bundled up into your `CHANGELOG.md` when you want to create a new application release using one of our tools mentioned in the [introduction section](#-introduction).

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

If the configuration exists, user will be able to choose only one of the categories provided as the change category. On the other hand, if the configuration file does not exist or the categories list is empty, user will be shown a text input and everything  will be able to qualify as the change category.

For more features that can be configured using the configuration file, see the [Changelog Manager's README](https://github.com/Enterwell/ChangelogManager/blob/main/Enterwell.CI.Changelog/README.md/#configuration-file).

## ☎ Support
If you are having problems, please let us know by [raising a new issue](https://github.com/Enterwell/ChangelogManager/issues/new?title=[VSCodeExtension]).

## 🪪 License
This project is licensed with the [MIT License](LICENSE).