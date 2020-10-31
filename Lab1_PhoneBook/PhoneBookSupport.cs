using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_PhoneBook
{
    public partial class PhoneBook
    {
        public Note GetNote(string surname, string name)
        {
            return notes.Find(note => note.name == name && note.surname == surname);
        }
        public Note GetNote(string phoneNumber)
        {
            return notes.Find(note => note.phoneNumber == phoneNumber);
        }
        public void GetNoteIdx(string surname, string name,out int lowerBound, out int upperBound)
        {
            //absolutely ineffective search on sorted array
            //but 
            //where is std::equal_range or std::lower_bound and std::upper_bound??? I so miss c++
            lowerBound = notes.FindIndex(note => note.name == name && note.surname == surname);
            upperBound = notes.FindLastIndex(note => note.name == name && note.surname == surname) + 1;
        }
        public void GetNoteIdx(string phoneNumber, out int lowerBound, out int upperBound)
        {
            lowerBound = notes.FindIndex(note => note.phoneNumber == phoneNumber);
            upperBound = notes.FindLastIndex(note => note.phoneNumber == phoneNumber) + 1;
        }
        public int AcquireNoteIdxByNameOrPhone()
        {
            Console.WriteLine("Please, Enter...");
            Console.WriteLine("0 - Find by surname and name");
            Console.WriteLine("1 - Find by phone number");
            int lb = -1, ub = -1;
            switch (AcquireCommandInRange(0, 1))
            {
                case 0:
                    string surname;
                    string name;
                    AcquireSurnameAndName(out surname, out name);
                    GetNoteIdx(surname, name,out lb,out ub);
                    break;
                case 1:
                    string phoneNumber = AcquirePhoneNumber();
                    GetNoteIdx(phoneNumber,out lb, out ub);
                    break;
            }
            if (lb + 1 == ub)
                return lb;
            else /*contains equal fields*/
            {
                return AcquireIdxOfEqual(lb, ub);
            }
        }
        public int AcquireIdxOfEqual(int lowerBound,int upperBound)
        {
            Console.WriteLine("List contains several equal Notes:");
            DisplayShortInfo(lowerBound, upperBound);
            Console.WriteLine("Enter ID to perform action");
            return AcquireCommandInRange(lowerBound, upperBound - 1);
        }
        public static int AcquireCommandInRange(int first, int last)
        {
            int result;
            Console.Write("\n>>> ");
            while (!int.TryParse(Console.ReadLine(), out result) || !(result >= first && result <= last))
            {
                Console.WriteLine("Wrong input! Please, try again...");
                Console.Write("\n>>> ");
            }
            return result;
        }
        public static string AcquirePhoneNumber()
        {
            Console.Write("Enter phone number (Only digits allowed)");
            string result;
            while (true)
            {
                Console.Write("\n>>> ");
                result = Console.ReadLine();
                if (digitsOnly.IsMatch(result))
                    break;
                Console.WriteLine("Incorrect phone number\nOnly digits allowed");
            }
            return result;
        }
        public static void AcquireSurnameAndName(out string surname, out string name)
        {
            Console.WriteLine("Enter surname");
            surname = AcquireNotEmptyString();
            Console.WriteLine("Enter name");
            name = AcquireNotEmptyString();
        }
        public static string AcquireNotEmptyString()
        {
            Console.Write(">>> ");
            string result = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(result) || !lettersOnly.IsMatch(result))
            {
                Console.WriteLine("Empty or wrong chars... Try again!");
                Console.Write(">>> ");
                result = Console.ReadLine();
            }
            return result;
        }
        public static string AcquireAnyString()
        {
            Console.Write(">>> ");
            return Console.ReadLine();
        }
        public static DateTime AcquireDate()
        {
            DateTime result;
            Console.WriteLine("Date format: dd.MM.YYYY");
            Console.Write(">>> ");
            string s = Console.ReadLine();
            while (!DateTime.TryParseExact(s,"dd.MM.yyyy", cultInfo,DateTimeStyles.AssumeLocal,out result))
            {
                Console.WriteLine("Wrong... Try again!");
                Console.Write(">>> ");
                s = Console.ReadLine();
            }
            return result;
        }
        public static bool AcquireYesOrNo() /*true == yes*/
        {
            string input;
            while ((input = Console.ReadLine()).Length < 0 || !(input == "y" || input == "n"))
            {
                Console.WriteLine("Wroooong... Try again!");
                Console.Write(">>> ");
            }
            return input == "y";
        }
    }
}
