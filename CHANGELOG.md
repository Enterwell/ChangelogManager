# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.5.0] - 2025-04-16
### Added
- [CoreApp] VsixManifest project file versioning format

### Changed
- [CoreApp] Case-insensitive file and folder names
- [CLI] Case-insensitive file and folder names
- [VSIX] Case-insensitive file and folder names

## [3.4.0] - 2025-04-14
### Added
- [CLI] 'List changes' subcommand for listing created changes
- [CLI] 'Add' subcommand for adding a change
- [CLI] 'List categories' subcommand for listing categories allowed by the configuration

## [3.3.0] - 2025-04-08
### Added
- [CoreApp] New 'merge-changelog' CLI option for optionally skipping the merge changelog step
- [CoreApp] AssemblyInfo versioning support
- [CoreApp] New 'revision' CLI option for passing in the explicit revision number

## [3.2.0] - 2025-03-14
### Changed
- [CoreApp] Supporting various project files

### Fixed
- [CoreApp] Bumping minor part by default if there is no 'NoChanges' rule in the custom configuration

## [3.1.0] - 2025-03-12
### Changed
- [CoreApp] Using .NET 8
- [CLI] Using .NET 8

## [3.0.0] - 2023-04-17
### Added
- [CoreApp] Words BREAKING CHANGE in change description now bump major version
- [CoreApp] Release workflow
- [CoreApp] BreakingKeyword field to changelog configuration JSON

### Changed
- [CoreApp] Deprecated changes now bump minor instead of major version
- [CoreApp] Outputting new changelog section instead of a semantic version

## [2.2.0] - 2022-05-25
### Added
- [CLI] '.changelog.json' file discovery
- [CoreApp] Opt-in AutoBump option (set the version to appropriate project file)

### Changed
- [CLI] Passed in category is now case-insensitive
- [CLI] Updated README
- [CoreApp] Only deleting change files that were valid, accepted and used to create a new changelog version
- [CoreApp] Updated README

### Fixed
- [CLI] Removing excess whitespace from the entered change description and category
- [VSIX] Removing excess whitespace from the entered change description and category

## [2.1.0] - 2022-04-11
### Added
- [CoreApp] Outputting newly bumped semantic version

## [2.0.0] - 2022-04-07
### Added
- [CoreApp] Automatic version bumping
- [CoreApp] Defining custom bump rules is available through the configuration

### Changed
- [CLI] Using .NET 6
- [CoreApp] Using .NET 6
- [VSIX] Version bumped to correspond to other Changelog projects

### Deprecated
- [CoreApp] Explicit bumped version input

## [1.1.0] - 2021-09-22
### Added
- [CLI] --version option
- [CLI] 'changes' folder discovery
- [VSIX] VS 2022 support

### Fixed
- [VSIX] Category not cleared when opening another solution

## [1.0.0] - 2021-03-17
### Added
- [CLI] Initial version
- [CoreApp] Core logic implemented
- [VSIX] Initial version
