using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ggUI;

namespace gg
{
    public class CommandInterpreter : Interpreter
    {
        private string userCommand;
        private const string SPACES_PATTERN = "\\s+";
        private const string ADD_PATTERN = "^(a|add)$";
        private const string LIST_PATTERN = "^(ls|list)$";
        private const string CHANGE_PATTERN = "^(ch|change)$";
        private const string REMOVE_PATTERN = "^(rm|remove)$";
        private const string SYNC_PATTERN = "^(sync)$";
        private const string UNDO_PATTERN = "^(undo)$";
        private const string ACT_PATTERN = "^(act)$";
        private const string PIN_PATTERN = "^(pin)$";
        
        public CommandInterpreter()
        {

        }

        public Command Interpret(string userCommand)
        {
            this.userCommand = GetCommandStringFromUserInput(userCommand);
            if (IsAddCommand())
                return new AddCommand();
            else if (IsListCommand())
                return new ListCommand();
            else if (IsChangeCommand())
                return new ChangeCommand();
            else if (IsRemoveCommand())
                return new RemoveCommand();
            else if (IsSyncCommand())
                return new SyncCommand();
            else if (IsUndoCommand())
                return new UndoCommand();
            else if (IsPinCommand())
                return new PinCommand();
            else if (IsActCommand())
                return new ActCommand();
            return new NullCommand();
        }

        private string GetCommandStringFromUserInput(string userCommand)
        {
            string[] arr = Regex.Split(userCommand, SPACES_PATTERN);
            return arr[0].Trim();
        }

        private bool IsAddCommand()
        {
            return Regex.IsMatch(userCommand, ADD_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsListCommand()
        {
            return Regex.IsMatch(userCommand, LIST_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsChangeCommand()
        {
            return Regex.IsMatch(userCommand, CHANGE_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsRemoveCommand()
        {
            return Regex.IsMatch(userCommand, REMOVE_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsSyncCommand()
        {
            return Regex.IsMatch(userCommand, SYNC_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsUndoCommand()
        {
            return Regex.IsMatch(userCommand, UNDO_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsActCommand()
        {
            return Regex.IsMatch(userCommand, ACT_PATTERN, RegexOptions.IgnoreCase);
        }

        private bool IsPinCommand()
        {
            return Regex.IsMatch(userCommand, PIN_PATTERN, RegexOptions.IgnoreCase);
        }
    }
}
