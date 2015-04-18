using System;
using Newtonsoft.Json;
using System.IO;

namespace GtkCodeView
{
	public class Theme
	{
		public string Keywords { get; set; }
		public string Comments { get; set; }
		public string Strings { get; set; }
		public string Brackets { get; set; }
		public string Background { get; set; }
		public string Foreground { get; set; }
        public string Font { get; set; }
        public string GutterBackground { get; set; }
        public string GutterForeground { get; set; }
        public string GutterFont { get; set; }

		public static Theme LoadFromFile(string file)
		{
			string json = File.ReadAllText(file); 
			return JsonConvert.DeserializeObject<Theme>(json);
		}
	}
}

