using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class GuidConverter : CsvTypeConverter<Guid>
{
    private readonly string _format;

    public GuidConverter(bool throwing = true) : base(throwing) => _format = "D";

    public GuidConverter(
        string? format,
        bool throwing = true) : base(throwing, default)
    {
        _format = format ?? string.Empty;
        ExamineFormat(nameof(format));
    }

    public override bool AcceptsNull => false;


    protected override string? DoConvertToString(Guid value) => value.ToString(_format, CultureInfo.InvariantCulture);


    public override bool TryParseValue(ReadOnlySpan<char> value, out Guid result)
#if NET461 || NETSTANDARD2_0
        => Guid.TryParse(value.ToString(), out result);
#else
        => Guid.TryParse(value, out result);
#endif


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
