using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gtk;
using Gdk;
using Pango;

namespace GtkCodeView
{
	public class GtkCodeView : TextView
	{
		#region Fields
		public LanguageDescription Language { get; set; }
		public Theme Theme { get; set; }
        private Regex _keywords;
        private Regex _comments;
        private Regex _strings;
		private TextTag _keywordTag = new TextTag("keyword");
		private TextTag _commentTag = new TextTag("comment");
		private TextTag _stringTag = new TextTag("string");
		private TextTag _bracketTag = new TextTag("bracket");
        public Boolean ShowSuggestions { get; set; }
        private Gdk.Key _lastKey;
        private Popup Popup;
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
            SetBorderWindowSize(TextWindowType.Left, 25);
            this.ExposeEvent += GtkCodeView_ExposeEvent;
            this.KeyPressEvent += GtkCodeView_KeyPressEvent;
            Popup = new Popup(this.Buffer);
            ShowSuggestions = true;
		}

        [GLib.ConnectBefore]
        void GtkCodeView_KeyPressEvent(object o, KeyPressEventArgs args)
        {
            _lastKey = args.Event.Key;
        }

		public GtkCodeView(LanguageDescription lang) :
			this()
		{
			Language = lang;
			CreateRegex();
            Popup.Items.AddRange(Language.Keywords);
		}
		#endregion

		#region EventHandler
		[GLib.ConnectBefore]
		void HandleBufferChangedEvent (object o, EventArgs args)
		{
			HighlightText();
		}

        void GtkCodeView_ExposeEvent(object o, ExposeEventArgs args)
        {
            if (!this.IsDrawable) return;
            
            var layout = new Pango.Layout(this.PangoContext);
            var window = this.GetWindow(TextWindowType.Left);
            var markup = "";
            int x, y;
            BufferToWindowCoords(TextWindowType.Left, VisibleRect.Left, VisibleRect.Top, out x, out y);
            y *= -1;
            var iter = GetIterAtLocation(VisibleRect.Left, VisibleRect.Top);
            var endIter = GetIterAtLocation(VisibleRect.Left, VisibleRect.Bottom);
            for (int i = iter.Line; i < endIter.Line+1; i++)
            {
                markup += i + "\n";
            }
            layout.SetMarkup(markup);
            layout.Alignment = Pango.Alignment.Left;
            if (args.Event.Window != window) return;
            if (Theme != null && Theme.GutterForeground != null)
            {
                Pango.Color p = new Pango.Color();
                p.Parse(Theme.GutterForeground);
                layout.Attributes.Insert(new Pango.AttrForeground(p));
            }
            if(Theme != null && Theme.Font != null)
            {
                var f = Pango.FontDescription.FromString(Theme.Font);
                layout.Attributes.Insert(new Pango.AttrFontDesc(f));
            }
            window.ClearArea(window.VisibleRegion.Clipbox, false);
            window.DrawLayout(new Gdk.GC((Gdk.Drawable)GdkWindow), 2, y, layout);
        }

		#endregion
        
		#region Methods
		public void SetTheme(Theme th)
		{
			Theme = th;
			ApplyTheme();
		}

		public void HighlightText()
		{
			TextIter iter = GetIterAtLocation(VisibleRect.Left, VisibleRect.Top);
            TextIter endIter = GetIterAtLocation(VisibleRect.Left, VisibleRect.Bottom);
			var text = Buffer.GetText(iter,endIter, false);
			Buffer.RemoveAllTags(iter, endIter);
			TextIter start, end;
            if (_keywords != null)
            {
                foreach (Match m in _keywords.Matches(text))
                {
                    start = Buffer.GetIterAtOffset(iter.Offset + m.Index);
                    end = Buffer.GetIterAtOffset(iter.Offset + m.Index + m.Length);
                    Buffer.ApplyTag("keyword", start, end);
                }
            }
            if (_strings != null)
            {
                foreach (Match m in _strings.Matches(text))
                {
                    start = Buffer.GetIterAtOffset(iter.Offset + m.Index);
                    end = Buffer.GetIterAtOffset(iter.Offset + m.Index + m.Length);
                    Buffer.ApplyTag("string", start, end);
                }
            }
            if (_comments != null)
            {
                foreach (Match m in _comments.Matches(text))
                {
                    start = Buffer.GetIterAtOffset(iter.Offset + m.Index);
                    end = Buffer.GetIterAtOffset(iter.Offset + m.Index + m.Length);
                    Buffer.ApplyTag("comment", start, end);
                }
            }
            HiglightBrackets();
            CreatePopUp();
		}

		private void ApplyTheme()
		{
            if (Theme == null) return;
            if(Theme.Keywords != "")
			    _keywordTag.Foreground = Theme.Keywords;
            if(Theme.Comments != "")
			    _commentTag.Foreground = Theme.Comments;
            if(Theme.Strings != "")
			    _stringTag.Foreground = Theme.Strings;
            if(Theme.Brackets != "")
			    _bracketTag.Background = Theme.Brackets;
			var fg = Gdk.Color.Zero;
			var bg = Gdk.Color.Zero;
			Gdk.Color.Parse(Theme.Foreground, ref fg);
			Gdk.Color.Parse(Theme.Background, ref bg);
            if(Theme.Foreground != "")
			    ModifyBase(StateType.Normal, bg);
            if(Theme.Background != "")
			ModifyText(StateType.Normal, fg);
            if (Theme.Font != "")
                ModifyFont(FontDescription.FromString(Theme.Font));
		}

		public void SetLanguage(LanguageDescription lang)
		{
			Language = lang;
			CreateRegex();
            Popup.Items.Clear();
            Popup.Items.AddRange(Language.Keywords);
		}

		private void CreateRegex()
		{
            if (Language == null) return;
			var pattern = "(";
            if(Language.Keywords != null)
			    foreach (var s in Language.Keywords) 
			    {
				    pattern += @"\b" + s + @"\b|";
			    }
			pattern = pattern.Remove(pattern.Length - 1);
			pattern += ")";
			_keywords = new Regex(pattern);
			pattern = @"(/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/)";
            if(Language.Comments != null)
			    foreach (var s in Language.Comments) 
			    {
				    pattern += "|("+s+".*)";
			    }
			_comments = new Regex(pattern);
		}

		private void HiglightBrackets()
		{
            TextIter iter = GetIterAtLocation(VisibleRect.Left, VisibleRect.Top);
			Buffer.RemoveTag ("bracket", iter, Buffer.EndIter);
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

        private void CreatePopUp()
        {
            if (ShowSuggestions != true) return;
            Popup.DisplayWindow.Destroy();
            var insertMark = Buffer.InsertMark;
            var iter = Buffer.GetIterAtMark(insertMark);
            var end = iter;
            if(iter.EndsWord())
            {
                iter.BackwardWordStart();
                var text = iter.GetText(end);
                var display = true;
                foreach (var s in Language.Keywords)
                    if (s == text)
                        display = false;
                if(_lastKey != Gdk.Key.BackSpace && display)
                    Popup.ShowSuggestion(text, Buffer, this);
            }
        }
		#endregion
	}
}