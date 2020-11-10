using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Lab1_PhoneBook
{
    public static class UserDialog
    {
        public const int ESCAPE_VAL = -1;
        public const string lineSeparator = "========================================================";

        public static int AcquireCommandInRange(int first, int last, bool isEscapeAllowed)
        {
            int result = ESCAPE_VAL;
            DisplayReadyInput();
            while (!int.TryParse(Console.ReadLine(), out result)
                || !(result >= first && result <= last || (isEscapeAllowed && result == ESCAPE_VAL)))                
            {
      
                Console.WriteLine("Wrong input! Please, try again...");
                DisplayReadyInput();
            }
            return result;
        }
        public static string[] AcquireStringsFormated(string[] stringNames, Regex format, string formatHelp)
        {
            string[] result = new string[stringNames.Length];
            for (int i = 0; i < stringNames.Length; ++i)
            {
                result[i] = AcquireStringFormated(stringNames[i], format, formatHelp);
            }
            return result;
        }
        public static string AcquireStringFormated(string stringName, Regex format, string formatHelp)
        {
            Console.WriteLine("Enter " + stringName + " [format: " + formatHelp + "]");
            DisplayReadyInput();
            string result = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(result) || !format.IsMatch(result))
            {
                Console.WriteLine("Bad format... Try again! [format: " + formatHelp  + "]");
                DisplayReadyInput();
                result = Console.ReadLine();
            }
            return result;
        }
        public static string AcquireAnyString(string stringName)
        {
            Console.WriteLine("Enter " + stringName);
            DisplayReadyInput();
            return Console.ReadLine();
        }

        public static DateTime AcquireDateExact(string format)
        {
            DateTime result;
            Console.WriteLine("Date format: " + format);
            DisplayReadyInput();
            string s = Console.ReadLine();
            while (!DateTime.TryParseExact(s, format, new CultureInfo("ru-RU"), DateTimeStyles.AssumeLocal, out result))
            {
                Console.WriteLine("Wrong... Try again!");
                DisplayReadyInput();
                s = Console.ReadLine();
            }
            return result;
        }
        public static bool AcquireYesOrNo(string question) /*true == yes*/
        {
            Console.WriteLine(question + " [y/n]");
            DisplayReadyInput();
            string input = Console.ReadLine();
            while (string.IsNullOrEmpty(input) || !(input == "y" || input == "n"))
            {
                input = Console.ReadLine();
                Console.WriteLine("Wroooong... Try again!");
                DisplayReadyInput();
            }
            return input == "y";
        }

        public static void DisplayGreetings(string welcomeLine)
        {
            DisplayLineSeparator();
            int diff = lineSeparator.Length - welcomeLine.Length;
            if (diff > 0)
                Console.WriteLine(welcomeLine.PadLeft(welcomeLine.Length + diff / 2));
            else
                Console.WriteLine(welcomeLine);
            DisplayLineSeparator();
        }
        public static void DisplayFramedText(string text)
        {
            DisplayLineSeparator();
            Console.WriteLine(text);
            DisplayLineSeparator();
        }
        public static void DisplayEscapeMsg()
        {
            Console.WriteLine("Escape...");
        }
        public static void DisplayNoMatchMsg()
        {
            Console.WriteLine("No matches");
        }
        public static void DisplayLineSeparator()
        {
            Console.WriteLine(lineSeparator);
        }
        public static void DisplayReadyInput()
        {
            Console.Write(">>> ");
        }
        public static void DisplayOptions(string[] options, string header, string escapeStr)
        {
            DisplayLineSeparator();
            if (header != null)
                Console.WriteLine(header);
            for (int i = 0; i < options.Length; ++i)
            {
                Console.WriteLine($"{i} - {options[i]}");
            }
            if (escapeStr != null)
            {
                Console.WriteLine($"{ESCAPE_VAL} - {escapeStr}");
            }
            DisplayLineSeparator();
        }
    }
}
