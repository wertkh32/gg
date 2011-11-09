using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gg;

namespace ggUI.UnitTests
{
    /// <summary>
    /// Peng Ziwei
    /// </summary>
    class TestGGSync
    {

        public TestGGSync()
        {
            //GGSyncUnitTest();
            GGSyncCornerCaseTest();
        }

        private void PrintSperateLine()
        {
            Console.WriteLine("\n===================================================");

            Console.WriteLine("\n===================================================");
        }

        private void PreTestPrint(GGList ggList)
        {
            Console.WriteLine("\nBefore sycn gglist: ");
            PrintList(ggList);
        }

        private void PostTestPrint(GGList ggList)
        {
            Console.WriteLine("\nAfter sycn gglist: ");
            PrintList(ggList);
        }

        private void PrintList(GGList myList)
        {
            for (int i = 0; i < myList.GetInnerList().Count; i++)
            {
                Console.WriteLine(myList.GetGGItemAt(i).ToString());
            }
        }

        private void GGSyncUnitTest()
        {
            GGSync mySync = new GGSync("ggtestsync@gmail.com", "cs2103trocks");

            PrintSperateLine();
            Console.WriteLine("\nPlease delete GG to-do calendar\n");
            Console.ReadKey();

            GGList myList = new GGList();
            for (int i = 0; i < 3; i++)
            {
                myList.AddGGItem(new GGItem("desciption_test" + i, DateTime.Now.AddDays(i + 1), "tag_test" + i));
            }

            PrintSperateLine();
            Console.WriteLine("\nTest: add to empty google calender\n");

            PreTestPrint(myList);
            mySync.SyncWithGoogleCalendar(myList);
            PostTestPrint(myList);

            Console.WriteLine("\nPlease check if Google Calendar has 3 events.");

            PrintSperateLine();
            Console.WriteLine("\nTest: local has the latest version\n");
            Console.ReadKey();

            myList.GetGGItemAt(0).SetDescription(myList.GetGGItemAt(0).GetDescription() + " local updated");
            Console.WriteLine("\nupdate local: " + myList.GetGGItemAt(0).ToString());

            GGItem newGGItem = new GGItem("description_test_new", DateTime.Now.AddDays(5).AddHours(3), "tag_test3");
            myList.AddGGItem(newGGItem);
            Console.WriteLine("\nadd local: " + newGGItem.ToString());

            PreTestPrint(myList);
            mySync.SyncWithGoogleCalendar(myList);
            PostTestPrint(myList);

            Console.WriteLine("\nPlease check Google Calendar: event0->description: 'local updated' appended");
            Console.WriteLine("\nPlease check Google Calendar: new event added: tag_test3");

            PrintSperateLine();
            Console.WriteLine("\nTest: server has the latest version\n");
            Console.WriteLine("\nPlease modified one task, add one task and delete one task on calendar");
            Console.ReadKey();

            PreTestPrint(myList);
            mySync.SyncWithGoogleCalendar(myList);
            PostTestPrint(myList);

            Console.WriteLine("\nPlease check: there should be 4 items after sync");

            PrintSperateLine();
            Console.WriteLine("\nTest: both have some latest events\n");
            myList.GetGGItemAt(2).SetTag(myList.GetGGItemAt(2).GetTag() + " local update");
            Console.WriteLine("\nupdate local: " + myList.GetGGItemAt(2).ToString());
            Console.WriteLine("\nPlease update on server");
            Console.ReadKey();

            PreTestPrint(myList);
            mySync.SyncWithGoogleCalendar(myList);
            PostTestPrint(myList);

            Console.WriteLine("\nPlease check Google Calendar: event_2->tag: 'local update' appended");
            Console.WriteLine("\nPlease check: there should be 4 items after sync");

            PrintSperateLine();
            Console.WriteLine("\nTest: delete from server");

            Console.WriteLine("\nRemove at local list: " + myList.GetGGItemAt(0));
            myList.GetDeletedList().Add(myList.GetGGItemAt(0));
            myList.GetInnerList().Remove(myList.GetGGItemAt(0));
            Console.ReadKey();

            PreTestPrint(myList);
            mySync.SyncWithGoogleCalendar(myList);
            PostTestPrint(myList);

            Console.WriteLine("\nPlease check Google Calendar: there should be 3 events");

            Console.ReadKey();

        }

