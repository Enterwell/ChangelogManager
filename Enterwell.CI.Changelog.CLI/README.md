# Changelog Create CLI Tool

`Changelog.CLI` is a helper Console App project using .NET 6 that servers as a CLI version of the [Visual Studio extension](../Enterwell.CI.Changelog.VSIX), helping developers automatically create needed files from the command line/terminal to be used by the [`Changelog Manager`](../Enterwell.CI.Changelog) tool.

## Table of contents

+ [How to use this?](#how-to-use-this)
+ [Configuration file](#configuration-file)
+ [Result / Output](#result--output)

## How to use this?
If you're unfamiliar with the `Changelog Manager` tool, it is highly recommended that you read through the `Changelog Manager's` [README](../Enterwell.CI.Changelog/README.md) to understand why would you need this helper CLI.

Firstly, you can use our [GitHub Releases](https://github.com/Enterwell/ChangelogManager/releases/) tab to start using this CLI. Link contains pre-built binaries for Windows and Linux.

You can run the binary anywhere in the target project's solution directory.

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
   + One of the valid change categories (case-insensitive) defined in the [configuration file](#configuration-file) if it exists.
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

If the configuration exists, helper will deny the creation of change files which ignore every change in the **changes** directory that does not concur to it. On the other hand, if the configuration file does not exist, every change will be accepted and written to the `CHANGELOG.md`.

For more features that can be configured using the configuration file, see the [Changelog Manager's README](../Enterwell.CI.Changelog/README.md/#configuration-file).

## Result / Output
If the CLI is used in the directory that doesn't contain **changes** directory to create the change file in, it will automatically search directory tree upwards for the nearest **changes** directory. Created file is named correctly and can be used by our [`Changelog Manager`](../Enterwell.CI.Changelog) tool.

## Development

### Publishing

```bash
dotnet publish -c release -r win-x64 -p:PublishSingleFile=true
dotnet publish -c release -r linux-x64 -p:PublishSingleFile=true
```
