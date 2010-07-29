﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using VolumeRenderingEngines;
using OpenTK;
using Plugin;

namespace WpfOpenTK
{
    public class AnimationManager
    {
        private IRenderEngine engine = null;
        private GLControl glControl = null;

        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public void setup(GLControl glControl, IRenderEngine engine)
        {
            this.engine = engine;
            this.glControl = glControl;

            RayCaster raycaster = engine as RayCaster;
            if (raycaster != null)
            {
                dispatcherTimer.Tick += new EventHandler(this.rotateXYZ);
                dispatcherTimer.Interval = new TimeSpan(500000);
                dispatcherTimer.Start();
            }
        }

        public void rotateXYZ(object sender, EventArgs e)
        {
            RayCaster rayEngine = engine as RayCaster;
            if (rayEngine != null)
            {
                rayEngine.RotateXYZ();
                glControl.Refresh();
            }
        }
    }
}