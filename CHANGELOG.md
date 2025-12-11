# Changelog

All notable changes to PerfPulse will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- Support for methods with parameters
- Support for instance methods
- XML export format
- Comparison mode (compare multiple methods)
- HTML report generation

## [1.0.0] - 2025-01-15

### Added
- Initial release of PerfPulse
- Command-line benchmark mode
- Interactive benchmark mode
- Comprehensive timing statistics (Mean, Median, Min, Max, StdDev, P95, P99)
- Memory usage tracking
- Garbage Collection monitoring (Gen 0, 1, 2)
- Distribution histogram visualization
- CSV export functionality
- JSON export functionality
- Progress bar during benchmark execution
- Warmup phase support
- Color-coded console output
- Help command
- Support for static, parameterless methods
- Assembly loading and validation
- Method discovery and validation

### Features
- **Timing Measurements**: Nanosecond precision using `Stopwatch`
- **Memory Tracking**: Before/after memory snapshots and delta calculation
- **GC Analysis**: Track collections across all generations
- **Statistical Analysis**: Multiple percentiles and standard deviation
- **Visualization**: ASCII histogram with 20 buckets
- **Export Formats**: CSV (raw data) and JSON (structured results)
- **User Interface**: Rich console UI with Unicode box-drawing characters
- **Flexible Configuration**: Customizable iterations and warmup counts

### Technical Details
- Built with .NET 10
- Uses C# 14.0 features
- Reflection-based method invocation
- UTF-8 console encoding support
- Cross-platform compatible

## Version History

### Version Numbering

PerfPulse follows [Semantic Versioning](https://semver.org/):

- **MAJOR**: Incompatible API changes
- **MINOR**: Backwards-compatible functionality additions
- **PATCH**: Backwards-compatible bug fixes

[Unreleased]: https://github.com/yourusername/PerfPulse/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/yourusername/PerfPulse/releases/tag/v1.0.0
