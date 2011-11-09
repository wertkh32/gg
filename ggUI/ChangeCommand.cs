using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace gg
{
    class ChangeCommand : Command, IUndoable
    {
        public const string MESSAGE_CHANGE_SUCCESS = "Changed {0} succcessfully\n";
        private int itemIndex = -1;
        private GGResult result = new GGResult(string.Empty, string.Empty, -1, GGResult.RESULT_TYPE.RESULT_UPDATE);
        private GGItem OldGgItem = new GGItem();

        /// <summary>
        /// Updates the description of the indicated item in ggList.
        /// </summary>
        /// <param name="ggList">The full list (ggList from GGLogic)</param>
        /// <param name="itemIndex">The index of the list item</param>
        /// <param name="updateTo">The new info of the list item</param>
        /// <returns>The status of the operation</returns>
        public override GGResult Execute(string userCommand, GGList ggList)
        {
            this.SetGGList(ggList);
            Regex regex = new Regex("\\s+");
            string[] userCommandArray = regex.Split(userCommand, 3);

            if (userCommandArray.Count() < 3)
            {
                string errorMsg = "Error: Incorrect input"; // "ch", itemIndex, newInfo
                result = CreateResultInstance(errorMsg, GGResult.RESULT_TYPE.RESULT_INVALID);
                return result;
            }
            string itemIndexString = userCommandArray[1];
            string newInfo = userCommandArray[2];
            if (NewInfoIsEmpty(newInfo))
            {
                string errorMsg = "Error: What do you want to change to?";
                result = CreateResultInstance(errorMsg, GGResult.RESULT_TYPE.RESULT_INVALID);
                return result;
            }
            int.TryParse(itemIndexString, out itemIndex);
            if (IndexOutOfBounds(itemIndex))
            {
                string errorMsg = string.Format("Error: There is no list item of index {0}\n", itemIndex);
                result = CreateResultInstance(errorMsg, GGResult.RESULT_TYPE.RESULT_INVALID);
                return result;
            }
            try
            {
                string oldtag = ggList.GetGGItemAt(itemIndex - 1).GetTag();
                GGItem itemToChange = ggList.GetGGItemAt(itemIndex - 1);
                ggList.RemoveGGItemAt(itemIndex - 1);
                GGItem changedItem = DetermineFieldAndChange(newInfo, itemToChange);
                ggList.InsertGGItem(itemIndex - 1, changedItem);
                string successMsg = string.Format(MESSAGE_CHANGE_SUCCESS, itemIndex);

                result = new GGResult(string.Empty, successMsg, itemIndex - 1, GGResult.RESULT_TYPE.RESULT_UPDATE, oldtag);
                return result;

            }
            catch (Exception e)
            {
                string errorMsg = "Error: Invalid Input";
                result = CreateResultInstance(errorMsg, GGResult.RESULT_TYPE.RESULT_INVALID);
                return result;
            }
        }

        private bool NewInfoIsEmpty(string newInfo)
        {
            if (newInfo == null || newInfo.Trim().Equals(string.Empty))
                return true;
            else
                return false;
        }

        private GGItem DetermineFieldAndChange(string newInfoToChangeTo, GGItem itemToChange)
        {
            Match newInfoMatch = null;
            GGItem changedItem = itemToChange;
            DateTime endDate = GGItem.DEFAULT_ENDDATE;
            if (NewInfoIsTag(newInfoToChangeTo, out newInfoMatch))
            {
                string tag = GetTagFromNewInfoMatch(newInfoMatch);
                changedItem.SetTag(tag);
            }
            else if (NewInfoIsDate(newInfoToChangeTo, out endDate))
            {
                changedItem.SetEndDate(endDate);
            }
            else
                changedItem.SetDescription(newInfoToChangeTo);
            return changedItem;
        }

        private bool NewInfoIsDate(string newInfoToChangeTo, out DateTime endDate)
        {
            DateTimeParser parser = new DateTimeParser(newInfoToChangeTo);
            endDate = parser.GetDateTimeResult();
            if (endDate != GGItem.DEFAULT_ENDDATE)
                return true;
            else
                return false;
        }

        private string GetTagFromNewInfoMatch(Match newInfoMatch)
        {
            return newInfoMatch.Groups["tag"].Value;
        }

        private bool NewInfoIsTag(string newInfoToChangeTo, out Match newInfoMatch)
        {
            string tagPattern = @"#(?<tag>\w+)";
            newInfoMatch = Regex.Match(newInfoToChangeTo, tagPattern);
            return newInfoMatch.Success;
        }

        public void Undo()
        {
            if (itemIndex > 0)
                this.GetGGList().SetGGItemAt(itemIndex - 1, OldGgItem);
        }

        /// <summary>
        /// Checks if the index given by the user is out of the range of ggList
        /// </summary>
        /// <param name="itemIndex">The index number given by the user</param>
        /// <param name="ggList">The full list</param>
        /// <returns>false if the index is valid, true if the index is less than zero or
        /// more than the number of items in the list</returns>
        private bool IndexOutOfBounds(int itemIndex)
        {
            int normalizedIndex = (itemIndex - 1);
            if (normalizedIndex < 0 || normalizedIndex >= this.GetGGList().CountGGList())
                return true;
            else
                return false;
        }
    }
}
