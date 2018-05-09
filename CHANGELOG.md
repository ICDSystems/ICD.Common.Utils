# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]
 - Added Yield extension to return a single-item enumerable for an object.
 - Added util method for removing BOM characters from UTF8 data
 - Added extension method to convert from bool to ushort and back
 - Added extension method to cast enums to ushort value

## [3.0.0] - 2018-04-23
### Added
 - Adding extension method for getting Informational Version from an Assembly
 - Adding WeakKeyDictionary for caching
 - Reflection util methods
 
### Changed
 - JSON serialization/deserialization features moved into base converter
 - Removed suffix from assembly name
 