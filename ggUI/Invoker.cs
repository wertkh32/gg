using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace gg
{
    /// <summary>
    /// The invoker does these things:
    /// 1. It checks the input. 
    /// 2. It creates a command object and pushes the command to the stack.
    /// 3. It tells the command to execute it's behavior.
    /// 4. It can then pop the command and undo it's behavior.
    /// 
    /// In this context, the order list is a command list. 
    /// </summary>
    class Invoker
    {
        private static string errorMsg = "Error: Invalid Input\n";

        private Stack<IUndoable> undoableCommands;
        private GGList ggList;
        GGFileController fileCtrl;
        static Invoker invoker = null;

        /// <summary>
        /// The state of the program is initialized here.
        /// </summary>
        private Invoker()
        {
            undoableCommands = new Stack<IUndoable>();
            ggList = new GGList();
            fileCtrl = new GGFileController();
            ggList = fileCtrl.ReadFromFile();
        }

        public static Invoker GetInstance()
        {
            if (invoker == null)
                invoker = new Invoker();
            return invoker;
        }

        public GGResult InvokeCommand(string userCommand)
        {
            Command cmd = DetermineCommandType(userCommand);
            if (cmd.GetIsBadCommand())
            {
                Debug.WriteLine("Invoker: Invalid Input", "error");
                return ReturnErrorMessageInResult();
            }
            else
            {
                GGResult result = ExecuteAndPushToStack(userCommand, cmd);
                Debug.WriteLineIf(!result.GetNotice().Equals(string.Empty), "Invoker: " + result.GetNotice(), "info");
                return result;
            }
        }

        public bool PopAndUndo()
        {
            if (undoableCommands.Count == 0)
                return false;
            undoableCommands.Pop().Undo();
            return true;
        }

        private GGResult ReturnErrorMessageInResult()
        {
            GGResult result = new GGResult(string.Empty, errorMsg, -1, GGResult.RESULT_TYPE.RESULT_INVALID);
            return result;
        }

        private GGResult ExecuteAndPushToStack(string userCommand, Command cmd)
        {
            GGResult result = cmd.Execute(userCommand, ggList);
            if (cmd.GetIsSuccessful() && IsUndoableCommand(cmd))
            {
                undoableCommands.Push((IUndoable)cmd);
            }
            return result;
        }

        private bool IsUndoableCommand(Command cmd)
        {
            IUndoable undoObject = cmd as IUndoable;
            if (undoObject != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Command DetermineCommandType(string userCommand)
        {
            CommandInterpreter cmdInterpreter = new CommandInterpreter();
            return cmdInterpreter.Interpret(userCommand);
        }

        public void CleanUp()
        {
            fileCtrl.WriteToFile(ggList);
        }

        public int GetTagCount(string tag)
        {
            if (tag == null || tag.Equals(string.Empty))
                return -1;
            return ggList.GetTagCount(tag);
        }

        public List<string> GetTagList()
        {
            return ggList.GetTagList();
        }

        public List<int> GetTagCountList()
        {
            return ggList.GetTagCountList();
        }

        public GGList GetGGList()
        {
            return ggList;
        }
    }
}
