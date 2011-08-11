using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Plugin;
using WpfOpenTK.OpenGL.Utils;
using System.Drawing;
using System.Drawing.Imaging;

namespace WpfOpenTK.OpenGL.Engine
{
    public class RayCaster : IRenderEngine, IOrientation
    {
        #region Fields (3)

        private Dictionary<RenderingParameter, object> mRenderingParameters = null;
        AnimationManager anim_man = new AnimationManager();

        int mWidthOld = 0;
        int mHeightOld = 0;
        int mWidth = 0, mHeight = 0;
        //private float mAngleX = 0.0f;
        //private float mAngleY = 0.0f;
        //private float mAngleZ = 0.0f;
        //private float mTranslateZ = 0.0f;
        private float mAngle = 1.5f;
        private static int counter = 0;

        public bool offline_render { get; set; }

        DatasetManager mDatasetManager = null;
        FrameBufferObject mBackSide = null;
        FrameBufferObject mFrontSide = null;
        FrameBufferObject mTempFBO = null;
        GLControl mOpenglControl = null;
        GlTexture3D mVolumeTexture = null;
        GLTexture1D mClassificationTexture = null;
        GLTexture1D mDistanceTexture = null;

        public List<ShaderProgram> ShaderPrograms { get; set; }
        public AnimationManager animationManager { get { return this.anim_man; } }

        private bool firstRun = true;

        private List<string> shaderPaths = new List<string>();

        public List<string> ShaderPaths
        {
            get
            {
                return shaderPaths;
            }
            set
            {
                shaderPaths = value;
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }

        #endregion Fields

        #region Constructors (1)

        public RayCaster(DatasetManager datasetManager)
        {
            ShaderPrograms = null;
            mRenderingParameters = new Dictionary<RenderingParameter, object>
                                    {
                                        { RenderingParameter.AmbientLightColor,		 new Vector3( 0.69f, 0.78f, 0.80f) },
                                        { RenderingParameter.AmbientLightCoeff,		 0.4f},

                                        { RenderingParameter.DiffuseLightColor,		 new Vector3( 0.25f, 0.28f, 0.99f) },
                                        { RenderingParameter.DiffuseLightCoeff,		 0.4f},

                                        { RenderingParameter.SpecularLightColor,	 new Vector3( 0.94f, 0.15f, 0.15f) },
                                        { RenderingParameter.SpecularLightCoeff,	 0.8f },
                                        { RenderingParameter.SpecularPowerFactor,    0.5f },

                                        { RenderingParameter.FinalAlphaMixColor,	 new Vector3( 1.0f, 1.0f, 1.0f) },
                                        { RenderingParameter.FinalAlphaMixFactor,	 0.50f },
                                        { RenderingParameter.FinalAlphaMixThreshold, 0.05f },
                                    };

            var configurationAppSettings = new System.Configuration.AppSettingsReader();

            if (firstRun == true)
            {
                AngleX = (float)configurationAppSettings.GetValue("RotateX", typeof(float));
                AngleY = (float)configurationAppSettings.GetValue("RotateY", typeof(float));
                AngleZ = (float)configurationAppSettings.GetValue("RotateZ", typeof(float));
                TranslateZ = (float)configurationAppSettings.GetValue("TranslateZ", typeof(float));
            }

            // NOTE: what for is this path?
            shaderPaths.Add(@"../../Data/Shaders/simple.frag");
            LocateShaders();

            this.mDatasetManager = datasetManager;
            offline_render = false;
        }

        public RayCaster(DatasetManager datasetManager, AnimationManager anim_man)
            : this(datasetManager)
        {
            this.anim_man = anim_man;
        }

        #endregion Constructors

        #region Properties (1)

        //public PixelShader Pshader
        //{
        //  get { return pshader; }
        //  set { pshader = value; }
        //}

        public IRenderingDataset VolumeData { get; set; }
        public bool IsLoaded { get; set; }
        public double xAxis { get; set; }
        public double yAxis { get; set; }
        public double zAxis { get; set; }

        double mAngleX;
        public double AngleX
        {
            get { return mAngleX; }
            set { mAngleX = value; }
        }

        public double AngleY { get; set; }
        public double AngleZ { get; set; }

        public double TranslateZ { get; set; }
        #endregion Properties

        #region Methods (3)

        // Public Methods (1) 
        public void execute()
        {
            throw new NotImplementedException("You should not execute Raycaster via execute() calling, but Render(int width, int height) ");
        }

        public void Setup(int width, int height)
        {
            mWidth = width;
            mHeight = height;

            int dataWidth = mDatasetManager.RenderingDataset.Data3D.Width;
            int dataHeight = mDatasetManager.RenderingDataset.Data3D.Height;
            int dataDepth = mDatasetManager.RenderingDataset.Data3D.Depth;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Texture3DExt);

            mBackSide = new FrameBufferObject(width, height, TextureUnit.Texture0, "back");
            mFrontSide = new FrameBufferObject(width, height, TextureUnit.Texture1, "front");
            mTempFBO = new FrameBufferObject(width, height, TextureUnit.Texture4, "temp");

            mVolumeTexture = new GlTexture3D(dataWidth, dataHeight, dataDepth);
            mVolumeTexture.Load(dataWidth, dataHeight, dataDepth, mDatasetManager.RenderingDataset.Data3D.VolumeDataArray);

            // create Load function for GLTexture1D
            mClassificationTexture = new GLTexture1D(mDatasetManager.RenderingDataset.LookUpTables[0].LUTData1D);
            mDistanceTexture = new GLTexture1D(mDatasetManager.RenderingDataset.LookUpTables[1].LUTData1D);
        }

