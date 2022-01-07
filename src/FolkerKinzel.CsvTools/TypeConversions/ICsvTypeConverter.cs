namespace FolkerKinzel.CsvTools.TypeConversions;

public interface ICsvTypeConverter
{
    object? Parse(string? value);

    string? ConvertToString(object? value);

    bool Throwing { get; }

    object? FallbackValue { get; }
}
