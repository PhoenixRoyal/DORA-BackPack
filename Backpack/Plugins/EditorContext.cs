using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GtkCodeView;
using BackpackAPI;

namespace Backpack.Plugins
{
    public class EditorContext : IPluginContext
    {
        public GtkCodeView.GtkCodeView View { get; set; }
    }
}
