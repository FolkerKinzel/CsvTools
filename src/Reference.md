# FolkerKinzel.CsvTools
.NET-library that allows to read and write CSV files. ("Comma-Separated Values", RFC 4180) 

It contains
* `CsvReader`: Reads CSV from files and streams. It enables you to perform Linq-Queries on CSV files.
* `CsvWriter`: Writes CSV to files and streams.
* `CsvRecordWrapper`: Maps the columns of the CSV file to the columns of your `DataTable` or to the properties of an `object` and performs type conversions. `CsvRecordWrapper` supports aliases with wildcards for CSV column names and ships with a lot of supporting classes - most are data type converters for the most needed data types and an easy to implement interface to create converters of whatever data type you need.
* `CsvAnalyzer`: Analyzes CSV files to get the right parameters for proper reading. This enables you, to read CSV files, that don't fit the standard.

`FolkerKinzel.CsvTools` serves as a dependency in [FolkerKinzel.Contacts.IO](https://www.nuget.org/packages/FolkerKinzel.Contacts.IO/) - a small and easy to use framework for .NET, which allows to manage contact data of organizations and natural persons, including a data model and classes to persist it as vCard (*.vcf) or CSV.


[Download Reference (English)](https://github.com/FolkerKinzel/CsvTools/blob/master/ProjectReference/1.6.0/FolkerKinzel.CsvTools.en.chm)

[Projektdokumentation (Deutsch) herunterladen](https://github.com/FolkerKinzel/CsvTools/blob/master/ProjectReference/1.6.0/FolkerKinzel.CsvTools.de.chm)

> IMPORTANT: On some systems the content of the CHM file is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox (if it is present) in the lower right corner of the General tab in the Properties dialog.

- [Version History](https://github.com/FolkerKinzel/CsvTools/releases)



