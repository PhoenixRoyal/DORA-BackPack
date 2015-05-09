using System;
using Gtk;
using GtkCodeView;
using Backpack.Plugins;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build();
		var l = LanguageDescription.LoadFromFile("CSharp.json");
		var t = Theme.LoadFromFile("Theme.json");
		var view = new GtkCodeView.GtkCodeView(l);
		view.SetTheme(t);
		this.Add(view);
		SetSizeRequest(300, 400);
        var ec = new EditorContext();
        ec.View = view;
        PluginLoader.LoadPlugins(ec);
		this.ShowAll();
        Backpack.BuildScripts BScript1 = Backpack.BuildScripts.LoadScriptFromFile("BuildScriptC.json");
        BScript1.Build("hi");
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}
}
