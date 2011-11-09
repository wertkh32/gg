using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace gg
{
    /// <summary>
    /// <author>Rose Marie Tan</author>
    /// </summary>
    class RemoveCommand : Command, IUndoable
    {
        private const string MESSAGE_REMOVE_SUCCESS = "Removed \"{0}\" successfully\n";
        private GGResult result = new GGResult(string.Empty, string.Empty, -1, GGResult.RESULT_TYPE.RESULT_REMOVE);
        private GGItem removedGgItem = new GGItem();
        private int itemIndex = -1;

        /// <summary>
        /// Deletes an item from the ggList as specified by the user
        /// </summary>
        /// <param name="userCommand">The command issued by the user</param>
        /// <param name="ggList">The list of task items</param>
        public override GGResult Execute(string userCommand, GGList list)
        {
            Debug.Assert(userCommand != null && !userCommand.Equals(string.Empty));
            this.SetGGList(list);
            if (InvalidInput(userCommand))
            {
                string errorMsg = "Error: Please specify an item id to remove\n";
                Debug.WriteLine(errorMsg);
                return CreateErrorResultInstance(errorMsg);
            }
            int itemIndexToShow = GetItemIndexFromUserCommand(userCommand);
            itemIndex = itemIndexToShow - 1;
            if (IndexOutOfBounds(itemIndex))
            {
                string errorMsg = string.Format("Error: There is no list item of index {0}\n", itemIndexToShow);
                Debug.WriteLine(errorMsg);
                return CreateErrorResultInstance(errorMsg);
            }
            GGList ggList = this.GetGGList();
            removedGgItem = ggList.GetGGItemAt(itemIndex);
            ggList.RemoveGGItemAt(itemIndex);
            this.SetIsSuccessful(true);
            string successMsg = string.Format(MESSAGE_REMOVE_SUCCESS, removedGgItem.GetDescription());
            Debug.WriteLine(successMsg);
            result = new GGResult(string.Empty, successMsg, itemIndex, GGResult.RESULT_TYPE.RESULT_REMOVE, removedGgItem.GetTag()); //useless to return itemindex since item does not exist
            return result;
        }

        private GGResult CreateErrorResultInstance(string errorMsg)
        {
            result = new GGResult(string.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
            return result;
        }

        private bool InvalidInput(string userCommand)
        {
            if (userCommand == null || userCommand.Equals(string.Empty))
                return false;
            string[] userCommandArray = Regex.Split(userCommand, @"\s+");
            if (userCommandArray.Length == 2)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Parses the userCommand to retrieve the item index that the
        /// user wishes to remove
        /// </summary>
        /// <param name="userCommand">The command issued by the user</param>
        /// <returns>The index in the form of an integer. -1 is returned if an error
        /// has occurred
        /// </returns>
        private int GetItemIndexFromUserCommand(string userCommand)
        {
            string[] userCommandArray = Regex.Split(userCommand, "\\s+");
            int result = -1;
            if (userCommandArray.Count() == 1)
                return result;
            string itemIndex = userCommandArray[1];
            int.TryParse(itemIndex, out result);
            return result;
        }

        public void Undo()
        {
            this.GetGGList().InsertGGItem(itemIndex - 1, removedGgItem);
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
            if (itemIndex < 0 || itemIndex >= this.GetGGList().CountGGList())
                return true;
            else
                return false;
        }
    }
}
