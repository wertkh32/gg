using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Globalization;

namespace gg.UnitTests
{
    /// <summary>
    /// 
    /// <author>Yeo Jie Shun</author>
    /// </summary>
    class RemoveCommandTests
    {
        [TestFixture]
        class TestChangeCommand
        {
            static string[,] originalData = new string[,] { 
            {"buy shoes", "09/19/2011", "misc"},
            {"do dev guide", "10/31/2011", "misc"}
        };

            static object[] testCases = 
        {
            //command, itemId to remove, expected result message, message
            new object[] {"rm 2", 2, "Removed \"do dev guide\" successfully\n",  "simple rm success"},
            new object[] {"rm -1", -1, "Error: There is no list item of index -1\n", "negative number"},
            new object[] {"rm 0", 0, "Error: There is no list item of index 0\n", "zero index"},
            new object[] {"rm 3", 3, "Error: There is no list item of index 3\n", "out of bound"},
            new object[] {"rm", -99, "Error: Please specify an item id to remove\n", "item id not specified"},
        };
            static CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");
            [Test, TestCaseSource("testCases")]
            public static void GenerateAndRunTestCases(string command, int itemIdToChange, string expectedResultMessage, string message)
            {
                GGList originalList = new GGList();
                AddOriginalDataToList(originalList);
                GGList expectedList = new GGList();
                CreateExpectedList(expectedList, itemIdToChange);
                RemoveCommand rmCommand = new RemoveCommand();
                GGResult actualResult = rmCommand.Execute(command, originalList);
                Assert.AreEqual(expectedList, originalList, message);
                Assert.AreEqual(expectedResultMessage, actualResult.GetNotice(), "result message");
            }

            /// <summary>
            /// Creates the expected list for the test cases. If index is out of bounds, don't
            /// bother to modify the list
            /// </summary>
            /// <param name="expectedList"></param>
            /// <param name="itemIdToChange"></param>
            /// <returns></returns>
            private static GGList CreateExpectedList(GGList expectedList, int itemIdToChange)
            {
                AddOriginalDataToList(expectedList);
                if (itemIdToChange > 0 && itemIdToChange <= expectedList.CountGGList())
                    expectedList.RemoveGGItemAt(itemIdToChange - 1);
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
}
