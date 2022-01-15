namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class EnumConverter<TEnum> : CsvTypeConverter<TEnum> where TEnum : struct, Enum
{
    private const string DEFAULT_FORMAT = "D";

    public EnumConverter(
        bool throwing = true,
        TEnum fallbackValue = default,
        bool ignoreCase = true)
        : base(throwing, fallbackValue) => IgnoreCase = ignoreCase;

    public EnumConverter(
        string? format,
        bool throwing = true,
        TEnum fallbackValue = default,
        bool ignoreCase = true)
        : base(throwing, fallbackValue)
    {
        ValidateFormat(format);
        IgnoreCase = ignoreCase;
        Format = format;
    }


    private static void ValidateFormat(string? format)
    {
        if (format is null || format.Length == 0)
        {
            return;
        }

        if (format.Length == 1)
        {
            switch (char.ToUpperInvariant(format[0]))
            {
                case 'G':
                case 'D':
                case 'X':
                case 'F':
                    return;
            }
        }

        throw new ArgumentException("Invalid format string.", nameof(format));
    }

    public bool IgnoreCase { get; }
    public string? Format { get; } = DEFAULT_FORMAT;

    protected override string? DoConvertToString(TEnum value) => value.ToString(Format);

    public override bool TryParseValue(ReadOnlySpan<char> value, out TEnum result)
#if NET461 || NETSTANDARD2_0 || NETSTANDARD2_1 || NET5_0
        => Enum.TryParse<TEnum>(value.ToString(), IgnoreCase, out result);
#else
        => Enum.TryParse<TEnum>(value, IgnoreCase, out result);
#endif
}
