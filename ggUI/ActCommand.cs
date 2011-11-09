using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace gg
{
    class ActCommand : Command
    {
        private const String MESSAGE_ACT_SUCCESS = "Activated {0} succcessfully\n";
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
            string[] userCommandArray = rgx.Split(userCommand, 2);

            if (userCommandArray.Count() != 2)
            {
                String errorMsg = "Error: Incorrect input"; 
                result = new GGResult(String.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                return result;
            }
            else
            {
                string itemIndexString = userCommandArray[1];

                if (!int.TryParse(itemIndexString, out itemIndex))
                {
                    String errorMsg = "Error: Incorrect input"; 
                    result = new GGResult(String.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                    return result;
                }
                else if (IndexOutOfBounds(itemIndex))
                {
                    String errorMsg = String.Format("Error: There is no list item of index {0}\n", itemIndex);
                    result = new GGResult(String.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                    return result;
                }
                else
                {

                    GGItem itemToChange = ggList.GetGGItemAt(itemIndex - 1);
                    string path = itemToChange.GetPath();

                    if (path.Equals(""))
                    {
                        String errorMsg = String.Format("Error: There is no path pinned to index {0}\n", itemIndex);
                        result = new GGResult(String.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                        return result;
                    }

                    try
                    {
                        runpath(path);
                    }
                    catch (Exception e)
                    {
                        String errorMsg = String.Format("Error: Invalid path pinned to index {0}\n", itemIndex);
                        result = new GGResult(String.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
                        return result;
                    }

                    String successMsg = String.Format(MESSAGE_ACT_SUCCESS, itemIndex);

                    result = new GGResult(String.Empty, successMsg, itemIndex - 1, GGResult.RESULT_TYPE.RESULT_ACT, itemToChange.GetTag());
                    return result;
                }
            }
        }
        private void runpath(string path)
        {
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.UseShellExecute = true;
            p.Start();
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
