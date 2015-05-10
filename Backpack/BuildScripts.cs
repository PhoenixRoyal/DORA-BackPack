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
        private string[] Commands;
        private string Name;

        public string Build(string path)
        {
            Process.Start("cmd.exe", "echo hi");
            return "";
        }

        public static BuildScripts LoadScriptFromFile(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<BuildScripts>(json);
        }
    }
}
