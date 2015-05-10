using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackAPI
{
    public interface IPluginContext
    {
        GtkCodeView.GtkCodeView View { get; set; }
    }
}
