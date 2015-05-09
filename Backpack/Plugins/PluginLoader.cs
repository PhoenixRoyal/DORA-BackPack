using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BackpackAPI;
using System.IO;

namespace Backpack.Plugins
{
    public static class PluginLoader
    {
        private const string PLUGIN_DIR = @"C:\Users\Florian\Documents\GitHub\DORA-BackPack\Backpack\bin\Debug\plugins";
        public static List<IPlugin> Plugins { get; private set; }

        public static void LoadPlugins(EditorContext ec)
        {
            string[] dlls = null;
            if (Directory.Exists(PLUGIN_DIR))
            {
                dlls = Directory.GetFiles(PLUGIN_DIR, "*.dll");
            }
            else return;
            var assemblies = new List<Assembly>(dlls.Length);
            foreach(var a in dlls)
            {
                var assem = Assembly.LoadFrom(a);
                assemblies.Add(assem);
            }
            Type pluginType = typeof(IPlugin);
            ICollection<Type> pluginTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly != null)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }
                        else
                        {
                            if (type.GetInterface(pluginType.Name) != null)
                            {
                                pluginTypes.Add(type);
                            }
                        }
                    }
                }
            }
            Plugins = new List<IPlugin>(pluginTypes.Count);
            foreach (Type type in pluginTypes)
            {
                IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                plugin.Init(ec);
                Console.WriteLine(plugin.Description);
                Plugins.Add(plugin);
            }
        }
    }
}
