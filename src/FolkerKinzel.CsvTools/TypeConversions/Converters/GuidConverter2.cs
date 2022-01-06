using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class GuidConverter2 : CsvTypeConverter<Guid>
{
    private readonly string _format;

    private GuidConverter2(bool throwsOnParseErrors) : base(throwsOnParseErrors) => _format = "D";

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options)
        => new GuidConverter2(options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
        .HandleNullableAndDBNullAcceptance(options);


    public GuidConverter2(
        string? format,
        bool throwOnParseErrors = false) : base(throwOnParseErrors, default)
    {
        _format = format ?? string.Empty;
        ExamineFormat(nameof(format));
    }


    protected override string? DoConvertToString(Guid value) => value.ToString(_format, CultureInfo.InvariantCulture);


    public override bool TryParseValue(string value, [NotNullWhen(true)] out Guid result)
        => Guid.TryParse(value, out result);


    private void ExamineFormat(string parameterName)
    {
        try
        {
            _ = Guid.Empty.ToString(_format, CultureInfo.InvariantCulture);
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, parameterName, e);
        }
    }
}
