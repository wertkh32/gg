using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using gg;
using System.Globalization;

namespace ggUI.UnitTests
{
    [TestFixture]
    /// <summary>
    /// <author>Yeo Jie Shun</author>
    /// </summary>
    class TestQuickParser
    {
            //userInput,expected desc,expected tag,expected time, describe test  
        static CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");
        static object[] tests = 
        { 
               new object[] {"smell feet", "smell feet", "misc", DateTime.MinValue.ToString(), "simple"},
                new object[] {"smell feet #smell", "smell feet", "smell", DateTime.MinValue.ToString(), "tag"},
                new object[] {"smell feet #", "smell feet #", "misc", DateTime.MinValue.ToString(), "empty tag"},
                new object[] {"smell feet tmr", "smell feet", "misc", DateTime.Parse(DateTime.Now.Date.AddDays(1).AddHours(23.98333333).ToString()).ToString(), "tmr test"},
                //new object[] {"eat 2100 burgers by tmr", "eat 2100 burgers by tmr", "misc", DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString()).ToString(), "tmr test"},
                new object[] {"smell feet tmr #smell", "smell feet", "smell", DateTime.Parse(DateTime.Now.Date.AddDays(1).AddHours(23.9833333).ToString()).ToString(), "tmr test with tag"},
                new object[] {"smell feet tmr 5pm", "smell feet", "misc", DateTime.Parse(DateTime.Now.Date.AddDays(1).ToShortDateString()).AddHours(17).ToString(), "tmr test with time"},
                // new object[] {"smell feet by tmr 2359", "smell feet", "misc", DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString()).AddHours(23).AddMinutes(59).ToString(), "tmr test with time"},
                new object[] {"smell feet by tmr 5pm #smell", "smell feet", "smell", DateTime.Parse(DateTime.Now.Date.AddDays(1).ToString()).AddHours(17).ToString(), "tmr test with time and tag"},
                new object[] {"smell feet 17/09/2011", "smell feet", "misc", DateTime.Parse("09/17/2011", usCulture).Date.AddHours(23.98333333).ToString(), "specific date"},
                new object[] {"smell feet by 17/09", "smell feet", "misc", DateTime.Parse("09/17/2011", usCulture).Date.AddHours(23.98333333).ToString(), "short date"},
                new object[] {"smell feet 17-9", "smell feet", "misc", DateTime.Parse("09/17/2011", usCulture).Date.AddHours(23.98333333).ToString(), "short date variation"},
                new object[] {"smell feet 17-9 5pm", "smell feet", "misc", DateTime.Parse("09/17/2011", usCulture).AddHours(17).ToString(), "short date and time"},
                new object[] {"smell feet  this sun 5pm", "smell feet", "misc", DateTime.Parse("11/06/2011", usCulture).AddHours(17).ToString(), "next week and time"},
                new object[] {"smell this smelly feet", "smell this smelly feet", "misc", DateTime.MinValue.ToString(), "to confuse"},
                new object[] {"read book #book  11 feb", "read book", "book", DateTime.Parse("02/11/2011", usCulture).Date.AddHours(23.98333333).ToString(), "natural date"},
                new object[] {"read book #book by oct", "read book", "book", DateTime.Parse("10/1/2011", usCulture).Date.AddHours(23.98333333).ToString(), "natural month only"},
                new object[] {"read book #book sun 5pm", "read book", "book", DateTime.Parse("11/06/2011", usCulture).AddHours(17).ToString(), "natural weekday without by"},
                new object[] {"smell feet at 5pm tmr", "smell feet", "misc", DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString()).AddHours(17).ToString(), "with at"},
                new object[] {"\"do not 11 nov parse\"", "do not 11 nov parse", "misc", DateTime.MinValue.ToString(), "with quotes"},  
        };

        [Test, TestCaseSource("tests")]
        public static void QuickParserTest(string command, string description, string tag, string dateTime, string message)
        {
                GGItem gg = QuickParser.Parse(command);
                Assert.AreEqual(description, gg.GetDescription(),message + " description");
                Assert.AreEqual(tag, gg.GetTag(), message + " tag");
                Assert.AreEqual(dateTime, gg.GetEndDate().ToString(), message + " datetime");
       
        }
    }
}
