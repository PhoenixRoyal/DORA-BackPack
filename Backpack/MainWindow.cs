using System;
using Gtk;
using GtkCodeView;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build();
		//var l = LanguageDescription.LoadFromFile("/home/florian/C.json");
		//var t = Theme.LoadFromFile("/home/florian/Test.json");
		var view = new GtkCodeView.GtkCodeView();
		//view.SetTheme(t);
		this.Add(view);
		this.ShowAll();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}
}
