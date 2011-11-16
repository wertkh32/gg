using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gg
{
    public class GGResult
    {
        public enum RESULT_TYPE { RESULT_ADD, RESULT_UPDATE, RESULT_REMOVE, RESULT_LIST, RESULT_SYNC, RESULT_UNDO, RESULT_INVALID, RESULT_PIN, RESULT_ACT };
        private string mainResult;
        private string notice;
        private int itemIndex;
        private string tag;
        RESULT_TYPE type;

        public GGResult(string mainResult, string notice, int itemIndex, RESULT_TYPE result_type, string tag="")
        {
            this.mainResult = mainResult;
            this.notice = notice;
            this.itemIndex = itemIndex;
            this.type = result_type;
            this.tag = tag;
        }

        public string GetMainResult()
        {
            return mainResult;
        }

        public string GetNotice()
        {
            return notice;
        }

        public int GetItemIndex()
        {
            return itemIndex;
        }

        public RESULT_TYPE GetResultType()
        {
            return type;
        }
        public string GetTag()
        {
            return tag;
        }
    }
}
