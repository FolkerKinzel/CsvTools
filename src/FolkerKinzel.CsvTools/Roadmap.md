# FolkerKinzel.CsvTools
## Roadmap

### 2.0.0
- [x] Remove all obsolete symbols.
- [x] Reduce namespaces to increase the usability.
- [x] Remove the interface ICloneable from CsvProperty and CsvIndexProperty.
- [x] Remove CsvIndexProperty.
- [x] Add a new constructor to CsvProperty that takes the desired column index as argument (instead of
column name aliases). This is for CSV files without header row.
- [x] Add new properties DesiredCsvColumnIndex and ReferredCsvColumnIndex to CsvProperty.
- [x] CsvProperty should internally use the column index to target CsvRecord (for performance).

