<div align="center">
  <a style="display: inline-block;" href="https://enterwell.net" target="_blank">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://enterwell.net/wp-content/uploads/2023/05/ew-logomark-color-negative-128.x48680.png">
      <img width="128" height="128" alt="logo" src="https://enterwell.net/wp-content/uploads/2023/05/ew-logomark-color-positive-128.x48680.png">
    </picture>
  </a>
  
  <h1>Changelog Create extension for Visual Studio</h1>

  <p>This extension helps developers make easy notes on changes made to their codebase.</p>
</div>

## 🌱 Introduction
This is the *Visual Studio* alternative to a [Visual Studio Code extension](../Enterwell.CI.Changelog.VSCodeExtension/) developed for the convenience of creating special *change* files that are used to manage a [changelog](https://keepachangelog.com/en/1.1.0/).

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

To avoid incorrect file naming and to ease file creation process on the developer, this extension was made for Visual Studio alongside our other helpers:
 + [Visual Studio Code extension](../Enterwell.CI.Changelog.VSCodeExtension)
 + [CLI helper](../Enterwell.CI.Changelog.CLI)

These files are then used with our [Changelog Manager tool](../Enterwell.CI.Changelog) to compile changes and insert a new changelog sections and thus versioning the application.

**For the convenience of using this tool to manage a changelog in an automated CI/CD environment we made a [GitHub Action](https://github.com/Enterwell/ChangelogManager-GitHub-Action) and an [Azure DevOps extension](../Enterwell.CI.Changelog.DevOpsExtension).**

*We **highly** recommend that you read up on how and what exactly is it doing behind the scenes, as well as, learn how to use the `.changelog.json` configuration file to customize the tool's behaviour.*

## 📖 Table of contents
+ 🌱 [Introduction](#-introduction)
+ 🛠️ [Prerequisites](#-prerequisites)
+ 💻 [Installation](#-installation)
+ 📝 [Usage](#-usage)
+ ⚙️ [Configuration file](#-configuration-file)
+ 🏗 [Development](#-development)
+ ☎️ [Support](#-support)
+ 🪪 [License](#-license)

## 🛠 Prerequisities

Simple, just [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) or newer 🎉

## 💻 Installation

You can get the extension either by downloading and installing it manually from [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=Enterwell.EnterwellChangelogVsix) or by searching for it in the Visual Studio Extension Manager.

![](../img/extensionManager.png)

## 📝 Usage

Extension will only show up if you have an open Solution in the editor.

You can trigger the extension by using the `ALT + C` shortcut or by right-clicking on the Solution and selecting the `Add Change to Changelog` option.

![](../img/contextMenu.png)

One of the following dialog boxes will show up:

+ Changelog configuration ([explained later](#configuration-file)) exists:

  ![](../img/dialog_withConfig.png)

  `Change Category` is a drop-down list containing all of the valid categories for the changelog that are defined in the aforementioned Changelog configuration.

+ Changelog configuration does not exists:

  ![](../img/dialog_withoutConfig.png)

  `Change Category` is a text-box accepting any user input as the change category. (Input can be an empty value and all excess whitespace will be removed).

In both cases `Change Description` is a text-box that can accept any non-empty user input which is used to describe the changes made by the user. Excess whitespace will be removed.

**Add Change** button is disabled if the `Change Description` is invalid (empty), and enabled otherwise.

Extension uses the Visual Studio Status bar in order to log the results:

+ User pressed the **Cancel Change** button:

  ![](../img/statusBar_cancelled.png)

  This is the same behaviour as in the case of an exception. Only the *Reason* part of the message will show the exception message.

+ User pressed the **Add Change** button:

  ![](../img/statusBar_added.png)

And that's it! You should now have a new file created inside the **changes** folder in your solution! 🎉

You can now let this folder accumulate change entries which will be bundled up into your `CHANGELOG.md` when you want to create a new application release using our [Changelog Manager tool](../Enterwell.CI.Changelog) or one of our CI/CD tasks mentioned in the [introduction section](#-introduction).

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

For more features that can be configured using the configuration file, see the [Changelog Manager's README](../Enterwell.CI.Changelog/README.md/#configuration-file).

## 🏗 Development

In order to be able to run this code on your machine, you need to have:
1. [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) or newer
2. Visual Studio extension development toolset from the Visual Studio Installer

   ![](../img/dependency.png)

## ☎ Support
If you are having problems, please let us know by [raising a new issue](https://github.com/Enterwell/ChangelogManager/issues/new?title=[VSExtension]).

## 🪪 License
This project is licensed with the [MIT License](../LICENSE).