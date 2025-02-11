using FolkerKinzel.CsvTools;

namespace Examples;

internal static class CsvStringExample
{
    public static void ConvertingCsvStrings()
    {
        object?[][] data =
        [
            [ "Color", "Direction", "Number"],
            [ "brown", "right", 0],
            ["red", "left", 42],
            [null, "Somewhere \"over\" the Rainbow"],
            ["""
             Blue
             in 
             Green
             """, "Up, or down", -3.14]
        ];

        string csv = data.ToCsv();

        Console.WriteLine(csv);
        Console.WriteLine();

        CsvRecord[] results = Csv.ParseAnalyzed(csv);

        foreach (CsvRecord record in results)
        {
            Console.WriteLine(record);
        }
    }
}

/*
Console Output:

Color,Direction,Number
brown,right,0
red,left,42
,"Somewhere ""over"" the Rainbow",
"Blue
in
Green","Up, or down",-3.14

Color: brown, Direction: right, Number: 0
Color: red, Direction: left, Number: 42
Color: , Direction: Somewhere "over" the Rainbow, Number:
Color: Blue
in
Green, Direction: Up, or down, Number: -3.14
*/
