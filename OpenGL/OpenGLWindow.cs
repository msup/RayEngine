using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using Plugin;
using WpfOpenTK.OpenGL.Engine;
using OpenTK.Graphics.OpenGL;

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
        private Thread renderThread = null;

        #endregion private fields

        // TODO:
        //' FIXME
        // this remove to different class
        float renderingStep = 0.005f;
        private int actRenderStep = 0;
        private bool loaded = false;
        private ThreadStart ts = null;
            

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

            actRenderStep = 0;

            //ts = delegate
            //{
            //    MethodInvoker updateIt = delegate
            //    {
            //        renderEngine.Render( 555, 555, this, renderingStep );
            //        renderEngine.RotateXYZ();
            //        SwapBuffers();
            //    };
            //    Invoke( updateIt );
            //};
                 
        }

        private void renderProgressTimer_Elapsed( object sender, System.Timers.ElapsedEventArgs e )
        {
            //if ( renderingStep > 0.001 )
            //{
            //    renderingStep /= 1.5f;
            //    renderProgressTimer.Interval = 500;
            //    Invalidate();
            //}

            List<float> Steps = new List<float>()
                                    {
                                        0.1f,
                                        //0.05f,
                                        //0.01f,
                                        //0.005f,
                                        //0.001f,
                                        //0.0008f,
                                        //0.0004f,
                                        //0.00005f,
                                        ////0.0001f,
                                        //0.00008f,
                                        //0.00005f,
                                        //0.00002f
                                    };

            renderingStep = Steps[actRenderStep];
            if(actRenderStep < Steps.Count - 1) {
                actRenderStep += 1;
                //renderProgressTimer.Interval = 750;
                var msg = "Render step: " + Steps[actRenderStep].ToString();
                Trace.WriteLine( msg );
                Invalidate();
            }
        }

        public void Render( float renderingStep )
        {
            lock(this) {

                //renderEngine.Render( Width,Height, this, renderingStep );
                renderEngine.Render( 333, 333, this, renderingStep );
                renderEngine.RotateXYZ();
                SwapBuffers();

                // //renderEngine.Render( Width, Height, this, renderingStep );
                // //                                    renderEngine.RotateXYZ();
                 //                                    SwapBuffers();

                //ThreadStart ts = delegate
                //{
                //    MethodInvoker updateIt = delegate
                //                                 {
                //                                     renderEngine.Render( 333, 333, this, renderingStep );
                //                                     renderEngine.RotateXYZ();
                //                                     SwapBuffers();
                //                                 };
                //    Invoke( updateIt );
                //};

                ////////if ( renderThread == null )
                ////{

                
                //renderThread = new Thread( ts );
                //renderThread.Priority = ThreadPriority.BelowNormal;
                //renderThread.Name = "Rendering Thread";
                //renderThread.Start();
                
            
            }
                /*else if ( renderThread.IsAlive == true )
                    {
                    actRenderStep -= 1;
                    renderProgressTimer.Interval = 1000;
                    //renderThread.Join();
                    //renderProgressTimer.Enabled = true;
                    }
                else
                    {
                    renderThread.Priority = ThreadPriority.Lowest;
                    renderThread.Name = "Rendering Thread";
                    renderThread.Start();
                    }*/
            //}
        }

        public void SetRenderingMethod( IRenderEngine _renderEngine )
        {
            renderEngine = _renderEngine;

            RayCaster Engine = renderEngine as RayCaster;
            if(Engine != null)
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

            //Size = new System.Drawing.Size( 111,111);
            this.Load += new System.EventHandler( this.OpenGLWindow_Load );
            this.Paint += new System.Windows.Forms.PaintEventHandler( this.OpenGLWindow_Paint );
            this.Resize += new EventHandler( OpenGLWindow_Resize );
            this.SizeChanged += new EventHandler( OpenGLWindow_MarginChanged );
            
            //this.AutoSize = true;
            //this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.Name = "OpenGLWindow";

            this.ResumeLayout( false );

            //this.Invalidate();
            //this.Update();
        }

        private void OpenGLWindow_Load( object sender, System.EventArgs e )
        {
            loaded = true;

            //SetupViewport();
        }

        private void SetupViewport()
        {
            int w = 800;
            int h = 600;
            GL.MatrixMode( MatrixMode.Projection );
            GL.LoadIdentity();
            GL.Ortho( 0, w, 0, h, -1, 1 ); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport( 0, 0, w, h ); // Use all of the glControl painting area
        }
        
        private void OpenGLWindow_MarginChanged( object sender, System.EventArgs e )
        {
            renderProgressTimer.Enabled = false;
            //renderingStep = 0.02f;

            Debug.WriteLine( "{0} {1}", this.Width, this.Height );
            actRenderStep = 0;
        }

        private void OpenGLWindow_Resize( object sender, System.EventArgs e )
        {
            if(!loaded)
                return;

            Debug.WriteLine( "{0} {1}", this.Width, this.Height );

            if(!this.Context.IsCurrent)
                this.MakeCurrent();
            else {
                //renderingStep = 0.02f;
                actRenderStep = 0;
                //renderProgressTimer.Enabled = false;
                //renderProgressTimer.Interval = 100;
                //renderProgressTimer.Enabled = true; // FIXME: true to be working
                Update();
            }
        }

        private void OpenGLWindow_Paint( object sender, PaintEventArgs e )
        {
        
            Render( renderingStep );
            //renderProgressTimer.Interval = 500;

            //renderProgressTimer.Enabled = false;
            //renderProgressTimer.Enabled = true; // FIXME: true to be working

        }

        #endregion private methods
    }
}