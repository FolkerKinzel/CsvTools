# FolkerKinzel.CsvTools
## Roadmap

### 2.0.0
- [x] Add .NET 6.0 support.
- [x] Add .NET Standard 2.0 support.
- [x] Add .NET Framework 4.6.1 support.
- [x] Remove all obsolete symbols.
- [x] Reduce namespaces to increase the usability.
- [x] Remove the interface `ICloneable` from `CsvProperty`.
- [x] Remove `CsvIndexProperty`.
- [x] Add a new constructor to `CsvProperty` that takes the desired column index as argument (instead of
column name aliases). This is for CSV files without header row.
- [x] Add new properties `DesiredCsvColumnIndex` and `ReferredCsvColumnIndex` to `CsvProperty`.
- [x] `CsvProperty` should internally use the column index to target `CsvRecord` (for performance).
- [x] Add `CsvMultiColumnTypeConverter`.
- [x] Rename `FolkerKinzel.CsvTools.Helpers` to `FolkerKinzel.CsvTools.TypeConversions`.
- [ ] Don't allow `CsvWriter` to terminate the last row in a CSV file.
- [ ] Let `CsvReader` implement `IEnumerable<CsvRecord>`.
- [ ] Let `CsvReader.Read()` return the next `CsvRecord` or `null` if EOF is reached.

### 2.1.0
- [ ] Add a BigIntegerConverter.
- [ ] Add a UriConverter.
- [ ] Add a VersionConverter.
- [ ] Replace NumberConverter&lt;T&gt;by Converters of specified Types to avoid unnecessary unboxing for performance reasons.

### 3.0.0
- [ ] Remove .NET Framework 4.0 support.
- [ ] Add a DateOnlyConverter.
- [ ] Add a TimeOnlyConverter.

