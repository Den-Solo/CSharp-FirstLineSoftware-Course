using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab1_PhoneBook
{
    
    public partial class PhoneBook
    {
        public enum Result
        {
            Success,
            InvalidPhoneNumber,
            InvalidSurname,
            InvalidName
        }
        public static Regex PhoneNumberFilter { get; private set; } = new Regex("^[0-9]+$");
        public static Regex NameFilter { get; private set; } = new Regex("^[a-zA-ZА-Яа-я]+$");
        private List<Note> notesSortedByName = new List<Note>();
        private Dictionary<string, Note> notesByPhoneNumber = new Dictionary<string, Note>();

        private void _GetNoteIdx(string surname, string name,out int lowerBound, out int upperBound)
        {
            //absolutely ineffective search on sorted array
            //but 
            //where is std::equal_range or std::lower_bound and std::upper_bound???
            lowerBound = notesSortedByName.FindIndex(note => note.name == name && note.surname == surname);
            upperBound = notesSortedByName.FindLastIndex(note => note.name == name && note.surname == surname) + 1;
           // (lowerBound, upperBound) = EqualRange(notes, (x,y) => -1 == Note.Compare(surname,name,x));
        }
        private int _GetNoteIdx(string phoneNumber)
        {
            Note n = _GetNote(phoneNumber);
            if (n == null)
            {
                return -1;
            }
            int lb = -1, ub = -1;
            _GetNoteIdx(n.surname, n.name, out lb, out ub);
            for (; lb < ub; ++lb)
            {
                if (phoneNumber == notesSortedByName[lb].phoneNumber)
                {
                    return lb;
                } 
            }
            return -1;
        }
        private Note _GetNote(string phoneNumber)
        {
            if (!notesByPhoneNumber.ContainsKey(phoneNumber))
            {
                return null;
            }
            return notesByPhoneNumber[phoneNumber];
        }

        public bool Exists(string phoneNumber)
        {
            return !notesByPhoneNumber.ContainsKey(phoneNumber);
        }


        public IReadOnlyList<Tuple<int, Note>> GetNotes(string surname, string name)
        {
            int lb = -1, ub = -1;
            List<Tuple<int, Note>> result = new List<Tuple<int, Note>>();
            _GetNoteIdx(surname, name, out lb, out ub);
            if (lb == -1)
            {
                return result; //empty
            }
            for (; lb < ub; ++lb)
            {
                result.Add(new Tuple<int,Note>(lb, notesSortedByName[lb]));
            }
            return result;
        }
        public IReadOnlyList<Tuple<int, Note>> GetAllNotes()
        {
            List<Tuple<int, Note>> result = new List<Tuple<int, Note>>();
            for (int i =0; i < notesSortedByName.Count; ++i)
            {
                result.Add(new Tuple<int, Note>(i, notesSortedByName[i]));
            }
            return result;
        }
        public IReadOnlyList<Tuple<int, Note>> GetNotes(string phoneNumber)
        {
            int idx = _GetNoteIdx(phoneNumber);
            if (idx == -1)
            {
                return new List<Tuple<int, Note>>();
            }
            return new List<Tuple<int, Note>>() { new Tuple<int, Note>(idx, notesSortedByName[idx])}; //only one element as readonly
        }
        public Note ExtractNote(int idx)
        {
            if (idx < 0 || idx >= notesSortedByName.Count)
                throw new ArgumentOutOfRangeException();
            Note tmp = notesSortedByName[idx];
            notesSortedByName.RemoveAt(idx);
            notesByPhoneNumber.Remove(tmp.phoneNumber);
            return tmp;
        }
        public Result InsertNote(Note note)
        {
            if (!PhoneNumberFilter.IsMatch(note.phoneNumber) 
                || notesByPhoneNumber.ContainsKey(note.phoneNumber))
            {
                return Result.InvalidPhoneNumber;
            }
            if (!NameFilter.IsMatch(note.surname))
            {
                return Result.InvalidSurname;
            }
            if (!NameFilter.IsMatch(note.name))
            {
                return Result.InvalidName;
            }
            notesSortedByName.Add(note);
            notesSortedByName.Sort();
            notesByPhoneNumber.Add(note.phoneNumber, note);
            return Result.Success;
        }

    }
}