        private void GGSyncCornerCaseTest()
        {
            GGSync syncTest = new GGSync("ggtestsync@gmail.com", "cs2103trocks");
            GGList testList = new GGList();

            PrintSperateLine();

            Console.WriteLine("\nPlease delete GG to-do calendar.");
            Console.ReadKey();

            Console.WriteLine("\nTest: Sync with empty GGList");
            Console.ReadKey();

            PreTestPrint(testList);
            syncTest.SyncWithGoogleCalendar(testList);
            PostTestPrint(testList);

            Console.WriteLine("\nPlease check: there should be no event on both sides");

            PrintSperateLine();

            Console.WriteLine("\nTest: Add to empty calendar");
            Console.ReadKey();

            for (int i = 0; i < 3; i++)
            {
                testList.AddGGItem(new GGItem("corner test description " + i, DateTime.Now.AddDays(-i).AddHours(i), "corner test tag " + i));
            }

            PreTestPrint(testList);
            syncTest.SyncWithGoogleCalendar(testList);
            PostTestPrint(testList);

            Console.WriteLine("\nPlease check: there should be 3 events on Google Calendar");

            PrintSperateLine();

            Console.WriteLine("\nTest: Add to non-empty calendar");
            Console.ReadKey();

            for (int i = 3; i < 5; i++)
            {
                GGItem testItem = new GGItem("corner test description " + i, DateTime.Now.AddDays(-i).AddHours(i), "corner test tag " + i);
                testList.AddGGItem(testItem);
                Console.WriteLine("\nAdd to local GGList: " + testItem.ToString());
            }

            PreTestPrint(testList);
            syncTest.SyncWithGoogleCalendar(testList);
            PostTestPrint(testList);

            Console.WriteLine("\nPlease check: there should be 5 events on Google Calendar");

            PrintSperateLine();

            Console.WriteLine("\nTest: Sync after deleting all events in local GGList");
            Console.ReadKey();

            testList.ClearGGList();

            PreTestPrint(testList);
            syncTest.SyncWithGoogleCalendar(testList);
            PostTestPrint(testList);

            Console.WriteLine("\nPlease check: there should be no event on Google Calendar");
            Console.ReadKey();

            PrintSperateLine();

            Console.WriteLine("\nTest: Sync after deleting all events on Google Calendar");

            for (int i = 0; i < 3; i++)
            {
                testList.AddGGItem(new GGItem("corner test description " + i, DateTime.Now.AddDays(-i).AddHours(i), "corner test tag " + i));
            }

            syncTest.SyncWithGoogleCalendar(testList);

            Console.WriteLine("\nPlease delete all 3 events on Google Calendar");
            Console.ReadKey();

            PreTestPrint(testList);
            syncTest.SyncWithGoogleCalendar(testList);
            PostTestPrint(testList);

            Console.WriteLine("Please check: there should be no events in local GGList");
            Console.ReadKey();

            PrintSperateLine();

            Console.WriteLine("\nTest: Delete the same event both sides");

            testList.AddGGItem(new GGItem("corner test description 0", DateTime.Now, "corner test tag 0"));
            syncTest.SyncWithGoogleCalendar(testList);

            Console.WriteLine("Please delete the only event on Google Calendar");
            Console.ReadKey();

            testList.ClearGGList();

            PreTestPrint(testList);
            syncTest.SyncWithGoogleCalendar(testList);
            PostTestPrint(testList);

            Console.WriteLine("Please check: there should be no event on both sides");

            Console.ReadKey();

        }
    }
}
