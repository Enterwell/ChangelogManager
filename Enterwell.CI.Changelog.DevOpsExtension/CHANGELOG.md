# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.4.0] - 2025-04-09
### Added
- New optional action inputs

### Changed
- Using ChangelogManager v3.3.0 binaries

## [3.3.0] - 2025-04-08
### Added
- macOS binary for running on macOS agents

### Changed
- Using ChangelogManager v3.2.0 binaries

## [3.2.0] - 2025-03-11
### Added
- Node 20 support

## [3.1.0] - 2023-12-20
### Changed
- Updated README

### Fixed
- File executable in V3 is now called correctly

## [3.0.0] - 2023-04-17
### Added
- Multiple task outputs
- MergeChangelog@3 that calls the new CoreApp

### Changed
- MergeChangelog@3 'setVersionFlag' input is now called 'shouldBumpVersion'

## [2.2.0] - 2022-05-25
### Added
- MergeChangelog@2 Azure Pipelines task that can be ran on both Ubuntu and Windows VMs

### Changed
- Updated README

## [2.1.0] - 2022-04-11
### Added
- Outputting newly bumped semantic version as an output variable called 'bumpedSemanticVersion'

## [2.0.0] - 2022-04-07
### Added
- MergeChangelog@2 that does not need an explicit semantic version

## [1.0.0] - 2021-03-17
### Added
- Initial version
