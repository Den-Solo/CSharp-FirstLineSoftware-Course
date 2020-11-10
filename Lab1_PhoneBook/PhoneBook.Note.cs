using System;

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

                result = string.Compare(this.surname, otherNote.surname);
                if (result != 0) return result;
                result = string.Compare(this.name, otherNote.name);
                if (result != 0) return result;
                result = string.Compare(this.lastname, otherNote.lastname);
                if (result != 0) return result;
                result = string.Compare(this.phoneNumber, otherNote.phoneNumber);
                if (result != 0) return result;
                result = DateTime.Compare(this.birthDate, otherNote.birthDate);
                if (result != 0) return result;
                return 0;
            }

        }
    }
}
