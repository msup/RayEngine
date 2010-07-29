using System;
using System.Collections.Generic;
using System.Text;

namespace Plugins
{
    public interface IPlugin
    {
        string Name { get; set; }

        string Description { get; set; }

        bool IsLoaded { get; set; }

        void execute();
    }
}