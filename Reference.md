# FolkerKinzel.CsvTools
.NET-library to read and write CSV-files.

The library contains
* `CsvReader`: Reads CSV from files and streams. It enables you to perform Linq-Queries on CSV files.
* `CsvWriter`: Writes CSV to files and stream.
* `CsvRecordWrapper`: Maps the columns of the CSV file to the columns of your `DataTable` or to the 
properties of an `object` and performs type conversions. `CsvRecordWrapper` supports aliases with wildcards
for CSV column names and ships with a lot of 
supporting classes - most are data type converters for the most needed data types and an easy to implement
interface to create converters of whatever data type you need.
* `CsvAnalyzer`: Analyzes CSV files to get the right parameters for proper reading. This enables you, to read
CSV files, that don't fit the standard.

.

[Download Reference (English)](https://github.com/FolkerKinzel/CsvTools/blob/master/FolkerKinzel.CsvTools.Reference.en/Help/FolkerKinzel.CsvTools.en.chm)

[Projektdokumentation (Deutsch) herunterladen](https://github.com/FolkerKinzel/CsvTools/blob/master/FolkerKinzel.CsvTools.Doku.de/Help/FolkerKinzel.CsvTools.de.chm)

> IMPORTANT: On some systems, the content of the CHM file is blocked. Before extracting it,
>  right click on the file, select Properties, and check the "Allow" checkbox - if it 
> is present - in the lower right corner of the General tab in the Properties dialog.
