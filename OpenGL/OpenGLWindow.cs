using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using OpenTK;
using Plugin;
using VolumeRenderingEngines;

namespace WpfOpenTK
{
	public partial class OpenGLWindow : GLControl
	{
		private Color _clearColor;
		private AnimationManager animationManager = new AnimationManager();
		private IRenderEngine renderEngine = null;
		private ShaderManager shaderManager = null;
		private List<string> shaderList = null;

		private bool init = false;
		private bool StartupInit = false;
		private bool renderNow = true;
		private bool loaded = false;
		private System.Timers.Timer aTimer = null;

		private MethodInvoker updateIt = null;

		private BackgroundWorker worker = null;
		BackgroundWorker worker2 = new BackgroundWorker();

		#region public methods

		public OpenGLWindow()
		{
			InitializeComponent();

			MakeCurrent();

			shaderManager = new ShaderManager();
			shaderManager.createProgram( @"" );

			worker = new BackgroundWorker();
			worker.DoWork += DoWork;
		}

		public void SetRenderingMethod( IRenderEngine _renderEngine )
		{
			renderEngine = _renderEngine;

			RayCaster Engine = renderEngine as RayCaster;
			if ( Engine != null )
				Engine.ShaderPrograms = shaderManager.ShaderPrograms;

			//MakeCurrent();

			//this.Context.MakeCurrent(null);
			//animationManager.setup( this, renderEngine );
		}

		public void SetShaderList( List<string> list )
		{
			shaderList = list;
		}

		//public Color ClearColor
		//{
		//    get { return _clearColor; }

		//    set
		//    {
		//        _clearColor = value;

		//        if ( !DesignMode )
		//        {
		//            MakeCurrent();
		//            GL.ClearColor( 0.5f, 10.5f, 10.1f, 0.9f );
		//        }
		//    }
		//}

		#endregion public methods

		//protected override void OnPaint( PaintEventArgs e )
		//{
		//    base.OnPaint( e );

		//    if ( !DesignMode )
		//    {
		//        // FIXME: debug remove
		//        //foreach ( string path in shaderList )
		//        //{
		//        //  shaderManager.createProgram( path );
		//        //}

		//        //FIXME: workaround. Maybe could be possible to run this in different function
		//        RayCaster Engine = renderEngine as RayCaster;
		//        if ( Engine != null )
		//            Engine.ShaderPrograms = shaderManager.ShaderPrograms;

		//        // animation
		//        if ( StartupInit == false )
		//        {
		//            MakeCurrent();

		//            animationManager.setup( this, renderEngine );
		//            //Engine.setup();

		//            StartupInit = true;
		//        }
		//    }

		//    if ( !DesignMode )
		//    {
		//        MakeCurrent();

		//        GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

		//        renderEngine.Render( Width, Height );

		//        SwapBuffers();
		//    }
		//}

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

			aTimer = new System.Timers.Timer();
			aTimer.Elapsed += new ElapsedEventHandler( OnTimedEvent );
			// Set the Interval to 5 seconds.
			aTimer.Interval = 1000;
			aTimer.Enabled = false;

			this.Invalidate();
			this.Update();

			renderNow = true;

			// worker.DoWork += DoWork;
			// worker.RunWorkerAsync();
		}

		private void OnTimedEvent( object source, ElapsedEventArgs e )
		{
			renderNow = true;
			Invalidate();
			//this.Update();
		}

		private void OpenGLWindow_Load( object sender, System.EventArgs e )
		{
			loaded = true;
		}

		private void OpenGLWindow_Resize( object sender, System.EventArgs e )
		{
			Debug.WriteLine( "{0} {1}", this.Width, this.Height );

			try
			{
				if ( !this.Context.IsCurrent && !worker2.IsBusy )
					this.MakeCurrent();
			}
			catch
			{
				int a = 5;
			}
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
						//this.Invalidate();
						//this.Update();
						renderEngine.RotateXYZ();
						//renderNow = false;
					};
					BeginInvoke( updateIt );
				};

				Thread newThread = new Thread( ts );
				newThread.Priority = ThreadPriority.Lowest;
				newThread.Start();
			}
		}

		private void OpenGLWindow_Paint( object sender, PaintEventArgs e )
		{
			//this.Invalidate();
			//if ( renderNow )
			{
				//MethodInvoker updateIt = delegate
				//{
				//    renderEngine.Render( Width, Height, this );
				//    this.Invalidate();
				//    SwapBuffers();
				//    renderEngine.RotateXYZ();
				//    renderNow = false;
				//    //this.Invalidate();
				//    //this.Update();
				//};

				//lock ( this )
				//	BeginInvoke( updateIt );

				Render();
				Update();

				//lock ( this )
				//{
				//    ThreadStart ts = delegate
				//    {
				//        MethodInvoker updateIt = delegate
				//        {
				//            renderEngine.Render( Width, Height, this );
				//            this.Invalidate();
				//            SwapBuffers();
				//            renderEngine.RotateXYZ();
				//            renderNow = false;
				//            //this.Invalidate();
				//            //this.Update();
				//        };
				//        BeginInvoke( updateIt );
				//    };

				//    Thread newThread = new Thread( ts );
				//    newThread.Priority = ThreadPriority.BelowNormal;
				//    newThread.Start();

				//    //ts.BeginInvoke( null, null );
				//}

				//this.Context.MakeCurrent( null );

				//this.Context.MakeCurrent(null);
				//	this.MakeCurrent();
				//this.Context.MakeCurrent( null );
				//DoBgWork();

				//this.Context.MakeCurrent( null );

				worker2.DoWork += ( sender2, e2 ) =>
				{
					//this.MakeCurrent();
					RayCaster Engine = renderEngine as RayCaster;

					Engine.Render2( Width, Height, this );
					Engine.RotateXYZ();
					this.SwapBuffers();

					this.Context.MakeCurrent( null );
				};

				//lock ( this )
				//	worker2.RunWorkerAsync();

				//this.BeginInvoke( updateIt, DispatcherPriority.Background );

				//Dispatcher.CurrentDispatcher.BeginInvoke( updateIt, DispatcherPriority.SystemIdle );

				//renderEngine.Render( Width, Height, this );

				//Thread.Sleep(5000);

				//this.Invalidate();
				//this.Update();
			}
		}

		private void DoBgWork()
		{
			worker.WorkerReportsProgress = false;
			//worker.DoWork += DoWork;

			worker.RunWorkerAsync();
		}

		private void DoWork( object sender, DoWorkEventArgs e )
		{
			//this.MakeCurrent();
			//RayCaster Engine = renderEngine as RayCaster;

			////Engine.DrawCube();
			//Engine.Render2( Width, Height, this );

			//this.SwapBuffers();
			//this.Context.MakeCurrent( null );
		}

		/*
		private void OpenGLWindow_Paint( object sender, PaintEventArgs e )
		{
			if(!loaded) return;

			//MakeCurrent();

			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

			if (this.IsIdle)
			{
				//renderEngine.Render( Width, Height );
				//SwapBuffers();
			}
		}
		*/
	}
}