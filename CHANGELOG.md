# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [9.8.0] - 2019-09-03
### Added
 - Added Public API Properties to get the program install date based on the creation date of core dll file for NetStandard and SimplSharp
 - Implemented processor utils for NetStandard to get the system uptime and the program uptime
 - Added methods for deserializing an XML array

### Changed
 - Fixed a bug where ANSI color encoded strings with percentages were being scrambled
 - Improvements to JSON DateTime parsing, particularly in Net Standard

## [9.7.0] - 2019-08-15
### Added
 - Added logger timestamps to non simplsharp programs
 - Added Net Standard Support for JSON DateTime formats
 - Added EmailValidation class
 
### Changed
 - JSON dict serialization serializes keys instead of converting to property name

## [9.6.0] - 2019-07-03
### Added
 - Added RecursionUtils method to get a single clique given a starting node
 - Breadth First Search can now search graphs in addition to trees
 - Added StartOfDay and EndOfDay DateTime extension methods

### Changed
 - Fixed bug in IcdUriBuilder where Query property behaved differently to UriBuilder
 - Throwing an exception when attempting to read a non-primitive JSON token as a string

## [9.5.0] - 2019-06-10
### Added
 - Added Shim to read a list from xml with no root element
 - Added a URI query builder

### Changed
 - Fixed JSON DateTime parsing in .Net Standard
 - Fixed threading exception in TypeExtensions
 - Fixes for platform-agnostic culture handling

## [9.4.0] - 2019-05-10
### Added
 - Added extension method for peeking queues
 - Added extension method for getting or adding a new item to a dictionary
 - Added methods for serializing additional types, arrays and dictionaries to JSON
 - AbstractGenericJsonConverter exposes virtual methods for overriding object serialization/deserialization
 - Added RemoveRange method to IcdHashSet
 - Added IcdCultureInfo and CultureInfo database for localization

### Changed
 - IcdUriBuilder constructors behave closer to UriBuilder, Host defaults to "localhost"

## [9.3.0] - 2019-04-16
### Added
 - Added SPlusUtils with ConvertToInt method taking LowWord/HighWord ushorts
 - Added JsonReader extension methods for reading DateTimes
 - Added JsonReader extension methods for writing properties
 - IcdStreamWriter exposes WriteLine(string)
 - Added ProgramLogsPath to PathUtils

### Changed
 - Fixes for VC4 compatibility
 - Fixed JSON DateTime parsing for timezone information
 - Small reflection optimizations

## [9.2.0] - 2019-03-01
### Added
 - Added Type IsAssignableTo extension shim
 - Added constructor to BiDictionary to instantiate from an existing dict

### Changed
 - Fixed bug preventing deserialization of XML lists
 - Crestron ConsoleResponse uses PrintLine instead of Print
 - Use PrintLine instead of ConsoleResponse on Crestron server

## [9.1.0] - 2019-02-07
### Added
 - Added SubscribeEvent shim for delegate callbacks
 - Extension method for reading JSON token as a GUID
 - Added ToStringJsonConverter

### Changed
 - Significantly reduced size of JSON serialized Types
 - Small logging optimizations

## [9.0.0] - 2019-01-29
### Added
 - IcdConsole.OnConsolePrint event

### Changed
 - Better VC-4 support for IcdConsole
 - JSON refactoring for simpler deserialization

## [8.8.0] - 2020-01-23
### Added
  - Added an overload to PriorityQueue for determing the de-duplication behaviour

## [8.7.2] - 2019-10-29
### Changed
  - Fixed a bug with PriorityQueue de-duplication where a new command would be inserted in the wrong position

## [8.7.1] - 2019-08-22
### Changed
 - Fixed a bug with the IcdOrderedDict index setter that was creating additional values

## [8.7.0] - 2019-06-24
### Added
 - IcdXmlException exposes line number and position properties

