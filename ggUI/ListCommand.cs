using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace gg
{
    class ListCommand : Command
    {
        private const string MESSAGE_EMPTY_SEARCH = "There are no items containing the search query \"{0}\"\n";
        private const string MESSAGE_EMPTY_TAG_SEARCH = "There are no items containing the tag query \"{0}\"\n";
        private GGResult result = new GGResult(string.Empty, string.Empty, -1, GGResult.RESULT_TYPE.RESULT_LIST);

        /// <summary>
        /// Loops through GGList and displays relevant items to the user in the console.
        /// </summary>
        /// <param name="ggList">The main list</param>
        /// <param name="userCommand">ls, followed by an empty string, a tag query, or a search query</param>
        /// <returns> The list of items (in string form) to display to the user.</returns>
        public override GGResult Execute(string userCommand, GGList ggList)
        {
            this.SetGGList(ggList);
            ggList.SortGGList();

            string pattern = "ls\\s*";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            string parameter = rgx.Replace(userCommand, "", 1); // strip "ls"

            if (IsDisplayAll(parameter))
            {
                result = DisplayAll();
            }
            else if (isDisplayTag(parameter))
            {
                result = DisplayByTag(parameter);
            }
            else // search in descriptions
            {
                result = DisplayBySearch(parameter);
            }
            return result;
        }

        /// <summary>
        /// Gives a string representation of all the items in the list.
        /// </summary>
        /// <returns>A string of all items in ggList</returns>
        private GGResult DisplayAll()
        {
            GGList ggList = this.GetGGList();

            string toDisplay = "";
            for (int i = 0; i < ggList.CountGGList(); i++)
            {
                string toAdd = (i + 1) + ". " + ggList.GetGGItemAt(i).ToString() + "\n";
                toDisplay = toDisplay + toAdd;
            }
            GGResult result = new GGResult(toDisplay, string.Empty, -1, GGResult.RESULT_TYPE.RESULT_LIST);
            return result;
        }

        /// <summary>
        /// Gives a string representation of all items that match the search word in
        /// userCommand.
        /// </summary>
        /// <param name="searchWord">The word to search for</param>
        /// <param name="ggList">The main list</param>
        /// <returns>A string of all items in ggList that contain the word(s) in the search query in their descriptions</returns>
        private GGResult DisplayBySearch(string searchWord)
        {
            GGList ggList = this.GetGGList();

            string toDisplay = "";
            for (int i = 0; i < ggList.CountGGList(); i++)
            {
                GGItem currentItem = ggList.GetGGItemAt(i);
                string checkDescription = currentItem.GetDescription();
                if (FoundWord(checkDescription, searchWord))
                {
                    string toAdd = (i + 1) + ". " + currentItem.ToString() + "\n";
                    toDisplay = toDisplay + toAdd;
                }
            }
            if (toDisplay.Length == 0)
            {
                string emptySearchMsg = string.Format(MESSAGE_EMPTY_SEARCH, searchWord);
                GGResult result = new GGResult(string.Empty, emptySearchMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                return result;
            }
            else
            {
                GGResult result = new GGResult(toDisplay, string.Empty, -1, GGResult.RESULT_TYPE.RESULT_LIST);
                return result;
            }
        }

        private GGResult DisplayByTag(string searchWord)
        {
            GGList ggList = this.GetGGList();

            string toDisplay = "";
            searchWord = Regex.Replace(searchWord, "#", "");
            searchWord = Regex.Replace(searchWord, "\\s+.*", "");
            for (int i = 0; i < ggList.CountGGList(); i++)
            {
                GGItem currentItem = ggList.GetGGItemAt(i);
                string checkTag = currentItem.GetTag();
                if (FoundWord(checkTag, searchWord))
                {
                    string toAdd = (i + 1) + ". " + currentItem.ToString() + "\n";
                    toDisplay = toDisplay + toAdd;
                }
            }
            if (toDisplay.Length == 0)
            {
                string emptySearchMsg = string.Format(MESSAGE_EMPTY_TAG_SEARCH, searchWord);
                GGResult result = new GGResult(string.Empty, emptySearchMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                return result;
            }
            else
            {
                GGResult result = new GGResult(toDisplay, string.Empty, -1, GGResult.RESULT_TYPE.RESULT_LIST);
                return result;
            }
        }

        /// <summary>
        /// Checks if the all items should be displayed, or if the user is searching for specific words or tags
        /// </summary>
        /// <param name="parameter">any parameter following "ls" in the userCommand</param>
        /// <returns>true if parameter is empty, false otherwise</returns>
        private bool IsDisplayAll(string parameter)
        {
            if (parameter.Length == 0)
                return true;
            else
                return false;
        }

        private bool isDisplayTag(string parameter)
        {
            Match match = Regex.Match(parameter, "((#\\s*)[a-zA-Z0-9]+)", RegexOptions.IgnoreCase);

            return (match.Success);
        }

        /// <summary>
        /// Checks a GGItem's description or tag to find if a certain word or phrase can be found in it
        /// </summary>
        /// <param name="toCheck">The description or tag of a GGItem</param>
        /// <param name="toFind">The keyword(s) to be searched for</param>
        /// <returns>true if the keyword(s) can be found, false otherwise</returns>
        private bool FoundWord(string toCheck, string toFind)
        {
            Match match = Regex.Match(toCheck, "\\b" + toFind + "\\b", RegexOptions.IgnoreCase); //TODO" debug case with (/)

            return (match.Success);
        }
    }
}
