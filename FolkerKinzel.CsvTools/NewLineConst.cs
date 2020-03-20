using System;

namespace FolkerKinzel
{
    namespace Csv
    {
        //Die Klasse nutzt das "Safe-Enum-Pattern".

        /// <summary>
        /// Enthält Konstanten für NewLine-Zeichen.
        /// </summary>
        public sealed class NewLineConst
        {
            /// <summary>
            /// NewLine-Zeichen auf Windows-Systemen ("\r\n").
            /// </summary>
            public static readonly NewLineConst Windows = new NewLineConst("\r\n");

            /// <summary>
            /// NewLine-Zeichen auf Mac und Linux-Systemen ("\n").
            /// </summary>
            public static readonly NewLineConst Unix = new NewLineConst("\n");


            /// <summary>
            /// Das auf dem aktuellen System eingestellte NewLine-Zeichen (Environment.NewLine).
            /// </summary>
            public static readonly NewLineConst Default = new NewLineConst(Environment.NewLine);


            /// <summary>
            /// privater Konstruktor
            /// </summary>
            /// <param name="value">string, der das NewLine-Zeichen enthält</param>
            private NewLineConst(string value) { Value = value; }

            /// <summary>
            /// Gibt den String mit dem NewLine-Zeichen zurück, der in der Konstante steckt.
            /// </summary>
            internal string Value { get; private set; }
        }//class
    }//namespace
}
