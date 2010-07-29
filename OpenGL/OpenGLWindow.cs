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

		#endregion private fields

		#region public methods

		public OpenGLWindow()
		{
			InitializeComponent();

			MakeCurrent();

			shaderManager = new ShaderManager();
			shaderManager.createProgram( @"" );
		}

		public void Render()
		{
			lock ( this )
			{
				ThreadStart ts = delegate
				{
					MethodInvoker updateIt = delegate
					{
						renderEngine.Render( Width, Height, this );
						SwapBuffers();
						renderEngine.RotateXYZ();
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

			//
			// OpenGLWindow
			//

			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.Name = "OpenGLWindow";
			this.Size = new System.Drawing.Size( 443, 247 );
			this.Load += new System.EventHandler( this.OpenGLWindow_Load );
			this.Paint += new System.Windows.Forms.PaintEventHandler( this.OpenGLWindow_Paint );
			this.Resize += new EventHandler( OpenGLWindow_Resize );

			this.ResumeLayout( false );

			this.Invalidate();
			this.Update();
		}

		private void OpenGLWindow_Load( object sender, System.EventArgs e )
		{
		}

		private void OpenGLWindow_Resize( object sender, System.EventArgs e )
		{
			Debug.WriteLine( "{0} {1}", this.Width, this.Height );

			if ( !this.Context.IsCurrent )
				this.MakeCurrent();

			Update();
		}

		private void OpenGLWindow_Paint( object sender, PaintEventArgs e )
		{
			Render();
		}

		#endregion private methods
	}
}