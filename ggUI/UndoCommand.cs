using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gg
{
    class UndoCommand : Command
    {
        string successMsg = "Successfully Undone\n";
        string errorMsg = "Nothing to undo\n";

        public override GGResult Execute(string userCommand, GGList list)
        {
            Invoker invoker = Invoker.GetInstance();
            string msg = string.Empty;
            if (invoker.PopAndUndo())
            {
                msg = successMsg;
            }
            else
            {
                msg = errorMsg;
            }
            GGResult result = new GGResult(string.Empty, msg, -1, GGResult.RESULT_TYPE.RESULT_UNDO);
            return result;
        }
    }
}
