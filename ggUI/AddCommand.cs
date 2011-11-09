using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace gg
{
    /// <summary>
    /// <author>Yeo Jie Shun</author>
    /// </summary>
    class AddCommand : Command, IUndoable
    {
        private const string MESSAGE_ERROR = "Error: {0}\n";
        private const string MESSAGE_ADD_SUCCESS = "Added {0} successfully\n";
        private string error = string.Empty;
        private GGItem newGgItem = new GGItem();
        private GGResult result = new GGResult(string.Empty, string.Empty, -1, GGResult.RESULT_TYPE.RESULT_ADD);

        public override GGResult Execute(string userCommand, GGList ggList)
        {
            Debug.Assert(userCommand != null && !userCommand.Equals(string.Empty));
            Debug.Assert(ggList != null);
            this.SetGGList(ggList);
            userCommand = StripAddCommandFromUserCommand(userCommand).Trim();
            try
            {
                ThrowExceptionIfIsEmptyInput(userCommand);
                newGgItem = QuickParser.Parse(userCommand);
                ggList.AddGGItem(newGgItem);
                this.SetIsSuccessful(true);
                string successMsg = string.Format(MESSAGE_ADD_SUCCESS, newGgItem.GetDescription());
                Debug.WriteLine(successMsg, "info");
                result = CreateResultInstance(successMsg, GGResult.RESULT_TYPE.RESULT_ADD, newGgItem.GetTag());
            }
            catch (Exception e)
            {
                error = e.Message;
                string errorMsg = string.Format(MESSAGE_ERROR, error);
                Debug.WriteLine(errorMsg, "error");
                result = CreateResultInstance(errorMsg, GGResult.RESULT_TYPE.RESULT_INVALID);
            }
            return result;
        }

        private void ThrowExceptionIfIsEmptyInput(string userCommand)
        {
            if (userCommand == null || userCommand.Equals(string.Empty))
                throw new FormatException("What do you want to add?");
        }

        private string StripAddCommandFromUserCommand(string userCommand)
        {
            string pattern = "(ad?d?)";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            userCommand = rgx.Replace(userCommand, "", 1);
            return userCommand;
        }

        public void Undo()
        {
            this.GetGGList().RemoveGGItem(newGgItem);
        }
    }
}
