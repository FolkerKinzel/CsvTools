namespace FolkerKinzel.CsvTools.TypeConversions;

public interface ICsvTypeConverter2
{
    object? Parse(string? value);

    string? ConvertToString(object? value);

    bool ThrowsOnParseErrors { get; }

    //object? FallbackValue { get; }
}
