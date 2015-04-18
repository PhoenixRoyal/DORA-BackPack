using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace GtkCodeView
{
    class Popup
    {
        public List<String> Items { get; private set; }
        public Window DisplayWindow { get; private set; }
        private string _filterText;
        private bool _shouldDisplay;

        public Popup(TextBuffer buffer)
        {
            Items = new List<string>();
            DisplayWindow = new Window(WindowType.Popup);
        }

        public void ShowSuggestion(string text, TextBuffer buffer, TextView view)
        {
            // Calculate Window Pos
            int win_x, win_y, x, y;
            var cursor = view.GetIterLocation(buffer.GetIterAtMark(buffer.InsertMark));
            view.BufferToWindowCoords(TextWindowType.Widget, cursor.X, cursor.Y, out x, out y);
            var window = view.GetWindow(TextWindowType.Widget);
            window.GetOrigin(out win_x, out win_y);
            DisplayWindow.Move(x + win_x, y + win_y + 20);
            DisplayWindow.BorderWidth = 0;

            var eb = new EventBox();
            eb.BorderWidth = 1;
            DisplayWindow.Add(eb);

            var tree = new TreeView();
            eb.Add(tree);
            var suggestions = new TreeViewColumn();
            tree.AppendColumn(suggestions);
            Gtk.ListStore keywords = new ListStore(typeof(string));
            foreach(var s in Items)
            {
                keywords.AppendValues(s);
            }
            var render = new CellRendererText();
            suggestions.PackStart(render, true);
            suggestions.AddAttribute(render, "text", 0);            
            var pfd = Pango.FontDescription.FromString("courier");
            tree.ModifyFont(pfd);
            var filter = new TreeModelFilter(keywords, null);
            _filterText = text;
            filter.VisibleFunc = FilterItems;
            tree.Model = filter;

            var cmap = DisplayWindow.Colormap;
            var Color = new Gdk.Color(0, 0, 0);
            if (cmap.AllocColor(ref Color, false, true))
                DisplayWindow.ModifyBg(StateType.Normal, Color);
            cmap = tree.Colormap;
            Color = new Gdk.Color(255, 255, 150);
            render.BackgroundGdk = Color;
            tree.ModifyBg(StateType.Normal, Color);
            if(_shouldDisplay)
                DisplayWindow.ShowAll();
            _shouldDisplay = false;
        }

        private bool FilterItems(Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            string key = model.GetValue(iter, 0).ToString();

            if (_filterText == "")
            {
                _shouldDisplay = true;
                return true;
            }

            if (key.StartsWith(_filterText))
            {
                _shouldDisplay = true;
                return true;
            }
            else
                return false;
        }
    }
}
