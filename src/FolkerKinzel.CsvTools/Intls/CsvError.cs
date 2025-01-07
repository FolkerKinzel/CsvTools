namespace FolkerKinzel.CsvTools.Intls;

[Flags]
internal enum CsvError
{
    None = 0,
    TooMuchFields = 1,
    TooFewFields = 2,
    EmptyLine = 4,
    FileTruncated = 8,
    InvalidMasking = 16
}
