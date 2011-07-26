using System;
using System.Windows.Threading;
using OpenTK;
using Plugin;
using WpfOpenTK.OpenGL.Engine;
using System.Threading;
using WpfOpenTK.Managers;

namespace WpfOpenTK
{
	public class AnimationManager
	{
        private IRenderEngine engine = null;
		private GLControl glControl = null;
        private delegate void renderDelegate();
        private AnimationList animation_list = null;

		private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public AnimationList GetAnimationList { get { return animation_list; } }

        public AnimationManager()
        {
            animation_list = new AnimationList();            
        }

		public void setup( GLControl glControl, IRenderEngine engine )
		{
			this.engine = engine;
			this.glControl = glControl;

			RayCaster raycaster = engine as RayCaster;
			if ( raycaster != null )
			{
				dispatcherTimer.Tick += new EventHandler( this.animate );
				dispatcherTimer.Interval = new TimeSpan( 500000 );
				dispatcherTimer.Start();
			}
		}

        public void animate( object sender, EventArgs e )
        {
            var render_engine = engine as RayCaster;
            



            ThreadStart ts = delegate
            {
                render_engine.RotateXYZ();
                render_engine.xAxis = 10;
                render_engine.AngleX = 180;
                render_engine.AngleY = 80; 

                render_engine.Render();
            };

            lock(this) {
                Thread newThread = new Thread( ts );
                newThread.Priority = ThreadPriority.BelowNormal;
                newThread.Start();
            }
            
        }

		public void rotateXYZ( object sender, EventArgs e )
		{
			RayCaster rayEngine = engine as RayCaster;
			if ( rayEngine != null )
			{
				rayEngine.RotateXYZ();
				glControl.Refresh();
			}
		}
	}
}