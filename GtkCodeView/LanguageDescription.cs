using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GtkCodeView
{
	public class LanguageDescription
	{
		public String[] Keywords;
		public String[] Comments;
		public String Name;

		public LanguageDescription() {}

		public LanguageDescription(string file)
		{
			var keywordList = new List<string> ();
			var commentList = new List<string> ();
			var doc = new XmlDocument ();
			doc.Load(file);

			Name = doc.SelectSingleNode ("Language").Attributes ["Name"].Value;
			foreach (XmlNode n in doc.SelectSingleNode("Language/Keywords")) 
			{
				keywordList.Add (n.InnerXml);
			}

			foreach (XmlNode n in doc.SelectSingleNode("Language/Comments")) 
			{
				commentList.Add (n.InnerXml);
			}

			Comments = commentList.ToArray ();
			Keywords = keywordList.ToArray ();
		}

		public static LanguageDescription Plain
		{
			get 
			{
				LanguageDescription lang = new LanguageDescription ();
				lang.Keywords = new string[0];
				lang.Comments = new string[0];
				lang.Name = "Plain";
				return lang;
			}
		}
	}
}

