using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Backpack
{
    class BuildScripts
    {
        public string[] Commands;
        public string Name;
        public bool Vanish;

        public string Build(string path)
        {

            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = @"C:\windows\system32\cmd.exe";
            proc.Arguments = "/c ";
            for (int i = 0; i < Commands.Length; i++)
            {
                proc.Arguments += Commands[0];
                if(!(i == Commands.Length - 1)) proc.Arguments += " & ";
            }
            if (!Vanish) proc.Arguments += " & pause";
            Process.Start(proc);
            return proc.Arguments;
        }

        public static BuildScripts LoadScriptFromFile(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<BuildScripts>(json);
        }
    }
}
