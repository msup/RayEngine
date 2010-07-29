using System;
using System.Collections.Generic;
using System.Text;
using Plugins;

namespace Plugin
{
    public interface IRenderEngine : IPlugin
    {
        #region Operations (1)

        //VolumeData data;
        void Render(int width, int height);

        #endregion Operations
    }
}