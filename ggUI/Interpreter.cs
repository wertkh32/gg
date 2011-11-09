using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ggUI;

namespace gg
{
    interface Interpreter
    {
        Command Interpret(string command);
    }
}
