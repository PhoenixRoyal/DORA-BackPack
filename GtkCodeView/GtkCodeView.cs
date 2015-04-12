using System;
using Gtk;
using System.Text.RegularExpressions;

namespace GtkCodeView
{
	[System.ComponentModel.ToolboxItem(true)]
	public class GtkCodeView : Gtk.TextView
	{
		private Regex _keywordRegex = new Regex("");
		private Regex _commentRegex = new Regex("");
		private Regex _strings = new Regex("");
		private LanguageDescription _lang;
		private Colorscheme _cs;
		private TextTag keywordTag, commentTag, bracketTag, stringTag;

		public GtkCodeView ()
		{
			keywordTag = new TextTag ("keyword");
			commentTag = new TextTag ("comment");
			stringTag = new TextTag ("strings");
			bracketTag = new TextTag ("bracket");
			Buffer.TagTable.Add (keywordTag);
			Buffer.TagTable.Add (commentTag);
			Buffer.TagTable.Add (stringTag);
			Buffer.TagTable.Add (bracketTag);

			_strings = new Regex("\"(.*?)\"");
			Buffer.Changed += new EventHandler (BufferChanged);
			this.MoveCursor += (o, args) => HiglightBrackets ();
			this.TooltipWindow = new Window ("Test");
		}
			
		private void BufferChanged(object sender, EventArgs e)
		{
			
			Buffer.RemoveAllTags (Buffer.StartIter, Buffer.EndIter);
			foreach(Match m in _keywordRegex.Matches(Buffer.Text))
			{
				var start = Buffer.GetIterAtOffset (m.Index);
				var stop = Buffer.GetIterAtOffset (m.Index + m.Length);
				Buffer.ApplyTag ("keyword", start, stop);
			}

			foreach(Match m in _strings.Matches(Buffer.Text))
			{
				var start = Buffer.GetIterAtOffset (m.Index);
				var stop = Buffer.GetIterAtOffset (m.Index + m.Length);
				Buffer.ApplyTag ("strings", start, stop);
			}

			foreach(Match m in _commentRegex.Matches(Buffer.Text))
			{
				var start = Buffer.GetIterAtOffset (m.Index);
				var stop = Buffer.GetIterAtOffset (m.Index + m.Length);
				Buffer.ApplyTag ("comment", start, stop);
			}

			HiglightBrackets ();
		}

		private void CursorMoved(object sender, MoveCursorArgs e)
		{
			HiglightBrackets ();
		}

		private void HiglightBrackets()
		{
			Buffer.RemoveTag ("bracket", Buffer.StartIter, Buffer.EndIter);
			var openingBrackets = new string[] { "(", "{", "[", "<" };
			var closingBrackets = new string[] { ")", "}", "]", ">" };
			var begin = Buffer.GetIterAtOffset(Buffer.CursorPosition-1);
			var end = Buffer.GetIterAtOffset (Buffer.CursorPosition);
			var text = Buffer.GetText (begin, end, false);
			var closingCount = 0;
			var openingCount = 0;
			for(int i=0;i<4;i++)
				if (text == closingBrackets[i]) 
				{
					var pos = Buffer.CursorPosition-1;
					while (pos >= 0 && pos <= Buffer.Text.Length) { 
						begin = Buffer.GetIterAtOffset (pos);
						end = Buffer.GetIterAtOffset (pos + 1);
						if (Buffer.GetText (begin, end, false) == openingBrackets [i]) {
							openingCount++;
						}
						if (Buffer.GetText (begin, end, false) == closingBrackets [i]) {
							closingCount++;
						}
						if (closingCount - openingCount == 0) {
							Console.WriteLine ("Found matching brackets");
							Buffer.ApplyTag ("bracket", begin, end);
							break;
						}
						pos--;
					}
			}
			for(int i=0;i<4;i++)
				if (text == openingBrackets[i]) 
				{
					var pos = Buffer.CursorPosition-1;
					while (pos >= 0 && pos <= Buffer.Text.Length) { 
						begin = Buffer.GetIterAtOffset (pos);
						end = Buffer.GetIterAtOffset (pos + 1);
						if (Buffer.GetText (begin, end, false) == closingBrackets[i]) 
						{
							openingCount++;
						}
						if (Buffer.GetText (begin, end, false) == openingBrackets[i]) 
						{
							closingCount++;
						}
						if (closingCount - openingCount == 0) 
						{
							Console.WriteLine ("Found matching brackets");
							Buffer.ApplyTag ("bracket", begin, end);
							break;
						}
					pos++;
				}
			}
		}

		private void CreateRegex()
		{
			var pattern = "(";
			foreach (var s in _lang.Keywords) 
			{
				pattern += @"\b" + s + @"\b|";
			}
			pattern = pattern.Remove(pattern.Length-1) + ")";
			_keywordRegex = new Regex (pattern);

			pattern = "(";
			foreach (var s in _lang.Comments) 
			{
				pattern += s + ".*|";
			}
			pattern = pattern.Remove(pattern.Length-1) + ")";
			_commentRegex = new Regex (pattern);
		}

		public void SetLanguage(LanguageDescription lang)
		{
			_lang = lang;
			CreateRegex ();
		}

		public string GetLanguageName()
		{
			return _lang.Name;
		}

		public void SetColorscheme(Colorscheme cs)
		{
			_cs = cs;
			CreateTags ();
		}

		private void CreateTags()
		{
			keywordTag.Foreground = _cs.Keywords;
			commentTag.Foreground = _cs.Comments;
			bracketTag.Background = _cs.Brackets;
			stringTag.Foreground = _cs.Strings;
		}

		private void ChangeColors()
		{
			Gdk.Color fg = new Gdk.Color ();
			Gdk.Color bg = new Gdk.Color ();
			Gdk.Color.Parse (_cs.Background, ref bg);
			Gdk.Color.Parse (_cs.Foreground, ref fg);
			ModifyBg (StateType.Normal, bg);
			ModifyFg (StateType.Normal, fg);
		}
	}
}

