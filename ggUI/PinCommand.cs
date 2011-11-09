using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace gg
{
    class PinCommand : Command
    {
        private const String MESSAGE_PIN_SUCCESS = "Pinned {0} to {1} succcessfully\n";
        private const String MESSAGE_PIN_REMOVED = "Removed pin from {0} succcessfully\n";
        private int itemIndex = -1;
        private GGResult result = new GGResult(String.Empty, String.Empty, -1, GGResult.RESULT_TYPE.RESULT_PIN);

        /// <summary>
        /// Pins path to an item in ggList.
        /// </summary>
        /// <param name="ggList">The full list (ggList from GGLogic)</param>
        /// <param name="itemIndex">The index of the list item</param>
        /// <param name="updateTo">The new info of the list item</param>
        /// <returns>The status of the operation</returns>
        public override GGResult Execute(String userCommand, GGList ggList)
        {
            this.SetGGList(ggList);
            Regex rgx = new Regex("\\s+");
            string[] userCommandArray = rgx.Split(userCommand, 3);

            if (userCommandArray.Count() < 3)
            {
                String errorMsg = "Error: Incorrect input"; // "ch", itemIndex, newInfo
                result = new GGResult(String.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                return result;
            }
            else
            {
                string itemIndexString = userCommandArray[1];
                string path = userCommandArray[2].Equals("none") ? "" : userCommandArray[2];
                int.TryParse(itemIndexString, out itemIndex);
                if (IndexOutOfBounds(itemIndex))
                {
                    String errorMsg = String.Format("Error: There is no list item of index {0}\n", itemIndex);
                    result = new GGResult(String.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                    return result;
                }

                if (path.Equals("file"))
                    {
                        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                        if (dlg.ShowDialog() == true)
                        {
                            path = dlg.FileName;
                        }
                        else
                        {
                            String errorMsg = "Error: No File Selected"; // "ch", itemIndex, newInfo
                            result = new GGResult(String.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                            return result;
                        }
                    }
                else if (path.Equals("folder"))
                {
                    System.Windows.Forms.FolderBrowserDialog fb= new System.Windows.Forms.FolderBrowserDialog();
                    fb.ShowNewFolderButton = true;
                    fb.Description = "Select folder to pin to your item.";

                    if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        path = fb.SelectedPath;
                    }
                    else
                    {
                        String errorMsg = "Error: No Folder Selected"; // "ch", itemIndex, newInfo
                        result = new GGResult(String.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                        return result;
                    }
                }
                 
                    GGItem itemToChange = ggList.GetGGItemAt(itemIndex - 1);
                    itemToChange.SetPath(path);
                    String successMsg = path.Equals("") ? String.Format(MESSAGE_PIN_REMOVED,itemIndex)
                        : String.Format(MESSAGE_PIN_SUCCESS, path, itemIndex);

                    result = new GGResult(String.Empty, successMsg, itemIndex - 1, GGResult.RESULT_TYPE.RESULT_PIN, itemToChange.GetTag());
                    return result;
                
            }
        }
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
