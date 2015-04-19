using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace GtkCodeView
{
    class Popup : IDisposable
    {
        public List<String> Items { get; private set; }
        public Window DisplayWindow { get; private set; }
        private string _filterText;
        private TreeView tree;
        private TextBuffer _buffer;
        private bool _shouldDisplay;

        public Popup(TextBuffer buffer)
        {
            Items = new List<string>();
            DisplayWindow = new Window(WindowType.Toplevel);
            DisplayWindow.Decorated = false;
            DisplayWindow.TypeHint = Gdk.WindowTypeHint.PopupMenu;
            _buffer = buffer;
        }

        [GLib.ConnectBefore]
        void DisplayWindow_KeyPressEvent(object o, KeyPressEventArgs args)
        {
            var key = args.Event.Key;
            if(key != Gdk.Key.Down &&
               key != Gdk.Key.Up &&
               key != Gdk.Key.Left &&
               key != Gdk.Key.Right)
            {
                if (args.Event.KeyValue >= 33 && args.Event.KeyValue <= 126)
                {
                    _buffer.InsertAtCursor(key.ToString());
                }
                if (args.Event.Key == Gdk.Key.space || args.Event.Key == Gdk.Key.Tab)
                {
                    var model = tree.Model;
                    TreeIter iter;
                    var text = "";
                    if (tree.Selection.GetSelected(out model, out iter))
                    {
                        text = (string)model.GetValue(iter, 0);
                        _buffer.InsertAtCursor(text.Remove(0, _filterText.Length));
                    }
                    _buffer.InsertAtCursor(" ");
                }
                if (args.Event.Key == Gdk.Key.BackSpace)
                {
                    var start = _buffer.GetIterAtMark(_buffer.InsertMark);
                    var end = start;
                    start.BackwardChar();
                    _buffer.Delete(ref start, ref end);
                }
                DisplayWindow.Destroy();
            }
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

            tree = new TreeView();
            tree.KeyPressEvent += DisplayWindow_KeyPressEvent;
            tree.Shown += tree_Shown;
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
            if (_shouldDisplay)
            {
                DisplayWindow.ShowAll();
            }
            _shouldDisplay = false;
        }

        void tree_Shown(object sender, EventArgs e)
        {
            while (Gdk.Keyboard.Grab(DisplayWindow.GdkWindow, false, 0) != Gdk.GrabStatus.Success)
                System.Threading.Thread.Sleep(100);
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

        public void Dispose()
        {
            tree.Dispose();
        }
    }
}
