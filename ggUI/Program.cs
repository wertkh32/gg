using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ggUI.UnitTests;
using System.Diagnostics;
using System.Globalization;

namespace gg
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");
            Console.WriteLine(DateTime.Parse(DateTime.Now.Date.AddDays(1).AddHours(23.9833333).ToString()));
            string input;
            while (true)
            {
                input = Console.ReadLine();
                DateTime result;
                DateTime.TryParse(input, out result);
                Console.WriteLine(result);
            }
        }
    }
}