        public void SetOrientation()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            GL.Translate(0.1, 0.1, 0);
            GL.Rotate(AngleX, 1, 0, 0);
            GL.Rotate(AngleY, 0, 1, 0);
            GL.Rotate(AngleZ, 0, 0, 1);
            GL.Rotate(mAngle, 1, 1, 0);
        }

        public void Render()
        {
            if (mOpenglControl != null)
            {
                mOpenglControl.Invalidate();
                mOpenglControl.SwapBuffers();
            }
        }

        public void Render(int width, int height, GLControl glControl, float renderingStep)
        {
            if (!glControl.Context.IsCurrent)
                glControl.MakeCurrent();

            #region First run - FBO setup

            if (firstRun == true)
            {
                // this.Setup( glControl.Width, glControl.Height );
                this.Setup(555, 555);
                // mWidthOld = width;
                // mHeightOld = height;
                mOpenglControl = glControl;

                firstRun = false;
            }

            #endregion

            #region Resolution change detection

            //if((width != mWidthOld) || (height != mHeightOld)) {
            //    mBackSide.ChangeResolution( width, height, TextureUnit.Texture0, "back" );
            //    mFrontSide.ChangeResolution( width, height, TextureUnit.Texture1, "front" );
            //}

            #endregion

            #region OpenGL states setup

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Texture3DExt);

            #endregion

            #region Projection Setup


            GL.Viewport(0, 0,555, 555); // Use all of the glControl painting area
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            float x = (float)width / height;
            //GL.Ortho( -x, x, -1.0, 1.0, 0.0, 5.0 );
            //GL.Ortho( -0.7, 0.7, -0.7, 0.7, 0, 5 );
            //GL.Ortho(-x,x,-x,x, -50, 50);
            GL.Ortho(-1.50, 1.50, -1.50, 1.50, -5, 10);
            //GL.Frustum(-50, 50, -50, 50, -1, 100);


            //GL.Ortho( 0, width, 0, height, -1, 1 ); // Bottom-left corner pixel has coordinate (0, 0)
            //GL.Viewport( 0, 0, width, height );
            //GL.Viewport( 0, 0, 222,222 );

            #endregion

            #region Cube scene preparation

            SetOrientation();

            #endregion

            #region Render Cube Back Side to FBO

            GL.FrontFace(FrontFaceDirection.Cw);
            //GL.ActiveTexture( TextureUnit.Texture0 );
            //FrameBufferObject backSide  = new FrameBufferObject( width, height, TextureUnit.Texture0,"back" );
            mBackSide.Attach();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawCube();
            mBackSide.Save("back.bmp");
            mBackSide.Detach();

            #endregion

            #region Render Cube Front Side to FBO

            GL.FrontFace(FrontFaceDirection.Ccw);
            //FrameBufferObject frontSide = new FrameBufferObject( width, height, TextureUnit.Texture1, "front" );
            mFrontSide.Attach();
            //GL.ActiveTexture( TextureUnit.Texture1 );
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawCube();
            //mFrontSide.Save( "front.bmp" );
            mFrontSide.Detach();
            #endregion

