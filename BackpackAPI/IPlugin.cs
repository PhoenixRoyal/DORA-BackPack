using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackAPI
{
    interface IPlugin
    {
        string Name { get; }

        string Description { get; }

        string Author { get; }

        string Version { get; }

        void Init();

        void Unload();
    }
}
