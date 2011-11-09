using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Globalization;

namespace gg.UnitTests
{
    /// <summary>
    /// <author>Yeo Jie Shun</author>
    /// </summary>
    class AddCommandTest
    {
        [TestFixture]
        class TestChangeCommand
        {
            static string[,] originalData = new string[,] { 
            {"buy shoes", "09/19/2011", "misc"},
            {"do dev guide", "10/31/2011", "misc"}
        };
            static CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");
            static object[] testCases = 
        {
            //command, desc, tag, date, expected result message, message
            new object[] {"add respond to blog #cs2101 24 nov", "respond to blog", "cs2101", DateTime.Parse("11/24/2011", usCulture).AddHours(23.9833333), "Added respond to blog successfully\n",  "simple desc add"},
            new object[] {"add", null, null, DateTime.MinValue, "Error: What do you want to add?\n", "empty adding"},
        };

            [Test, TestCaseSource("testCases")]
            public static void GenerateAndRunTestCases(string command, string expectedDesc, string expectedTag, DateTime expectedDate, string expectedResultMessage, string message)
            {
                GGList originalList = new GGList();
                AddOriginalDataToList(originalList);
                GGList expectedList = new GGList();
                CreateExpectedList(expectedList, expectedDesc, expectedTag, expectedDate);
                AddCommand addCommand = new AddCommand();
                GGResult actualResult = addCommand.Execute(command, originalList);
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
            private static GGList CreateExpectedList(GGList expectedList, string expectedDesc, string expectedTag, DateTime expectedDate)
            {
                AddOriginalDataToList(expectedList);
                string desc = string.Empty;
                string tag = string.Empty;
                DateTime date = GGItem.DEFAULT_ENDDATE;
                if (expectedDesc == null)
                    return expectedList;
                desc = expectedDesc;
                if (expectedTag != null)
                    tag = expectedTag;
                if (expectedDate != GGItem.DEFAULT_ENDDATE)
                    date = expectedDate;
                expectedList.AddGGItem(new GGItem(desc, date, tag));
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
