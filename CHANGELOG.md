# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.1.0] - 2022-04-11
### Added
- [CoreApp] Outputting newly bumped semantic version
- [DevOpsTask] Outputting newly bumped semantic version as an output variable called 'bumpedSemanticVersion'

## [2.0.0] - 2022-04-07
### Added
- [CoreApp] Automatic version bumping
- [CoreApp] Defining custom bump rules is available through the configuration
- [DevOpsTask] MergeChangelog@2 that does not need an explicit semantic version

### Changed
- [CLI] Using .NET 6
- [CoreApp] Using .NET 6
- [VSIX] Version bumped to correspond to other Changelog projects

### Deprecated
- [CoreApp] Explicit bumped version input

## [1.1.0] - 2021-09-22
### Added
- [CLI] --version option
- [CoreApp] 'changes' folder discovery
- [VSIX] VS 2022 support

### Fixed
- [VSIX] Category not cleared when opening another solution

## [1.0.0] - 2021-03-17
### Added
- [CLI] Initial version
- [CoreApp] Core logic implemented
- [DevOpsTask] Initial version
- [VSIX] Initial version
