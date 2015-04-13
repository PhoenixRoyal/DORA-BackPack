using System;
using System.Text.RegularExpressions;
using Gtk;
using Gdk;

namespace GtkCodeView
{
	public class GtkCodeView : TextView
	{
		#region Fields
		public LanguageDescription Language { get; set; }
		public Theme Theme { get; set; }
		private Regex _keywords = new Regex("");
		private Regex _comments = new Regex("");
		private Regex _strings = new Regex("");
		private TextTag _keywordTag = new TextTag("keyword");
		private TextTag _commentTag = new TextTag("comment");
		private TextTag _stringTag = new TextTag("string");
		private TextTag _bracketTag = new TextTag("bracket");
		#endregion

		#region Constructors
		public GtkCodeView()
		{
			Buffer.TagTable.Add(_keywordTag);
			Buffer.TagTable.Add(_commentTag);
			Buffer.TagTable.Add(_stringTag);
			Buffer.TagTable.Add(_bracketTag);
			_strings = new Regex("\"(.*?)\"");
			this.Buffer.Changed += HandleBufferChangedEvent;
			this.MoveCursor += (o, args) => HiglightBrackets();
		}

		public GtkCodeView(LanguageDescription lang) :
			this()
		{
			Language = lang;
			CreateRegex();
		}
		#endregion

		#region EventHandler
		[GLib.ConnectBefore]
		void HandleBufferChangedEvent (object o, EventArgs args)
		{
			Buffer.RemoveAllTags(Buffer.StartIter, Buffer.EndIter);
			TextIter start, end;
			foreach (Match m in _keywords.Matches(Buffer.Text)) 
			{
				start = Buffer.GetIterAtOffset(m.Index);
				end = Buffer.GetIterAtOffset(m.Index + m.Length);
				Buffer.ApplyTag("keyword", start, end);
			}
			foreach (Match m in _comments.Matches(Buffer.Text))
			{
				start = Buffer.GetIterAtOffset(m.Index);
				end = Buffer.GetIterAtOffset(m.Index + m.Length);
				Buffer.ApplyTag("comment", start, end);
			}
			foreach (Match m in _strings.Matches(Buffer.Text)) 
			{
				start = Buffer.GetIterAtOffset(m.Index);
				end = Buffer.GetIterAtOffset(m.Index + m.Length);
				Buffer.ApplyTag("string", start, end);
			}
			HiglightBrackets();
		}
		#endregion

		#region Methods
		public void SetTheme(Theme th)
		{
			Theme = th;
			ApplyTheme();
		}

		private void ApplyTheme()
		{
			_keywordTag.Foreground = Theme.Keywords;
			_commentTag.Foreground = Theme.Comments;
			_stringTag.Foreground = Theme.Strings;
			_bracketTag.Background = Theme.Brackets;
			var fg = Color.Zero;
			var bg = Color.Zero;
			Color.Parse(Theme.Foreground, ref fg);
			Color.Parse(Theme.Background, ref bg);
			ModifyBase(StateType.Normal, bg);
			ModifyText(StateType.Normal, fg);
		}

		public void SetLanguage(LanguageDescription lang)
		{
			Language = lang;
			CreateRegex();
		}

		private void CreateRegex()
		{
			var pattern = "(";
			foreach (var s in Language.Keywords) 
			{
				pattern += @"\b" + s + @"\b|";
			}
			pattern = pattern.Remove(pattern.Length - 1);
			pattern += ")";
			_keywords = new Regex(pattern);
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
						Buffer.ApplyTag ("bracket", begin, end);
						break;
					}
					pos++;
				}
			}
		}
		#endregion
	}
}

