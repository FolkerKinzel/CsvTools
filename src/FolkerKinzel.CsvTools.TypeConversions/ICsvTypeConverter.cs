namespace FolkerKinzel.CsvTools.TypeConversions;

public interface ICsvTypeConverter
{
    object? Parse(ReadOnlySpan<char> value);

    string? ConvertToString(object? value);

    bool Throwing { get; }

    object? FallbackValue { get; }

    bool AcceptsNull { get; }
}
