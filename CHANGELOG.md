# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
 - Enum Extension "SetFlags", takes a bool to set or unset the given flags
 - BiDictionay - Added constructors with TKey and TValue comparers

## [15.0.0] - 2021-05-14
### Added
 - Ported CsvReader for CF 3.5 compatibility from: https://github.com/tspence/csharp-csv-reader
 - Added enum extension method for cycling to the next enum value
 - Added GetLocalTimeZoneName method to IcdEnvironment
 - Added MatchAny method to RegexUtils
 - Added OnSystemDeviceAddedRemoved and associated raise methods to IcdEnvironment for NETSTANDARD
 - Added GetParentUri method to UriExtensions
 - Added RegistryExtensions for working with Windows registry
 - Added session change event to IcdEnvironment for login/logout feedback
 - Added OrderedDictionary collection

### Changed
 - Updated TimeZones.sqlite to include daylight time zone info, added a new display name column.
 - Implemented ProcessorUtils for Windows
 - Renamed OrderedDictionary to SortedDictionary for consistency with .Net
 - Fixed a bug where SafeTimer.Trigger() would run the callback twice on .Net Standard
 - Fixed a bug where XML deserialization would fail to read out of empty elements
 
### Removed
 - ANSI color is no longer enabled on .Net Standard by default - it must be enabled by the calling application

## [14.2.0] - 2021-02-04
### Changed
 - ProcessorUtils Uptime methods changed to StartTime

## [14.1.0] - 2021-01-21
### Added
 - Added overload to GuidUtils that takes an enumerable of guids and combines them into a new deterministic guid

### Changed
 - A SafeTimer constructor that executes the callback immediately now does this instead of waiting infinitely

## [14.0.0] - 2021-01-14
### Added
 - Added Get and Set extensions to PropertyInfo in SIMPLSHARP to mimic overloads avaliable in NETSTANDARD
 - Added Collection extensions for setting and adding ranges of items
 - Added a method for getting the total number of seconds in a date
 - Added extensions to raise events with common event args using the data directly
 - Added property to IcdEnvironment to determine whether SSL communication is enabled
 - Added IcdTimeZoneInfo, a very light implementation of System.TimeZoneInfo for the .NET Compact Framework
 - Added ThreadedWorkerQueue - a threadsafe way to enqueue items and have a worker thread process them one at a time
 - Added eDaysOfWeek flags enum
 - Added support for reading the primitive type double to IcdXmlReader and XmlUtils
 - Added ProcessorUtils.GetSystemStartTime() to get DateTime the system started instead of a TimeSpan

### Changed
 - Repeater changed to use configured callbacks instead of a dumb event
 - Scheduled action callbacks allow a TimeSpan to be returned to delay actions
 - Handling a Crestron bug where File.Exists throws an exception on 4-Series instead of returning false
 - Changed ProcessorUtils.ModelVersion to be a string, Crestron pulls model version from CrestronEnvironment
 - For 4-series console outputs, replacing \n with \r\n to help console readability
 - Changed RuntimeEnvironment to be 3 variables - Framework for Crestron vs Standard, CrestronSeries for 3 vs 4, and CrestronRuntimeEnvironment for Simpl vs SimplSharpPro vs Server

## [13.0.0] - 2020-09-03
### Added
 - Added util methods for removing duplicate whitespace in strings
 - Added dequeue overload to ScrollQueue

### Changed
 - Replaced Crestron Unzip with Yallie Unzip
 - Fixed "version" regex for 4-series
 - Fixed date parsing error for 4-series
 - Split SimplSharpProMono runtime environment into SimplSharpProSever
 - Fixed log formatting on 4-series

## [12.1.0] - 2020-07-14
### Added
 - ReflectionExtensions : GetProperty, SetProperty, CallMethod extensions for NetStandard
 - Added attributes for controlling obfuscation
 - Added AggregateOrDefault extension method for applying an accumulator function over a sequence that returns a default value if the sequence is empty

### Changed
 - DHCP status is a boolean
 - Changed Hostname property to Hostnames

## [12.0.0] - 2020-06-18
### Added
 - Added ToCollection extension method for copying an enumerable to a new collection
 - TableBuilder supports multi-line content
 - Added eIcdFileMode for IO platform agnosticism
 - Extension method for determining if a Type is anonymous
 - Extension method for getting inner generic Types
 - Added extension method for dynamically converting a sequence to a generic list of the given item type
 - Added methods for getting UserData paths
 - Added methods for reading GUIDs from XML
 - Added methods for reading DateTimes from XML
 - Added method for combining GUIDs
 - Added method for getting the EventArgs type for an EventHandler
 - Added methods for getting a JSON value as a float or double
 - Added dictionary Remove method for outputting the removed value
 - Added IGenericEventArgs interface
 - Added MinimalTypeConverter for serializing Types to JSON
 - Added common JSON serializer settings for common, platform agnostic DateTime and Type conversion
 
### Changed
 - Rewrote JsonItemWrapper serialization for JsonConvert friendliness
 - Reflection optimizations
 - Fixed NullParameterException in TableBuilder
 - Improvements to EnumUtils, less reliance on casting to/from int
 - Cleaned up TimeSpan.ToReadableString() output
 - Fixed a bug where System.Reflection exceptions can't be caught in S#
 - TableBuilder no longer draws redundant separators
 - Fixed a bug where CompiledOn date was not being parsed correctly due to culture
 - S# DateTimes are serialized to JSON in ISO-8601 format
 - Deadlock detection works better for false positives
 - Improved LogItem JSON serialization
 - Improved NiceName method to better handle syntax, whitespace and punctuation
 - Fixed a bug where IcdCultureInfo would fail to load on Crestron 4-series processors
 - Clarifying which culture failed to load when IcdCultureInfo throws an exception

