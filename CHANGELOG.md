# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [3.7.0] - 2018-07-02
### Added
 - Adding SequenceComparer for ordering collections of lists, arrays, etc
 - Added RateLimitedEventQueue collection for throttling events

### Changed
 - Potential fix for timer disposal on Net Standard
 - Added workaround for older RPC servers where the typestring being broadcast would stil include _SimplSharp, now will be stripped
 - Fixing bug where Timer.Reset() would continue repeating on an interval in Net Standard

## [3.6.0] - 2018-06-19
### Added
 - Added ZIP features for examining the contents of an archive
 - Added FileNameEqualityComparer
 - Added BiDictionary for one-to-one maps

## [3.5.1] - 2018-06-04
### Changed
 - PriorityQueue indexing fix
 - Pathfinding optimizations

## [3.5.0] - 2018-05-24
### Added
 - Added GetFlagsExceptNone parameterless enum method
 - Added pathfinding methods for determining if a path exists

## [3.4.0] - 2018-05-23
### Added
 - Added SetEquals method to IcdHashSet

## [3.3.0] - 2018-05-18
### Added
 - Added IcdOrderedDictionary collection
 - Added PriorityQueue collection
 - Added IcdUriBuilder and UriExtensions for reading/writing URI data

## [3.2.0] - 2018-05-09
### Added
 - Added util method for removing BOM characters from UTF8 data
 - Added extension method to convert from bool to ushort and back
 - Added extension method to cast enums to ushort value

## [3.1.0] - 2018-05-04
### Added
 - Added Yield extension to return a single-item enumerable for an object.

## [3.0.0] - 2018-04-23
### Added
 - Adding extension method for getting Informational Version from an Assembly
 - Adding WeakKeyDictionary for caching
 - Reflection util methods
 
### Changed
 - JSON serialization/deserialization features moved into base converter
 - Removed suffix from assembly name
 