            #region Prepare scene for Fullscreen textured rectangle - changed by GLSL shader

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();
            GL.Translate(-3.0 * width / height / 2, -3.0 / 2, 0.0);
            GL.Disable(EnableCap.CullFace);

            #endregion

            #region Setup/Release GLSL Program & setup uniform values, setup textures to textureUnits

            // Draw fullscreen textured rectangle

            // Activate 3D texture
            GL.ActiveTexture(TextureUnit.Texture2);
            this.mVolumeTexture.Bind();

            GL.ActiveTexture(TextureUnit.Texture3);
            this.mClassificationTexture.Bind();

            GL.ActiveTexture(TextureUnit.Texture4);
            this.mClassificationTexture.Bind_zDistance();

            this.ShaderPrograms[0].Use();
            ShaderPrograms[0].SetUniform1("backBuffer", 0);
            ShaderPrograms[0].SetUniform1("frontBuffer", 1);

            //renderingStep = 0.001f;

            foreach (var shaderAlgorithmParameter in mRenderingParameters)
            {
                if (shaderAlgorithmParameter.Value.GetType().Name == typeof(Vector3).Name)
                {
                    ShaderPrograms[0].SetUniform3(
                                         shaderAlgorithmParameter.Key.ToString(),
                                         (Vector3)shaderAlgorithmParameter.Value);
                }
                else
                {
                    ShaderPrograms[0].SetUniform1(
                                        shaderAlgorithmParameter.Key.ToString(),
                                        (float)shaderAlgorithmParameter.Value);
                }
            }

            //ShaderPrograms[0].SetUniform3( RenderingParameter.AmbientLightColor.ToString(),
            //                               (Vector3) mRenderingParameters[RenderingParameter.AmbientLightColor] );

            ShaderPrograms[0].SetUniform1("delta", renderingStep);
            ShaderPrograms[0].SetUniform1("color_text", 3);
            ShaderPrograms[0].SetUniform1("z_distance_texture", 4);

            ////DrawCube();
            mFrontSide.BindTextureToTextureUnit();
            mBackSide.BindTextureToTextureUnit();

            if (offline_render)
            {
                mTempFBO.Attach();
            }

            FullScreenArea(111, 111);
            //       FullScreenArea( width, height );

            ShaderPrograms[0].SetUniform1("volume", 2);
            GL.UseProgram(0);

            if (offline_render)
            {
                counter++;
                string fbo_bitmap_str = String.Format("{0}-fbo_save.bmp", counter);
                mTempFBO.Save(fbo_bitmap_str);
                mTempFBO.Detach();
            }

            GL.ActiveTexture(TextureUnit.Texture2);
            this.mVolumeTexture.Unbind();

