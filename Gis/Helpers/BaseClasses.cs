using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace Gis.Helpers.BaseClasses
{
    class BaseClasses
    {
        /// <summary>
        /// Форматированный вывод успешного сообщения
        /// </summary>
        /// <param name="string1">Текст сообщения</param>
        public static void OutputMessage(string string1)
        {
            Console.WriteLine("{0}", string1);
            WriteMessage(string1);
        }

        /// <summary>
        /// Форматированный вывод сообщения об ошибке
        /// </summary>
        /// <param name="string1">Код сообщения</param>
        /// <param name="string2">Описание сообщения</param>
        public static void OutputError(string string1, [Optional] string string2)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if(string2 is null)
            {
                Console.WriteLine("{0}", string1);
                WriteMessage(string1);
            }
            else
            {
                Console.WriteLine("{0}: {1}", string1, string2);
                WriteMessage(string1 + ": " + string2);
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Запись сообщения в лог
        /// </summary>
        /// <param name="StringMessage">Текст сообщения</param>
        private static void WriteMessage(string StringMessage)
        {
            File.AppendAllText(@"ImportSettlements.csv", DateTime.Now + "," + StringMessage + ";" + Environment.NewLine, Encoding.Default);
        }
    }
}
