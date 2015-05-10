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

        public void Build(string path)
        {
            foreach(var i in Commands)
            {
                var s = i.Replace("$file", path);
                var command = s.Split(null)[0];
                var p = new Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Arguments = s.Remove(0,command.Length);
                p.StartInfo.FileName = command;
                p.Start();
            }
        }

        public static BuildScripts LoadScriptFromFile(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<BuildScripts>(json);
        }
    }
}
