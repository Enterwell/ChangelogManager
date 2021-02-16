# Introduction 
`Changelog.CLI` Console App project using .NET 5 which servers as a CLI version of [VSIX](../Enterwell.CI.Changelog.VSIX) project, helping developers automatically create needed files from the command line / terminal.

# Usage
Application takes two arguments and one optional input: 
+ change type - required
+ change description - required
+ change category - optional (prefixed with either -c or --category)

Example of a call:

> cc [options] <change_type> <change_description>

Arguments:
+ change_type: 
   + Change type following the ['Keep a Changelog' guiding principles](https://keepachangelog.com/en/1.0.0/#how).
   + Acceptable entries for `<change_type>` are:
      + Added
      + Changed
      + Deprecated
      + Removed
      + Fixed
      + Security
      + each of the previous entries can be specified using their first letters also;
   + Acceptable entries are case-insensitive.
+ change_description:
   + Change description that describes the changes made.

Options:
+ -c | --category <change_category>:
   + One of the valid change categories determined in the configuration file (explained later), or arbitrary if configuration does not exist or is empty.
+ -? | -h | --help:
   + Shows help information.

Changes in the **changes** folder are just files with the following naming scheme:

> <change_type> <[change_category]> <change_description>

`<[change_category]>` part of the naming is controlled and validated using the `.changelog.json` configuration file.

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
If the change was added successfully, **changes** folder is created in the directory developer is calling the CLI from if one did not exist already, with the correct file and naming used by our [`Enterwell.CI.Changelog`](../Enterwell.CI.Changelog) project.