            GL.ActiveTexture(TextureUnit.Texture3);
            this.mClassificationTexture.Unbind();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture1D, 0);

            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture1D, 0);


            #endregion

            #region remove
            //GL.Viewport( 0, 0, 400, 400); // Use all of the glControl painting area
            //GL.MatrixMode( MatrixMode.Projection );
            //GL.LoadIdentity();
            //GL.Ortho( -1, 1, -1, 1, -50, 50 );
            //////FullScreenArea( width, height );

            ////// GL.Enable( EnableCap.DepthTest );
            ////GL.Enable( EnableCap.Blend );
            ////GL.BlendFunc( BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha );
            ////// GL.Enable(EnableCap.AlphaTest);

            //GL.PushMatrix();
            //GL.LoadIdentity();
            ////GL.Translate( 0.5, 0.5, 0 );
            ////GL.Rotate( angle, 1, 1, 0 );

            //GL.ClearColor( 1.0f, 1.0f, 1.0f, 1.0f );
            //GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
            //GL.ActiveTexture( TextureUnit.Texture0 );
            //GL.BindTexture( TextureTarget.Texture2D, mTempFBO.mFboTexture );

            //GL.Begin( BeginMode.Quads );
            //GL.Color4( 1.0, 1.0, 1.0, 1.0); // fixme:: jeden pes je zakopany aj tu

            ////GL.Vertex3( 0.1, 0.31, 0.0 );
            ////GL.Vertex3( -0.13, 0.21, 0.0 );
            ////GL.Vertex3( 0.23, 0.81, -0.0 );

            //GL.TexCoord2( 0.0, 0.0 ); GL.Vertex2( 0.0, 0.0 );
            //GL.TexCoord2( 1.0, 0.0 ); GL.Vertex2( 1.0, 0.0 );
            //GL.TexCoord2( 1.0, 1.0 ); GL.Vertex2( 1.0, 1.0 );
            //GL.TexCoord2( 0.0, 1.0 ); GL.Vertex2( 0.0, 1.0 );

            //GL.End();
            //GL.BindTexture( TextureTarget.Texture2D, 0 );


            ////GL.Disable( EnableCap.Blend );
            //GL.PopMatrix();

            #endregion


            mWidthOld = width;
            mHeightOld = height;
        }

        private void FullScreenArea(int width, int height)
        {
            int W = width, H = height;

            double x = (double)2.0 * W / H / 1.0;
            double y = (double)2.0 / 1.0;

            GL.Disable(EnableCap.DepthTest);

            GL.Begin(BeginMode.Quads);

            //    glTexCoord2f(0,0);
            GL.Color3(0.0, 0.0, 0.0);

            GL.MultiTexCoord4(TextureUnit.Texture0, 0.0, 0.0, 0.0, 0.0);
            GL.MultiTexCoord4(TextureUnit.Texture1, 0.0, 0.0, 0.0, 0.0);
            GL.Vertex2(0, 0);

            //    glTexCoord2f(1,0);
            GL.MultiTexCoord4(TextureUnit.Texture0, 1.0, 0.0, 0.0, 0.0);
            GL.MultiTexCoord4(TextureUnit.Texture1, 1.0, 0.0, 0.0, 0.0);
            GL.Vertex2(x, 0);

            //    glTexCoord2f(1, 1);
            GL.MultiTexCoord4(TextureUnit.Texture0, 1.0, 1.0, 0.0, 0.0);
            GL.MultiTexCoord4(TextureUnit.Texture1, 1.0, 1.0, 0.0, 0.0);
            GL.Vertex2(x, y);

            //    glTexCoord2f(0, 1);
            GL.MultiTexCoord4(TextureUnit.Texture0, 0.0, 1.0, 0.0, 0.0);
            GL.MultiTexCoord4(TextureUnit.Texture1, 0.0, 1.0, 0.0, 0.0);
            GL.Vertex2(0, y);

            GL.End();
        }

        public void RotateXYZ()
        {
            //mAngle += 6.0f;
        }

        public void GrabScreenshot()
        {
            if (mOpenglControl != null)
            {
                var screen = mOpenglControl.GrabScreenshot();

                var screenshots = from file in Directory.GetFiles(".", "*.bmp")
                                  select file;

                screen.Save((screenshots.Count() + 1).ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }


        public void Save(string fileName)
        {
            GL.Viewport(0, 0, mOpenglControl.Width, mOpenglControl.Height); // Use all of the glControl painting area
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            mTempFBO.Attach();

            mTempFBO.Save(fileName);
            mTempFBO.Detach();

            var bitmap = new Bitmap(mOpenglControl.Width, mOpenglControl.Height);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, mOpenglControl.Width, mOpenglControl.Height),
                                               ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.ReadBuffer((ReadBufferMode)FramebufferAttachment.ColorAttachment0Ext); // Set up where to read the pixels from.

            GL.ReadPixels(0, 0, mOpenglControl.Width, mOpenglControl.Height, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);
            // Read the pixels into the bitmap.

            mTempFBO.Detach();


            //uint[] pixs = new uint[mOpenglControl.Width*mOpenglControl.Height];
            //GL.ReadPixels(0, 0, mOpenglControl.Width, mOpenglControl.Height, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedInt8888, pixs);

            GL.ReadBuffer(ReadBufferMode.Back);
            // Set the read buffer to the back (I don't think this is necessary, but cleaning up is generally a good idea).

            bitmap.UnlockBits(data); // Unlock the bitmap data.

            try
            {
                bitmap.Save(fileName, ImageFormat.Bmp);
            }
            catch (Exception e)
            {
                // FIXME: Log this
            }
        }


        // Private Methods (2) 

        #region Utility functions

        private void ColorVector(float x, float y, float z)
        {
            const double correction = 0.5f;
            GL.Color4(x + correction, y + correction, z + correction, 1.0);
            GL.Vertex3(x, y, z);
        }

        public void DrawCube()
        {
            //float length = 1.0f;
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string renderCubeSideLength = (string)configurationAppSettings.GetValue("RenderCubeSideLength", typeof(string));
            float length = float.Parse(renderCubeSideLength);

            float k = 1.0f;

            GL.Begin(BeginMode.Quads);

            // Front Side
            this.ColorVector(-length, -length, +length);
            this.ColorVector(+k * length, -length, +length);
            this.ColorVector(+k * length, +length, +length);
            this.ColorVector(-length, +length, +length);

            // Back Side
            this.ColorVector(-length, -length, -length);
            this.ColorVector(-length, +length, -length);
            this.ColorVector(+k * length, +length, -length);
            this.ColorVector(+k * length, -length, -length);

            // Top Side
            this.ColorVector(-length, +length, -length);
            this.ColorVector(-length, +length, +length);
            this.ColorVector(+k * length, +length, +length);
            this.ColorVector(+k * length, +length, -length);

            // Bottom Side
            this.ColorVector(-length, -length, -length);
            this.ColorVector(+k * length, -length, -length);
            this.ColorVector(+k * length, -length, +length);
            this.ColorVector(-length, -length, +length);

            // Right Side
            this.ColorVector(+k * length, -length, -length);
            this.ColorVector(+k * length, +length, -length);
            this.ColorVector(+k * length, +length, +length);
            this.ColorVector(+k * length, -length, +length);

            // Left Side
            this.ColorVector(-length, -length, -length);
            this.ColorVector(-length, -length, +length);
            this.ColorVector(-length, +length, +length);
            this.ColorVector(-length, +length, -length);

            /*
            // front
            this.ColorVector( -length, -length, -length );
            this.ColorVector( +length, -length, -length );
            this.ColorVector( +length, +length, -length );
            this.ColorVector( -length, +length, -length );

            // back
            this.ColorVector( -length, -length, +length );
            this.ColorVector( +length, -length, +length );
            this.ColorVector( +length, +length, +length );
            this.ColorVector( -length, +length, +length );

            //bottom
            this.ColorVector( -length, -length, -length );
            this.ColorVector( +length, -length, -length );
            this.ColorVector( +length, -length, +length );
            this.ColorVector( -length, -length, +length );

            //up
            this.ColorVector( -length, +length, -length );
            this.ColorVector( +length, +length, -length );
            this.ColorVector( +length, +length, +length );
            this.ColorVector( -length, +length, -length );

            //left
            this.ColorVector( -length, -length, -length );
            this.ColorVector( -length, -length, +length );
            this.ColorVector( -length, +length, +length );
            this.ColorVector( -length, +length, -length );
            */

            //// Front Side
            //this.ColorVector( -length, -length, +length );
            //this.ColorVector( +1*length, -length, +length );
            //this.ColorVector( +1*length, +length, +length );
            //this.ColorVector( -length, +length, +length );

            //// Back Side
            //this.ColorVector( -length, -length, -length );
            //this.ColorVector( -length, +length, -length );
            //this.ColorVector( +length, +length, -length );
            //this.ColorVector( +length, -length, -length );

            //// Top Side
            //this.ColorVector( -length, +length, -length );
            //this.ColorVector( -length, +length, +length );
            //this.ColorVector( +length, +length, +length );
            //this.ColorVector( +length, +length, -length );

            //// Bottom Side
            //this.ColorVector( -length, -length, -length );
            //this.ColorVector( +1*length, -length, -length );
            //this.ColorVector( +1*length, -length, +length );
            //this.ColorVector( -length, -length, +length );

            //// Right Side
            //this.ColorVector( +length, -length, -length );
            //this.ColorVector( +length, +length, -length );
            //this.ColorVector( +length, +length, +length );
            //this.ColorVector( +length, -length, +length );

            //// Left Side
            //this.ColorVector( -length, -length, -length );
            //this.ColorVector( -length, -length, +length );
            //this.ColorVector( -length, +length, +length );
            //this.ColorVector( -length, +length, -length );

            GL.End();
        }

        #endregion

        private void LocateShaders()
        {
            string directory = Directory.GetCurrentDirectory();
        }

        #endregion Methods
    }
}