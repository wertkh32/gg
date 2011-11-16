using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace gg
{
    public abstract class Command
    {
        private bool isBadCommand = false;
        private bool isSuccessful = false;

        private GGList ggList;

        public Command()
        {
            ggList = new GGList();
            //initialize to empty list to prevent null exceptions
        }

        public abstract GGResult Execute(string userCommand, GGList list);

        public bool GetIsBadCommand()
        {
            return this.isBadCommand;
        }

        public void SetIsBadCommand(bool isBadCommand)
        {
            this.isBadCommand = isBadCommand;
        }

        public bool GetIsSuccessful()
        {
            return this.isSuccessful;
        }

        public void SetIsSuccessful(bool isSuccessful)
        {
            this.isSuccessful = isSuccessful;
        }

        public GGList GetGGList()
        {
            return this.ggList;
        }

        public void SetGGList(GGList ggList)
        {
            this.ggList = ggList;
        }

        public GGResult CreateResultInstance(string message, GGResult.RESULT_TYPE resultType, string tag = "")
        {
            Debug.Assert(message != null && !message.Equals(string.Empty));
            return new GGResult(string.Empty, message, -1, resultType, tag);
        }
        public GGResult CreateResultInstance(string message, GGResult.RESULT_TYPE resultType, int itemindex,string tag = "")
        {
            Debug.Assert(message != null && !message.Equals(string.Empty));
            return new GGResult(string.Empty, message, itemindex, resultType, tag);
        } 
    }

    public interface IUndoable
    {
        void Undo();
    }

    /// <summary>
    /// NullCommand. An empty and invalid command. 
    /// </summary>
    class NullCommand : Command
    {
        public NullCommand()
        {
            this.SetIsBadCommand(true);
        }

        public override GGResult Execute(string userCommand, GGList list)
        {
            string errorMsg = "Error: Invalid Input\n";
            GGResult result = new GGResult(string.Empty, errorMsg, -1,GGResult.RESULT_TYPE.RESULT_INVALID);
            return result;
        }
    }

   
}
