using System;
using System.IO;
using Newtonsoft.Json;

namespace GtkCodeView
{
	public class LanguageDescription
	{
		public String Name { get; set;}
		public String[] Keywords { get; set; }
		public String[] Comments { get; set; }

		public static LanguageDescription LoadFromFile(string file)
		{
			string json = File.ReadAllText(file); 
			return JsonConvert.DeserializeObject<LanguageDescription>(json);
		}
	}
}

