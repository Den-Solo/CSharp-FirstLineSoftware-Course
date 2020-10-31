using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab1_PhoneBook
{
    public partial class PhoneBook
    {
        public class Note : IComparable
        {
            public string surname;
            public string name;
            public string lastname;
            public string phoneNumber;
            public string country;
            public DateTime birthDate;
            public string organization;
            public string position;
            public string additionalNotes;

            public int CompareTo(object obj)
            {
                if (obj == null) return 1;
                int result = 0;
                Note otherNote = obj as Note;

                result = string.Compare(this.surname,otherNote.surname);
                if (result != 0) return result;
                result = string.Compare(this.name,otherNote.name);
                if (result != 0) return result;
                result = string.Compare(this.lastname, otherNote.lastname);
                if (result != 0) return result;
                result = string.Compare(this.phoneNumber,otherNote.phoneNumber);
                if (result != 0) return result;
                result = string.Compare(this.country,otherNote.country);
                if (result != 0) return result;
                result = DateTime.Compare(this.birthDate,otherNote.birthDate);
                if (result != 0) return result;
                result = string.Compare(this.organization,otherNote.organization);
                if (result != 0) return result;
                result = string.Compare(this.position,otherNote.position);
                if (result != 0) return result;
                result = string.Compare(this.position,otherNote.position);
                if (result != 0) return result;
                return 0;
                //microsoft should have implement tuple as std::tuple
                //with all possible compare operators, lengths and types
                //hard to believe but c++ STL sometimes is a lot more advanced
            }
        }
        
        private List<Note> notes = new List<Note>(); // slow to search if many Notes but support repetitive surnames+names
        private static Regex digitsOnly = new Regex("^[0-9]+$");
        private static Regex lettersOnly = new Regex("^[a-zA-ZА-Яа-я]+$");
        private static CultureInfo cultInfo = new CultureInfo("ru-RU");

        public void AddNote()
        {
            string name;
            string surname;

            Console.WriteLine("Add new person");
            AcquireSurnameAndName(out surname, out name);

            int lb = 0, ub= 0;
            GetNoteIdx(surname,name,out lb,out ub);

            if (lb != -1 && lb + 1 == ub)
            {
                Console.WriteLine("Note with such Name and Surname already exists\nDo you want to create new? [y/n]");
                if (!AcquireYesOrNo())
                {
                    Console.WriteLine("Do you want to Edit info?[y/n]");
                    if (AcquireYesOrNo())
                        EditNote(notes[lb]);
                    return;
                }
            }
            else if (lb + 1 != ub)
            {
                Console.WriteLine("There several Notes with such Name and Surname\nDo you want to create new? [y/n]");
                if (!AcquireYesOrNo())
                {
                    Console.WriteLine("Do you want to Edit one of existing?[y/n]");
                    if (AcquireYesOrNo())
                        EditNote(notes[AcquireIdxOfEqual(lb, ub)]);
                    return;
                }
            }
            /*creating new notes*/
            this.notes.Add(new Note() { name = name,surname = surname, 
                phoneNumber = AcquirePhoneNumber() });
            EditNote(notes.Last());
            this.notes.Sort();
        }
        public void EditNote()
        {
            int note_idx = AcquireNoteIdxByNameOrPhone();
            if (note_idx == -1)
            {
                Console.WriteLine("No matches");
                return;
            }
            EditNote(notes[note_idx]);
            notes.Sort();
        }
        public void EditNote(Note note)
        {

            while (true) {
                Console.WriteLine("\n========================================================");
                Console.WriteLine("Enter number to edit...");
                Console.WriteLine("0 - Set surname");
                Console.WriteLine("1 - Set name");
                Console.WriteLine("2 - Set lastname");
                Console.WriteLine("3 - Set birthDay");
                Console.WriteLine("4 - Set phone number");
                Console.WriteLine("5 - Set country");
                Console.WriteLine("6 - Set organisation");
                Console.WriteLine("7 - Set position");
                Console.WriteLine("8 - Set addtional notes");
                Console.WriteLine("9 - Done!");
                Console.WriteLine("========================================================");

                switch (AcquireCommandInRange(0, 9))
                {
                    case 0:
                        Console.WriteLine("Enter Surname:");
                        note.surname = AcquireNotEmptyString();
                        break;
                    case 1:
                        Console.WriteLine("Enter Name:");
                        note.name = AcquireNotEmptyString();
                        break;
                    case 2:
                        Console.WriteLine("Enter Lastname:");
                        note.lastname = AcquireNotEmptyString();
                        break;
                    case 3:
                        Console.WriteLine("Enter birthdate:");
                        note.birthDate = AcquireDate();
                        break;
                    case 4:
                        note.phoneNumber = AcquirePhoneNumber();
                        break;
                    case 5:
                        Console.WriteLine("Enter Country:");
                        note.country = AcquireNotEmptyString();
                        break;
                    case 6:
                        Console.WriteLine("Enter Organization:");
                        note.organization = AcquireNotEmptyString();
                        break;
                    case 7:
                        Console.WriteLine("Enter Job Position:");
                        note.position = AcquireNotEmptyString();
                        break;
                    case 8:
                        Console.WriteLine("Enter additional notes:");
                        note.additionalNotes = AcquireAnyString();
                        break;
                    case 9:
                        return;
                }
            }
        }

        public void DeleteNote()
        {
            int note_idx = AcquireNoteIdxByNameOrPhone();
            if (note_idx == -1)
            {
                Console.WriteLine("No matches");
                return;
            }
            notes.RemoveAt(note_idx);
            Console.WriteLine("Deleted successfully");
        }


        public void DisplayFullInfo()
        {

            int note_idx = AcquireNoteIdxByNameOrPhone();
            Console.WriteLine();
            if (note_idx != -1)
            {
                Note note = notes[note_idx];
                Console.WriteLine($"ID: {note_idx}");
                Console.WriteLine($"surname: {note.surname}");
                Console.WriteLine($"name: {note.name}");
                if (!string.IsNullOrEmpty(note.lastname))
                    Console.WriteLine($"lastname: {note.lastname}");
                Console.WriteLine($"phone number:  {note.phoneNumber}");
                if (!string.IsNullOrEmpty(note.country))
                    Console.WriteLine($"country: {note.country}");
                if (note.birthDate != default(DateTime))
                    Console.WriteLine($"birthdate: {note.birthDate.ToString("dd.MM.yyyy")}");
                if (!string.IsNullOrEmpty(note.organization))
                    Console.WriteLine($"organization: {note.organization}");
                if (!string.IsNullOrEmpty(note.position))
                    Console.WriteLine($"position: {note.position}");
                if (!string.IsNullOrEmpty(note.additionalNotes))
                    Console.WriteLine($"additional notes: {note.additionalNotes}");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No matches");
            }
        }


        public void DisplayShortInfo(int beg, int end)
        {
            if (end == 0)
                end = this.notes.Count();
            if (notes.Count() > 0)
            {
                int idx = 0;
                Console.WriteLine($"ID      surname      name       phone number");
                for (int i = beg; i < end; ++i)
                {
                    Console.WriteLine($"{idx++} {notes[i].surname.PadLeft(13)} {notes[i].name.PadLeft(10)} {notes[i].phoneNumber.PadLeft(17)}");
                }
            }
            else
            {
                Console.WriteLine("List is absolutely empty :(");
            }
        }

 
        public bool ConfirmExit()
        {
            Console.WriteLine("Are you sure? [y/n]");
            return AcquireYesOrNo();
        }
        public bool ActionChooser()
        {
            Console.WriteLine("\n========================================================");
            Console.WriteLine("Enter number to start action...");
            Console.WriteLine("0 - Add new note");
            Console.WriteLine("1 - Edit existing note");
            Console.WriteLine("2 - Delete note");
            Console.WriteLine("3 - Look through all notes full info");
            Console.WriteLine("4 - Look through all notes main info");
            Console.WriteLine("5 - Close app");
            Console.WriteLine("========================================================");

            switch (AcquireCommandInRange(0,5)) 
            {
                case 0:
                    AddNote();
                    break;
                case 1:
                    EditNote();
                    break;
                case 2:
                    DeleteNote();
                    break;
                case 3:
                    DisplayFullInfo();
                    break;
                case 4:
                    DisplayShortInfo(0,0);
                    break;
                case 5:
                    return !ConfirmExit();
            }
            return true;
        }

        static void Main(string[] args)
        {
            PhoneBook pb = new PhoneBook();
            Console.WriteLine("========================================================");
            Console.WriteLine("               Welcome to my phone book app             ");
            Console.WriteLine("========================================================\n");
            while (pb.ActionChooser());
        }
    }
}
