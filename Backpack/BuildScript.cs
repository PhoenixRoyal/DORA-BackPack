using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Backpack
{
    class BuildScript
    {
        private string[] Commands;
        private string Name;

        public string Build(string path)
        {
            return "";
        }

        public static BuildScript LoadScriptFromFile(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<BuildScript>(json);
        }
    }
}
