using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gg
{
    interface IFileController
    {
         GGList ReadFromFile();
         void WriteToFile(GGList ggList);
         Dictionary<string, string> ReadPreference();
         void WritePreference(Dictionary<string, string> userPreference);
    }
}
