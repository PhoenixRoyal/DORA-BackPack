using System;
using System.Xml;
using System.IO;
using System.Text;
using Gdk;
using System.Collections.Generic;

namespace GtkCodeView
{
	public class Colorscheme
	{
		public String Keywords;
		public String Comments;
		public String Strings;
		public String Foreground;
		public String Background;
		public String Brackets;

		public Colorscheme() {}

		public Colorscheme (string file)
		{
			var doc = new XmlDocument ();
			doc.Load(file);

			Keywords = doc.SelectSingleNode ("Colorscheme/Editor/Keywords").InnerXml;
			Comments = doc.SelectSingleNode ("Colorscheme/Editor/Comments").InnerXml;
			Strings = doc.SelectSingleNode("Colorscheme/Editor/Strings").InnerXml;
			Foreground = doc.SelectSingleNode("Colorscheme/Editor/Foreground").InnerXml;
			Background = doc.SelectSingleNode("Colorscheme/Editor/Background").InnerXml;
			Brackets = doc.SelectSingleNode ("Colorscheme/Editor/Brackets").InnerXml;
		}

		public static Colorscheme Default 
		{
			get 
			{
				Colorscheme cs = new Colorscheme ();
				cs.Keywords = "#009999";
				cs.Comments = "#006600";
				cs.Strings = "#FF6633";
				cs.Brackets = "#FF6600";
				cs.Background = "#000000";
				cs.Foreground = "#FFFFFF";
				return cs;
			}
		}
	}
}

