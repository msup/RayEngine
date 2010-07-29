using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using Plugin;
using VolumeRenderingEngines;

namespace WpfOpenTK
{
	public partial class OpenGLWindow : GLControl
	{
		#region private fields

		private AnimationManager animationManager = new AnimationManager();
		private IRenderEngine renderEngine = null;
		private ShaderManager shaderManager = null;
		private List<string> shaderList = null;
		private System.Timers.Timer renderProgressTimer = null;

		#endregion private fields

		// TODO:
		//' FIXME
		// this remove to different class
		float renderingStep = 0.01f;

		#region public methods

		public OpenGLWindow()
		{
			InitializeComponent();

			MakeCurrent();

			shaderManager = new ShaderManager();
			shaderManager.createProgram( @"" );

			// this timer allow progressie enhancement of rendered image if no interaction in window detected
			renderProgressTimer = new System.Timers.Timer();
			renderProgressTimer.Elapsed += new System.Timers.ElapsedEventHandler( renderProgressTimer_Elapsed );
			renderProgressTimer.AutoReset = false;
			// Set the Interval to 1000 milliseconds
			renderProgressTimer.Interval = 1000;
			renderProgressTimer.Enabled = false;
		}

		private void renderProgressTimer_Elapsed( object sender, System.Timers.ElapsedEventArgs e )
		{
			if ( renderingStep > 0.0005 )
			{
				renderingStep /= 2;
				renderProgressTimer.Interval = 1500;
				Invalidate();
			}
		}

		public void Render( float renderingStep )
		{
			lock ( this )
			{
				ThreadStart ts = delegate
				{
					MethodInvoker updateIt = delegate
					{
						renderEngine.Render( Width, Height, this, renderingStep );
						SwapBuffers();
						//renderEngine.RotateXYZ();
					};
					BeginInvoke( updateIt );
				};

				Thread newThread = new Thread( ts );
				newThread.Priority = ThreadPriority.BelowNormal;
				newThread.Start();
			}
		}

		public void SetRenderingMethod( IRenderEngine _renderEngine )
		{
			renderEngine = _renderEngine;

			RayCaster Engine = renderEngine as RayCaster;
			if ( Engine != null )
				Engine.ShaderPrograms = shaderManager.ShaderPrograms;

			//animationManager.setup( this, renderEngine );
		}

		public void SetShaderList( List<string> list )
		{
			shaderList = list;
		}

		#endregion public methods

		#region private methods

		private void InitializeComponent()
		{
			this.SuspendLayout();

			this.Size                = new System.Drawing.Size( 443, 247 );
			this.Load               += new System.EventHandler( this.OpenGLWindow_Load );
			this.Paint              += new System.Windows.Forms.PaintEventHandler( this.OpenGLWindow_Paint );
			this.Resize             += new EventHandler( OpenGLWindow_Resize );
			this.SizeChanged         += new EventHandler( OpenGLWindow_MarginChanged );

			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.Name                = "OpenGLWindow";

			this.ResumeLayout( false );

			this.Invalidate();
			this.Update();
		}

		private void OpenGLWindow_Load( object sender, System.EventArgs e )
		{
		}

		private void OpenGLWindow_MarginChanged( object sender, System.EventArgs e )
		{
			renderProgressTimer.Enabled = false;
		}

		private void OpenGLWindow_Resize( object sender, System.EventArgs e )
		{
			Debug.WriteLine( "{0} {1}", this.Width, this.Height );

			if ( !this.Context.IsCurrent )
				this.MakeCurrent();

			renderingStep = 0.01f;
			renderProgressTimer.Enabled = true;
			Update();
		}

		private void OpenGLWindow_Paint( object sender, PaintEventArgs e )
		{
			Render( renderingStep );
			renderProgressTimer.Interval = 1000;
			renderProgressTimer.Enabled = true;
		}

		#endregion private methods
	}
}