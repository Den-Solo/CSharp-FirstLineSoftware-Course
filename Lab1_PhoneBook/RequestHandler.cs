using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Lab1_PhoneBook
{
    public class RequestHandler
    {
        private const int ESCAPE_VAL = UserDialog.ESCAPE_VAL; 
        private static CultureInfo cultInfo = new CultureInfo("ru-RU");
        private PhoneBook phoneBook;

        public RequestHandler(PhoneBook pb)
        {
            this.phoneBook = pb;
        }


        private static string[] nameOrPhoneMenu = new string[] {
            "Find by surname and name", //0
            "Find by phone number"      //1
        };
        public IReadOnlyList<Tuple<int,PhoneBook.Note>> AcquireNotesByNameOrPhone(bool isEscapeAllowed)
        {
            UserDialog.DisplayOptions(nameOrPhoneMenu,"Enter option", isEscapeAllowed ? "To escape" : null);
            IReadOnlyList <Tuple<int, PhoneBook.Note>> notes = null;
            switch (UserDialog.AcquireCommandInRange(0, 1, isEscapeAllowed))
            {
                case ESCAPE_VAL:
                    notes = null;
                    break;
                case 0:
                    string surname;
                    string name;
                    AcquireSurnameAndName(out surname, out name);
                    notes = phoneBook.GetNotes(surname, name);
                    break;
                case 1:
                    string phoneNumber = AcquirePhoneNumberToCheck();
                    notes = phoneBook.GetNotes(phoneNumber);
                    break;
            }
            return notes;
        }

        public IReadOnlyList<Tuple<int, PhoneBook.Note>> AcquireNoteOfEqual(IReadOnlyList<Tuple<int, PhoneBook.Note>> notes, bool isEscapeAllowed)
        {
            if (notes == null)
            {
                throw new ArgumentOutOfRangeException("not allowed to send null list");
            }
            Console.WriteLine("List contains several equal Notes:");
            DisplayShortInfo(notes);
            Console.Write("Enter ID to perform action ");
            if (isEscapeAllowed)
            {
                Console.Write($"or {ESCAPE_VAL} to escape");
            }
            Console.WriteLine();
            int idx =  UserDialog.AcquireCommandInRange(notes.First().Item1, notes.Last().Item1,isEscapeAllowed);
            if (idx == ESCAPE_VAL)
            {
                return null;
            }
            return notes.Where(x => x.Item1 == idx).ToList();
        }
        public static void AcquireSurnameAndName(out string surname, out string name)
        {
            string[] surnameAndName = UserDialog.AcquireStringsFormated(
                new string[] { "surname", "name" }, PhoneBook.NameFilter, "letters only");
            surname = surnameAndName[0];
            name = surnameAndName[1];
        }
        public string AcquirePhoneNumberToAdd() // acceptes only not existing numbers
        {
            string pn = null;
            string filler = "";
            while (true)
            {
                pn = UserDialog.AcquireStringFormated(filler + "phone number",
                    PhoneBook.PhoneNumberFilter, "digits only");
                if (!phoneBook.Exists(pn))
                {
                    Console.WriteLine("This phone number already exists. Impossible to add");
                    filler = "another ";
                }
                else
                {
                    break;
                }
            }
            return pn;
        }
        public string AcquirePhoneNumberToCheck()
        {
            return UserDialog.AcquireStringFormated("phone number",
                    PhoneBook.PhoneNumberFilter, "digits only");
        }



        private void AddNote()
        {
            Console.WriteLine("Adding new note...");
            string surname;
            string name;
            AcquireSurnameAndName(out surname, out name);
            var notes = phoneBook.GetNotes(surname, name);

            if (notes != null && notes.Count == 1)
            {
               
                if (!UserDialog.AcquireYesOrNo("One Note with the same Name and Surname already exists\nDo you want to create new?"))
                {
                    if (UserDialog.AcquireYesOrNo("Do you want to Edit existing?"))
                    {
                        EditNoteInfo(notes[0]);
                    }   
                    return;
                }
            }
            else if (notes != null && notes.Count > 1)
            {
                if (!UserDialog.AcquireYesOrNo("There are several Notes with the same Name and Surname\nDo you want to create new?"))
                {
                    if (UserDialog.AcquireYesOrNo("Do you want to Edit one of existing?"))
                    {
                        EditNoteInfo(AcquireNoteOfEqual(notes, false).ElementAt(0));
                    }
                    return;
                }
            }
            /*creating new note*/
            PhoneBook.Note newNote = new PhoneBook.Note() { name = name, surname = surname, phoneNumber = AcquirePhoneNumberToAdd() };
            EditNote(newNote);
            if (PhoneBook.Result.Success != phoneBook.InsertNote(newNote))
            {
                throw new Exception("Insert Note error while adding");
            }
        }
        private void EditNote()
        {
            Console.WriteLine("Editing existing note..");
            var notes = AcquireNotesByNameOrPhone(true);
            if (notes == null)
            {
                UserDialog.DisplayEscapeMsg();
                return;
            }
            if (notes.Count == 0)
            {
                UserDialog.DisplayNoMatchMsg();
                return;
            }
            if (notes.Count > 1)
            {
                notes = AcquireNoteOfEqual(notes, true);
                if (notes == null)
                {
                    return;
                }
            }
            EditNoteInfo(notes[0]);
        }



        private static string[] editOptions = new string[] {
                "Set surname",                  //0
                "Set name",                     //1
                "Set lastname",                 //2
                "Set birthDay",                 //3
                "Set phone number",             //4
                "Set country",                  //5
                "Set organisation",             //6
                "Set position",                 //7
                "Set addtional notes",          //8
                "Done!"                         //9
        };
        private void EditNoteInfo(in Tuple<int,PhoneBook.Note> noteInfo)
        {
            PhoneBook.Note noteToEdit = phoneBook.ExtractNote(noteInfo.Item1);
            EditNote(noteToEdit); // assume we have provided all checks inside EditNote()
            if (PhoneBook.Result.Success != phoneBook.InsertNote(noteToEdit))
            {
                throw new Exception("InsertNote error");
            }
        }
        private void EditNote(PhoneBook.Note note)
        {

            while (true) {
                UserDialog.DisplayOptions(editOptions, "Enter number to edit...", null);

                switch (UserDialog.AcquireCommandInRange(0, editOptions.Length - 1,false))
                {
                    case 0:
                        note.surname = UserDialog.AcquireStringFormated("surname",PhoneBook.NameFilter,"letters only");
                        break;
                    case 1:
                        note.name = UserDialog.AcquireStringFormated("name", PhoneBook.NameFilter, "letters only"); 
                        break;
                    case 2:
                        note.lastname = UserDialog.AcquireStringFormated("lastname", PhoneBook.NameFilter, "letters only");
                        break;
                    case 3:
                        Console.WriteLine("Enter birthdate:");
                        note.birthDate = UserDialog.AcquireDateExact("dd.MM.yyyy");
                        break;
                    case 4:
                        note.phoneNumber = AcquirePhoneNumberToAdd();
                        break;
                    case 5:
                        note.country = UserDialog.AcquireStringFormated("country name", PhoneBook.NameFilter, "letters only");
                        break;
                    case 6:
                        note.organization = UserDialog.AcquireAnyString("organization");
                        break;
                    case 7:
                        note.position = UserDialog.AcquireAnyString("Job Position");
                        break;
                    case 8:
                        note.additionalNotes = UserDialog.AcquireAnyString("additional notes");
                        break;
                    case 9:
                        return;
                }
            }
        }

        private void DeleteNote()
        {
            Console.WriteLine("Deleting note...");
            var notes = AcquireNotesByNameOrPhone(true);
            if (notes == null)
            {
                UserDialog.DisplayEscapeMsg();
                return;
            }
            if (notes.Count == 0)
            {
                UserDialog.DisplayNoMatchMsg();
                return;
            }
            else if (notes.Count > 1)
            {
                notes = AcquireNoteOfEqual(notes, true);
                if (notes == null)
                {
                    return;
                }
            }
            phoneBook.ExtractNote(notes[0].Item1); // deleted from phoneBook
            Console.WriteLine("Deleted successfully");
        }


        private void DisplayFullInfo()
        {
            Console.WriteLine("Displaying full info...");
            var notes = AcquireNotesByNameOrPhone(true);
            if (notes == null)
            {
                UserDialog.DisplayEscapeMsg();
                return;
            }
            if (notes.Count == 0)
            {
                UserDialog.DisplayNoMatchMsg();
                return;
            }
            if (notes.Count > 1)
            {
                notes = AcquireNoteOfEqual(notes, true);
                if (notes == null)
                {
                    return;
                }
            }
        
            var note = notes[0].Item2;
            Console.WriteLine("ID: ".PadRight(20) + notes[0].Item1.ToString().PadLeft(20));
            Console.WriteLine("surname: ".PadRight(20) + note.surname.PadLeft(20));
            Console.WriteLine("name: ".PadRight(20) + note.name.PadLeft(20));
            if (!string.IsNullOrEmpty(note.lastname))
                Console.WriteLine("lastname: ".PadRight(20) + note.lastname.PadLeft(20));
            Console.WriteLine("phone number: ".PadRight(20) + note.phoneNumber.PadLeft(20));
            if (!string.IsNullOrEmpty(note.country))
                Console.WriteLine("country: ".PadRight(20) + note.country.PadLeft(20));
            if (note.birthDate != default(DateTime))
                Console.WriteLine("birthdate: ".PadRight(20) + note.birthDate.ToString("dd.MM.yyyy").PadLeft(20));
            if (!string.IsNullOrEmpty(note.organization))
                Console.WriteLine("organization: ".PadRight(20) +  note.organization.PadLeft(20));
            if (!string.IsNullOrEmpty(note.position))
                Console.WriteLine("position: ".PadRight(20) + note.position.PadLeft(20));
            if (!string.IsNullOrEmpty(note.additionalNotes))
                Console.WriteLine("additional notes: ".PadRight(20) + note.additionalNotes.PadLeft(20));
            Console.WriteLine();
   
        }


        private void DisplayShortInfo(in IReadOnlyList<Tuple<int, PhoneBook.Note>> notes)
        {
            if (notes == null || notes.Count == 0)
            {
                Console.WriteLine("List is absolutely empty :(");
                return;
            }

            Console.WriteLine("Displaying short info...\n");
            Console.WriteLine($"ID      surname       name      phone number");
            foreach (var info in notes)
            {
                Console.WriteLine($"{info.Item1} {info.Item2.surname.PadLeft(13)} {info.Item2.name.PadLeft(10)} {info.Item2.phoneNumber.PadLeft(17)}");
            }
        }


        private bool ConfirmExit() // if exit then true
        {
            return UserDialog.AcquireYesOrNo("Exit: Are you sure?");
        }



        private static string[] mainMenuOptions = new string[] {
                "Add new note",                           //0
                "Edit existing note",                     //1
                "Delete note",                            //2
                "Get one note full info",                 //3
                "Look through all notes main info",       //4
                "Close app"                               //5
        };                                                
                                                          
        public bool ActionChooser()
        {
            UserDialog.DisplayOptions(mainMenuOptions, "Enter number to proceed...", null);

            switch (UserDialog.AcquireCommandInRange(0,mainMenuOptions.Length - 1, false)) 
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
                    DisplayShortInfo(phoneBook.GetAllNotes());
                    break;
                case 5:
                    return !ConfirmExit();
            }
            return true;
        }
 

    }

}
