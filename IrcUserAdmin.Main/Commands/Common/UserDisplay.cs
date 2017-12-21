using System.Collections.Generic;
using System.Linq;

namespace IrcUserAdmin.Commands.Common
{
    public class UserDisplay
    {
        public IEnumerable<string> SortAlphabetically(IEnumerable<string> users)
        {
            var sortedByLetter = users.Select(s => new { Letter = s.ToUpper().ToCharArray().First(), User = s });
            var messages = new List<string>();
            foreach (var letter in Alphabet())
            {
                char letter1 = letter;
                var usersWithCurrentLetter = sortedByLetter.Where(u => u.Letter == letter1).Select(u => u.User).ToList();
                if (usersWithCurrentLetter.Any())
                {
                    messages.AddRange(DisplayUsersWithLetterPrefix(letter, usersWithCurrentLetter));
                }
            }
            return messages;
        }

        private static IEnumerable<string> DisplayUsersWithLetterPrefix(char letter, IList<string> usersWithCurrentLetter)
        {
            var result = new List<string>();
            const int numresults = 8;
            var userlist = new List<string>();
            for (int index = 0; index < usersWithCurrentLetter.Count; index++)
            {
                string name = usersWithCurrentLetter[index];
                userlist.Add(name);
                int modulo = index % numresults;
                if (index != 0 && modulo == 0)
                {
                    string userstring = string.Join(", ", userlist);
                    string prefixString = string.Format("{0}) {1}", letter, userstring);
                    result.Add(prefixString);
                    userlist = new List<string>();
                }
            }
            if (userlist.Count > 0)
            {
                string userstring = string.Join(", ", userlist);
                string prefixString = string.Format("{0}) {1}", letter, userstring);
                result.Add(prefixString);
            }
            return result;
        }

        private static IEnumerable<char> Alphabet()
        {
            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                yield return letter;
            }
        } 
    }
}