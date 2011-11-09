using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace gg
{
    class SyncCommand : Command
    {
        private const string MESSAGE_SYNC_SUCCESS = "Successfully synced!\n";
        private GGResult result = new GGResult(String.Empty, String.Empty, -1, GGResult.RESULT_TYPE.RESULT_SYNC);
        

        public override GGResult Execute(string userCommand, GGList list)
        {
            string username = string.Empty;
            string password = string.Empty;
            try
            {
                GetLoginCredentialsFromUserCommand(userCommand, out username, out password);
                GGSync sync = new GGSync(username, password);
                bool IsSuccessfulSync = sync.SyncWithGoogleCalendar(list);
                if (IsSuccessfulSync)
                    result = CreateResultInstance(MESSAGE_SYNC_SUCCESS, GGResult.RESULT_TYPE.RESULT_SYNC);
                else
                {
                    result = CreateResultInstance("Sync Error!", GGResult.RESULT_TYPE.RESULT_INVALID);
                }
            }
            catch (FormatException e)
            {
                result = CreateResultInstance(e.Message, GGResult.RESULT_TYPE.RESULT_INVALID);
            }
            return result;
        }

        private void GetLoginCredentialsFromUserCommand(string userCommand, 
            out string username, out string password)
        {
            if (userCommand == null || userCommand.Equals(string.Empty))
                throw new FormatException("No credentials received");
                   

            userCommand = StripCommandFromUserCommand(userCommand);
            string[] parameters = Regex.Split(userCommand, "\\s+");

            if (parameters.Length != 2)
            {
                throw new FormatException("Invalid credentials received");
            }

            username = parameters[0];
            password = parameters[1];
        }

        private string StripCommandFromUserCommand(string userCommand)
        {
            Debug.Assert(userCommand != null || !userCommand.Equals(string.Empty));
            return Regex.Replace(userCommand, "^sync\\s+", "", RegexOptions.IgnoreCase).Trim();
        }
    }
}
