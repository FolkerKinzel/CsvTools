﻿using System.Data;

namespace FolkerKinzel.CsvTools;

/// <summary>Named constants to specify options for reading CSV files.
/// The flags can be combined.</summary>
/// <remarks>
/// <note type="tip">
/// To work safely and conveniently with the <see cref="CsvOpts"/> enum, use the 
/// extension methods of the <see cref="CsvOptsExtension" /> class (see example).
/// </note>
/// </remarks>
/// <seealso cref="CsvReader"/>
/// <seealso cref="CsvOptsExtension"/>
/// <seealso cref="Csv"/>
[Flags]
public enum CsvOpts
{
    /// <summary>No flag is set. This creates a very lenient parser that rarely throws
    /// exceptions.</summary>
    None = 0,

    /// <summary>If set, <see cref="CsvReader" /> throws an <see cref="CsvFormatException"
    /// /> if a data row contains more fields than the first data row.
    /// <note type="note">
    /// <para>
    /// If a data row has more Fields than the first data row, this is a strong indication
    /// of a read error. The flag should therefore usually be set.
    /// </para>
    /// <para>
    /// Unset this flag only if the CSV file has a header and if you are not interested
    /// in the surplus fields. Alternatively use 
    /// <see cref="Csv.OpenReadAnalyzed(string, System.Text.Encoding?, Header, bool, int)"/>
    /// with <see cref="int.MaxValue"/> for the number of rows to analyze.
    /// </para>
    /// </note>
    /// </summary>
    ThrowOnTooMuchFields = 1,

    /// <summary> If set, <see cref="CsvReader" /> throws a <see cref="CsvFormatException" /> 
    /// if a data row contains fewer fields than the first data row.
    /// <note>
    /// Other software may not mark empty fields at the end of the line with field separators. 
    /// However, the absence of fields at the end of the line can also be an indication of a 
    /// data error.
    /// </note>
    /// </summary>
    ThrowOnTooFewFields = 1 << 1,

    /// <summary>If set, <see cref="CsvReader" /> throws an <see cref="CsvFormatException"
    /// /> if there are blank lines in the CSV file that are not part of a data field
    /// masked with quotes.
    /// <note>
    /// Blank lines that are not part of a masked field can in no case be interpreted
    /// as part of the data to be read. However, by removing the flag, <see cref="CsvReader"
    /// /> ignores such blank lines.
    /// </note>
    /// </summary>
    ThrowOnEmptyLines = 1 << 2,

    /// <summary>If set, <see cref="CsvReader" /> throws an <see cref="CsvFormatException"
    /// /> if a masked field at the end of the file is not properly closed.
    /// <note>
    /// An unclosed masked field at the end of a CSV file is a data error. However, by 
    /// removing the flag, 
    /// <see cref="CsvReader" /> will be able to parse the rest of the file. The 
    /// <see cref="ThrowOnTooFewFields"/> flag should also be unset in this case because a 
    /// CSV row could be truncated by the unclosed masked field.
    /// </note>
    /// </summary>
    ThrowOnTruncatedFiles = 1 << 3,

    /// <summary>If the flag is not set, the class <see cref="CsvRecord" /> interpretes
    /// the column names of the CSV file in a case-insensitive manner. This is the same
    /// behavior that <see cref="DataColumnCollection" /> shows.</summary>
    CaseSensitiveKeys = 1 << 4,

    /// <summary>If the flag is set, <see cref="CsvReader" /> will remove leading and trailing
    /// white space from all data tokens and the column names. That can damage data
    /// where the white space has a meaning. Only set the flag
    /// for reading non-standard CSV files, that introduce additional padding.</summary>
    TrimColumns = 1 << 5,

    /// <summary>Default setting. This is a combined value, that forces <see cref="CsvReader"
    /// /> to throw an <see cref="CsvFormatException" /> if the file to be read does
    /// not comply with the RFC 4180 standard. (Alternative column 
    /// separators and newline characters are always tolerated.)</summary>
    Default = ThrowOnTooMuchFields | ThrowOnTooFewFields | ThrowOnEmptyLines | ThrowOnTruncatedFiles,

    /// <summary>Set this flag to gain performance benefits when parsing large files. Note that if 
    /// this flag is set, the results of the read operation cannot be cached: the <see cref="CsvRecord" />
    /// instance provided by <see cref="CsvReader"/> will then always be the same. Only the values ​​it 
    /// contains will update with each iteration.</summary>
    DisableCaching = 1 << 6
}