## [11.1.1] - 2020-08-21
### Removed
 - Removed the OnItemTrimmed event from the ScrollQueue due to deadlocks

## [11.1.0] - 2020-05-19
### Added
 - ScrollQueue - Added OnItemTrimmed event
 - Added DateTimeNullableEventArgs

## [11.0.0] - 2020-03-20
### Added
 - Added Not null tag for ICDUriBuilder Constructor that takes a URI as an argument.
 - Added MathUtils methods for converting to and from percentages
 - Added enum extensions for finding the inclusion and exclusion of enum flags
 - Added DateTime extensions for adding years, months, days, hours, minutes and wrapping without modifying other values
 - Added shims for deserializing an XML array using a callback for each item
 - Added methods for serializing an XML array
 - Added WriteAllByte method on IcdFile.
 - Added PathUtils for building paths in the HTML directory
 - Added public access to GetValues enumeration extension
 - Added extensions for getting JsonReader values as long or ulong
 - Added DateTimeUtils methods for creating DateTimes from epoch seconds or milliseconds
 - Added utils for splitting ANSI into spans for conversion to XAML, HTML, etc
 
### Changed
 - Fixed exception trying to get DHCP status of network interfaces on Linux
 - Fixed a bug where color formatted console output on Net Standard was not raising the OnConsolePrint event
 - Simplifying ANSI color methods, better cross-platform color support
 - Console uses unicode for table drawing on Net Standard
 - Using UTC for tracking scheduled events, fixes issues with DST
 - Using UTC for tracking durations
 - Fixed a bug where table width calculations were not considering unprintable characters

## [10.3.0] - 2020-01-20
### Changed
 - Network/MAC/DNS address utils are now enumerating all adapter types
 - Ignoring Crestron ethernet parameters that say "Invalid Value"
 - Skipping network interfaces with an invalid adapter id

## [10.2.0] - 2019-12-04
### Added
 - Added shim methods for finding closest DateTimes from a sequence

## [10.1.0] - 2019-11-18
### Added
 - Added PathUtils methods for getting ProgramData paths
 - Added a method for determining if a URI is defaults
 - Added MaxOrDefault extension method for enumerables
 - Added a method for finding an item in a sorted list by a given predicate

### Changed
 - NullObject implements IComparable, fixes issues with null keys in ordered dictionaries
 - IcdSqliteConnection CreateFile method will create directories recursively

## [10.0.0] - 2019-10-07
### Added
 - IcdEnvironment.GetUtcTime() to get UTC representaiton of current time.
 - Extension methods for determining if a sequence is in order
 - Overload for calculating the modulus of longs
 - Default implementation for AbstractGenericXmlConverter Instantiate method
 - Additional binary search extensions, now working for all ILists
 - Added NullObject as a means of having null keys in hash tables

### Changed
 - Potential fix for unhelpful exception messages coming from SafeCriticalSection.Execute
 - Small performance improvement when copying arrays

## [9.9.0] - 2019-09-16
### Added
 - Added a method for converting 24 hour to 12 hour format
 - Added a method for determining if a culture uses 24 hour format
 - Added math util method for modulus
 - Added TimeSpan extension methods for cycling hours and minutes without modifying the day
 - Added a dictionary extension method for getting or adding a new value via func
 - Added CultureInfo extensions for converting between 12 hour and 24 hour time formatting
 - Added environment methods for setting the current date and time
 - Added BinarySearch extension method for all IList types
 - Added PathUtils methods to get ProgramData directory

### Changed
 - The Root Config path in Net Standard will now be the ICD.Connect folder in the current environments ProgramData directory
 - Fixed a bug where CultureInfo was not being cloned correctly
 - List AddSorted extensions now work for all IList types

## [9.8.0] - 2019-09-03
### Added
 - Added Public API Properties to get the program install date based on the creation date of core dll file for NetStandard and SimplSharp
 - Implemented processor utils for NetStandard to get the system uptime and the program uptime
 - Added methods for deserializing an XML array

### Changed
 - Improvements to JSON DateTime parsing, particularly in Net Standard

## [9.7.1] - 2019-08-17
### Changed
 - Fixed CultureInfo SQLite conection string for 4-series compatibility

## [9.7.0] - 2019-08-15
### Added
 - Added logger timestamps to non simplsharp programs
 - Added Net Standard Support for JSON DateTime formats
 - Added EmailValidation class

### Changed
 - JSON dict serialization serializes keys instead of converting to property name
 - Fixed a bug where ANSI color encoded strings with percentages were being scrambled

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

## [8.9.3] - 2020-08-17
### Changed
 - Workaround for logged XML format exceptions when failing to parse floats

## [8.9.2] - 2020-07-28
### Changed
 - StringExtensions - fixed an issue with IsNumeric where empty strings would return true

## [8.9.1] - 2020-05-27
### Changed
 - Changed ProcessorUtils to use CrestronEnvironment to retrive serial number - this fixes issues with new serial numbers that aren't deciaml TSIDs

## [8.9.0] - 2020-04-30
### Changed
 - ProgramUtils and ProcessorUtils return dates instead of strings for date properties

## [8.8.1] - 2020-02-18
### Changed
 - IcdTimer - fixed issue that prevented OnElapsed event from firing when Length is less than (or close to) Heartbeat Interval

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