## [8.6.1] - 2019-06-14
### Changed
 - Fixed a bug where stopped timers on NetStandard would still have a periodic callback duration

## [8.6.0] - 2019-06-14
### Changed
 - Overhaul of RangeAttribute remap methods to better avoid overflows

## [8.5.0] - 2019-06-06
### Added
 - Adding features to IcdEnvironment for tracking program initialization state

## [8.4.1] - 2019-06-05
### Changed
 - Caching the program/processor start time and calculating the uptime from those values instead of polling the crestron processor

## [8.4.0] - 2019-05-15
### Added
 - Added GUID utils for generating seeded GUIDs
 - Added extension method for getting stable hashcodes from strings
 - Added environment and processor utilities for determining DNS status and hostname

### Changed
 - RangeAttribute improvements for better type safety
 - PathUtils breaking out ProgramConfigDirectory and CommonConfigDirectory from the full paths

## [8.3.3] - 2019-05-24
### Added
 - Added empty, placeholder interface for ICD Attributes

## [8.3.2] - 2019-05-02
### Changed
 - Fixed PriorityQueue IndexOutOfRange exception when an inner queue becomes depleted

## [8.3.1] - 2019-04-05
### Changed
 - Fixed FormatException when parsing some JSON DateTimes

## [8.3.0] - 2019-01-25
### Added
 - Added SimplSharpProMono to eRuntimeEnvironment enum
 - Added path support for SimplSharpProMono environment
 - Added GetApplicationRootDirectory for all platforms

### Changed
 - Small fixes for better VC4 support

## [8.2.0] - 2019-01-10
### Added
 - Added TryGetPortForScheme method to UriExtensions
 - Added range attribute for clarifying numeric fields, properties and parameters

### Changed
 - IcdHashSet preserves comparer when an operation creates a new IcdHashSet
 - Fixed bug where XML fragments on Net Standard were being prepended with a document header

## [8.1.0] - 2019-01-02
### Added
 - Added GetAttributeAsEnum xml utils method
 - Adding short parsing methods to XML utils
 - Added methods to IcdUriBuilder for appending path
 - Added RegexUtils method for replacing a single group in a match

## [8.0.0] - 2018-11-20
### Added
 - XML TryGetAttribute methods

### Changed
 - Performance improvements when working with xml attributes
 - Fixed NullReferenceException when writing null strings to CSV
 - Fixed bug with string formatting console input on Net Standard

### Removed
 - Removed IcdXmlAttribute

## [7.1.0] - 2018-11-08
### Added
 - IcdXmlTextWriter exposes WriteStartDocument and WriteEndDocument
 - AttributeUtils method for getting properties with the given attribute type

### Changed
 - EnumerableExtensions performance improvements
 - Fixed bug in StringExtensions when removing a sequence of characters from a string

## [7.0.0] - 2018-10-30
### Changed
 - Micro-optimizations in string and XML manipulations
 - Significant hashset optimizations
 - Deprecated NVRAM in favor of USER directory

## [6.0.0] - 2018-10-18
### Added
 - CsvWriter for creating CSV files + Settings
 - AppendText method for IcdFile
 - IcdStreamWriter, a wrapper for a StreamWriter
 - New XML conversion framework for performance improvements

### Changed
 - XmlUtils is now using the improved XML conversion framework 
 - Better implementation of DictionaryExtensions.ToInverse

## [5.0.0] - 2018-09-14
### Added
 - Stopwatch profiling methods
 - Attempt to interpret old assembly naming convention when parsing types
 - Added RegexUtils

### Changed
 - Significant performance improvements across the board

## [4.0.0] - 2018-07-19
### Added
 - Added extension method for getting type name without trailing generic info
 - Added DateTime extensions for getting next/last time in a sequence
 - Added action scheduler service
 - Added IcdTimer methods for profiling event performance

### Changed
 - Re-raise base exception from ReflectionUtils.CreateInstance, TargetInvocationException and TypeLoadException don't say much

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
 