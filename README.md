# FolkerKinzel.CsvTools
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.CsvTools)](https://www.nuget.org/packages/FolkerKinzel.CsvTools/)
[![GitHub](https://img.shields.io/github/license/FolkerKinzel/CsvTools)](https://github.com/FolkerKinzel/CsvTools/blob/master/LICENSE)

## .NET library for reading and writing CSV files (RFC 4180)
[Project Reference](https://folkerkinzel.github.io/CsvTools/reference/)

Starting with version 2.0.0, mapping functionality and type converters 
are in the separate package [FolkerKinzel.CsvTools.Mappings](https://www.nuget.org/packages/FolkerKinzel.CsvTools.Mappings/). This ensures that only what is really needed needs to be installed.

You can use this small library independently
- for analyzing CSV files and CSV strings (delimiter, header row, column names, text encoding, and required options for reading non-standard CSV)
- serializing collections of any data and DataTables as CSV with simple formatting options. (Use [FolkerKinzel.CsvTools.Mappings](https://www.nuget.org/packages/FolkerKinzel.CsvTools.Mappings/) for advanced object serializing and deserializing.)
- parsing the string content of CSV files and CSV strings, e.g., with Linq. (Use [FolkerKinzel.CsvTools.Mappings](https://www.nuget.org/packages/FolkerKinzel.CsvTools.Mappings/) for deserializing data tables and data types other than strings.)

FolkerKinzel.CsvTools allows you to do things easily and with just a few lines of code. In addition, the library also provides the means to write high-performance code:
- It allows you to parse ultra large CSV files because only one row of the file has to be in memory at a time.
- It makes the fields of the CSV file available as `ReadOnlyMemory<Char>` instances. This avoids the allocation of numerous temporary strings.
- The `CsvOpts.DisableCaching` option allows reusing the same `CsvRecord` instance for each parsed row of the CSV file. This can avoid further allocations.

### Code Examples
- [Converting object data to CSV](https://github.com/FolkerKinzel/CsvTools/blob/master/src/Examples/CsvStringExample.cs)
- [Analyzing and converting CSV files](https://github.com/FolkerKinzel/CsvTools/blob/master/src/Examples/CsvAnalyzerExample.cs)
- [Linq on CSV files](https://github.com/FolkerKinzel/CsvTools/blob/master/src/Examples/LinqOnCsvExample.cs)
- [The DisableCaching option](https://github.com/FolkerKinzel/CsvTools/blob/master/src/Examples/DisableCachingExample.cs)

[Version History](https://github.com/FolkerKinzel/CsvTools/releases)





