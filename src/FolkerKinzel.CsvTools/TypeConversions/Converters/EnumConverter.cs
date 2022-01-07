namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class EnumConverter<TEnum> : CsvTypeConverter<TEnum> where TEnum : struct, Enum
{
    public EnumConverter(
        bool ignoreCase = true,
        string? format = "D",
        bool throwsOnParseErrors = true,
        TEnum fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        ValidateFormat(format);
        this.IgnoreCase = ignoreCase;
        this.Format = format;
    }

    internal static ICsvTypeConverter Create(CsvConverterOptions options, bool ignoreCase, string? format, TEnum fallbackValue)
        => new EnumConverter<TEnum>(ignoreCase, format, options.HasFlag(CsvConverterOptions.Throwing), fallbackValue)
        .HandleNullableAndDBNullAcceptance(options);

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

    internal bool IgnoreCase { get; }
    internal string? Format { get; }

    protected override string? DoConvertToString(TEnum value) => value.ToString(Format);

    public override bool TryParseValue(string value, out TEnum result) => Enum.TryParse<TEnum>(value, IgnoreCase, out result);
}
