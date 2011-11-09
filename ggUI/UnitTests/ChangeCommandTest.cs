using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gg;
using NUnit.Framework;
using System.Globalization;

namespace ggUI.UnitTests
{
    [TestFixture]
    /// <summary>
    /// <author>Yeo Jie Shun</author>
    /// </summary>
    class TestChangeCommand
    {
        static string[,] originalData = new string[,] { 
            {"buy shoes", "09/19/2011", "misc"},
            {"do dev guide", "10/31/2011", "misc"}
        };
        static CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");
        static object[] testCases = 
        {
            //command, type, itemId to change, expected change, expected result message, message
            //expected change is either desc, tag or date
            new object[] {"ch 1 make dinner", "desc", 1, "make dinner", "Changed 1 succcessfully\n",  "simple desc change"},
            new object[] {"ch 1 #smell", "tag", 1, "smell", "Changed 1 succcessfully\n", "simple tag change"},
            new object[] {"ch 1 23 oct", "date", 1, "10/23/2011", "Changed 1 succcessfully\n", "simple date change"},
        };

        [Test, TestCaseSource("testCases")]
        public static void GenerateAndRunTestCases(string command, string type, int itemIdToChange, string expectedChange, string expectedResultMessage, string message)
        {
            GGList originalList = new GGList();
            AddOriginalDataToList(originalList);
            GGList expectedList = new GGList();
            CreateExpectedList(expectedList, type, itemIdToChange, expectedChange);
            ChangeCommand cmd = new ChangeCommand();
            GGResult actualResult = cmd.Execute(command, originalList);
            Assert.AreEqual(expectedList, originalList, message);
            Assert.AreEqual(expectedResultMessage, actualResult.GetNotice(), "result message");
        }

        private static GGList CreateExpectedList( GGList expectedList, string type, int itemIdToChange, string expectedChange)
        {
            AddOriginalDataToList(expectedList);
            if (type.Equals("desc"))
            {
                expectedList.GetGGItemAt(itemIdToChange - 1).SetDescription(expectedChange);
            }
            else if (type.Equals("tag"))
            {
                expectedList.GetGGItemAt(itemIdToChange - 1).SetTag(expectedChange);
            }
            else if (type.Equals("date"))
            {
                expectedList.GetGGItemAt(itemIdToChange - 1).SetEndDate(DateTime.Parse(expectedChange, usCulture).Date.AddHours(23.9833333));
            }
            return expectedList;
        }

        private static void AddOriginalDataToList(GGList originalList)
        {
            for (int i = 0; i < originalData.Length / 3; i++)
            {
                GGItem item = new GGItem(originalData[i, 0], DateTime.Parse(originalData[i, 1], usCulture), originalData[i, 2]);
                originalList.AddGGItem(item);
            }
        }
    }
}
