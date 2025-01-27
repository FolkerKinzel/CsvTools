using FolkerKinzel.CsvTools;

namespace Examples;

internal static class StringExample
{
    public static void CsvExample()
    {
        string[][] data =
        [
            [ "Color", "Direction"],
            [ "brown", "right"],
            ["red", "left"],
            ["AliceBlue", "Somewhere \"over\" the Rainbow"],
            ["""
             Blue
             in 
             Green
             """, "Up, or down"]
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

Color,Direction
brown,right
red,left
AliceBlue,"Somewhere ""over"" the Rainbow"
"Blue
in
Green","Up, or down"

Color: brown, Direction: right
Color: red, Direction: left
Color: AliceBlue, Direction: Somewhere "over" the Rainbow
Color: Blue
in
Green, Direction: Up, or down
*/
