# Introduction 
`Changelog.VSIX` is a VSIX project that contains the [Visual Studio](https://visualstudio.microsoft.com/vs/) Extension which creates files in the solution folder that are used by the [`Changelog`](../Enterwell.CI.Changelog) project in order to fill out the `CHANGELOG.md` file.

# Installation

To be able to install the extension to your own machine, you need to build it and run the generated `Enterwell.CI.Changelog.VSIX.vsix` file in the `bin\(Debug|Release)` folder. 

Installer will pop up showing the extension name asking to which installed Visual Studio product you want to install the extension to.

![](../img/installer.png)

# Usage

Extension will only show up if you have open Solution in the editor.

You can trigger the extension by using the `ALT + C` shortcut or by right-clicking on the Solution and selecting the `Add Change to Changelog` option.

One of the following dialog boxes will show up:

+ Changelog configuration (explained later) exists:

  ![](../img/dialog_withConfig.png)

  `Change Category` is a drop-down list containing all of the valid categories for the changelog that are defined in the aforementioned Changelog configuration.

+ Changelog configuration does not exists:

  ![](../img/dialog_withoutConfig.png)

  `Change Category` is a text-box accepting any user input as the change category. (Input can be an empty value).

In both cases `Change Description` is a text-box that can accept any non-empty user input which is used to describe the changes made by the user. **Add Change** button is disabled if the `Change Description` is invalid (empty), and enabled otherwise.

Extension uses the Visual Studio Status bar in order to log the results:

+ User pressed the **Cancel Change** button:

  ![](../img/statusBar_cancelled.png)

  This is the same behaviour as in the case of an exception. Only the *Reason* part of the message will show the exception message.

+ User pressed the **Add Change** button:

  ![](../img/statusBar_added.png)

# Configuration file
`.changelog.json` is a [JSON file](https://www.json.org/json-en.html) which is optional. Configuration specifies which change categories are allowed in your project. File needs to be located in the solution root along with the **changes** folder and the appropriate `CHANGELOG.md` file.

If we wanted to allow only 3 different change categories: API, FE (Frontend) and BE (Backend), the configuration would look like:

```
{
  "categories": [
    "API",
    "FE",
    "BE"
  ]
}
```

If the configuration exist, application will ignore every change in the **changes** folder which does not concur to it. On the other hand, if the configuration file does not exist, every change will be accepted and written to the `CHANGELOG.md`.

# Result / Output

If the change was added successfully, **changes** folder is created in the Solution root if one did not exist already, with the correct file and naming used by our [`Enterwell.CI.Changelog`](../Enterwell.CI.Changelog) project.

# Development

In order to be able to run this code on your machine, you need to have:
1. [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) or newer
2. Visual Studio extension development toolset from the Visual Studio Installer

   ![](../img/dependency.png)