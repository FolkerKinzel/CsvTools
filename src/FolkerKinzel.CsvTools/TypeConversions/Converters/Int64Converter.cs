using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

//public sealed class NumberConverter2<T> : CsvTypeConverter<T> where T : struct, IConvertible
//{
//    public NumberConverter2(IFormatProvider? formatProvider, bool throwsOnParseErrors, T fallbackValue = default)
//        : base(throwsOnParseErrors, fallbackValue) => FormatProvider = formatProvider;

//    internal static ICsvTypeConverter2 Create(bool nullable, bool acceptsDBNull, IFormatProvider? formatProvider, bool throwsOnParseErrors)
//        => new NumberConverter2<T>(formatProvider, throwsOnParseErrors).HandleNullableAndDBNullAcceptance(nullable, acceptsDBNull);

//    internal IFormatProvider? FormatProvider { get; }

//    protected override string? DoConvertToString(T value) => Convert.ToString(value, FormatProvider);

//    public override bool TryParseValue(string value, [NotNullWhen(true)] out T result)
//    {
//        try
//        {
//            result = (T)Convert.ChangeType(value, typeof(T), FormatProvider);
//            return true;
//        }
//        catch
//        {
//            result = default;
//            return false;
//        }
//    }
//}

public sealed class Int64Converter : CsvTypeConverter<long>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public Int64Converter(IFormatProvider? formatProvider = null,
                          bool throwsOnParseErrors = true,
                          NumberStyles styles = DEFAULT_NUMBER_STYLE,
                          long fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options,
                                              IFormatProvider? formatProvider,
                                              bool hexConverter)
        => new Int64Converter(formatProvider,
                              options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors),
                              hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
           .HandleNullableAndDBNullAcceptance(options);

    protected override string? DoConvertToString(long value) => value.ToString(_format, _formatProvider);

    public override bool TryParseValue(string value, out long result) => long.TryParse(value, _styles, _formatProvider, out result);

}
