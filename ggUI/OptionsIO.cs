using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Input;

namespace ggUI
{
    class OptionsObject
    {
        public bool RunOnStartup, Valid;
        public Key ShowHotkey, ExitHotkey, ScrollHotkey;
    }
    class OptionsIO
    {
        private string fname;

        private bool SetBool(string opt)
        {
            return opt.Split('=')[1].StartsWith("True");
        }

        private Key SetKey(string opt)
        {
            return (Key)int.Parse(opt.Split('=')[1]);
        }

        public OptionsObject Read()
        {
            OptionsObject opt = new OptionsObject();
            FileStream fs = new FileStream(fname, FileMode.Open);
            //fs.Lock(0, fs.Length);
            StreamReader fr = new StreamReader(fs);
            
            if (fr.ReadLine() == "[options]")
            {
                opt.RunOnStartup = SetBool(fr.ReadLine());
                opt.ShowHotkey = SetKey(fr.ReadLine());
                opt.ExitHotkey = SetKey(fr.ReadLine());
                opt.ScrollHotkey = SetKey(fr.ReadLine());
            }
            fr.Close();
            return opt;
        }

        public void Write(OptionsObject opt)
        {
            FileStream fs = new FileStream(fname, FileMode.OpenOrCreate);
            //fs.Lock(0, fs.Length);
            StreamWriter fr = new StreamWriter(fs);

            fr.WriteLine("[options]");
            fr.WriteLine("RunOnStartup={0}", opt.RunOnStartup);
            fr.WriteLine("ShowHotkey={0}", (int)opt.ShowHotkey);
            fr.WriteLine("ExitHotkey={0}", (int)opt.ExitHotkey);
            fr.WriteLine("ScrollHotkey={0}", (int)opt.ScrollHotkey);

            fr.Close();
        }

        public OptionsIO(string filename)
        {
            fname = filename;
           
        }
    }
}
