using System;
using System.Collections.Generic;
using System.IO;
using Data;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Plugin;
using WpfOpenTK;
using WpfOpenTK.OpenGL;
using WpfOpenTK.OpenGL.Engine;
using WpfOpenTK.OpenGL.Utils;

namespace VolumeRenderingEngines
{
	class RayCaster : IRenderEngine, IOrientation
	{
		#region Fields (3)

		int widthOld                          = 0;
		int heightOld                         = 0;
		private float angleX                  = 0.0f;
		private float angleY                  = 0.0f;
		private float angleZ                  = 0.0f;
		private float translateZ              = 0.0f;
		private float angle                   = 1.5f;

		DatasetManager datasetManager         = null;
		FrameBufferObject backSide            = null;
		FrameBufferObject frontSide           = null;
		GLControl openglControl               = null;

		GLTexture3D volumeTexture                  = null;
		GLTexture1D classificationTexture          = null;
		private List<ShaderProgram> shaderPrograms = null;

		public List<ShaderProgram> ShaderPrograms
		{
			get
			{
				return shaderPrograms;
			}
			set
			{
				shaderPrograms = value;
			}
		}

		private List<string> shaderPaths = new List<string>();

		private bool firstRun = true;

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

		public string Name
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		#endregion Fields

		#region Constructors (1)

		public RayCaster( DatasetManager datasetManager )
		{
			System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();

			angleX     = (float) configurationAppSettings.GetValue( "RotateX", typeof( float ) );
			angleY     = (float) configurationAppSettings.GetValue( "RotateY", typeof( float ) );
			angleZ     = (float) configurationAppSettings.GetValue( "RotateZ", typeof( float ) );
			translateZ = (float) configurationAppSettings.GetValue( "TranslateZ", typeof( float ) );

			// NOTE: what for is this path?
			shaderPaths.Add( @"../../Data/Shaders/simple.frag" );
			LocateShaders();

			this.datasetManager = datasetManager;
		}

		#endregion Constructors

		#region Properties (1)

		//public PixelShader Pshader
		//{
		//  get { return pshader; }
		//  set { pshader = value; }
		//}

		public IRenderingDataset VolumeData
		{
			get;
			set;
		}

		public bool IsLoaded
		{
			get;
			set;
		}

		public double xAxis
		{
			get;
			set;
		}

		public double yAxis
		{
			get;
			set;
		}

		public double zAxis
		{
			get;
			set;
		}

		#endregion Properties

		#region Methods (3)

		// Public Methods (1) 
		public void execute()
		{
			throw new NotImplementedException(
			  "You should not execute Raycaster via execute() calling, but Render(int width, int height) " );
		}

		public void Setup( int width, int height )
		{
			int dataWidth  = datasetManager.RenderingDataset.Data3D.Width;
			int dataHeight = datasetManager.RenderingDataset.Data3D.Height;
			int dataDepth  = datasetManager.RenderingDataset.Data3D.Depth;

			GL.Enable( EnableCap.DepthTest );
			GL.Enable( EnableCap.CullFace );
			GL.Enable( EnableCap.Texture2D );
			GL.Enable( EnableCap.Texture3DExt );

			backSide  = new FrameBufferObject( width, height, TextureUnit.Texture0, "back" );
			frontSide = new FrameBufferObject( width, height, TextureUnit.Texture1, "front" );

			volumeTexture = new GLTexture3D( dataWidth, dataHeight, dataDepth );
			volumeTexture.Load( dataWidth, dataHeight, dataDepth, datasetManager.RenderingDataset.Data3D.VolumeDataArray );

			// create Load function for GLTexture1D
			classificationTexture = new GLTexture1D( datasetManager.RenderingDataset.LookUpTables[0].LUTData1D );
		}

		public void Render()
		{
			if ( openglControl != null )
			{
				openglControl.Invalidate();
			}
		}

		public void Render( int width, int height, GLControl glControl, float renderingStep )
		{
			glControl.MakeCurrent();

			#region First run - FBO setup

			if ( firstRun == true )
			{
				this.Setup( width, height );

				widthOld = width;
				heightOld = height;
				openglControl = glControl;

				firstRun = false;
			}

			#endregion

			#region Resolution change detection

			if ( ( width != widthOld ) || ( height != heightOld ) )
			{
				backSide.ChangeResolution( width, height, TextureUnit.Texture0, "back" );
				frontSide.ChangeResolution( width, height, TextureUnit.Texture1, "front" );
			}

			#endregion

			#region OpenGL states setup

			GL.Enable( EnableCap.DepthTest );
			GL.Enable( EnableCap.CullFace );
			GL.Enable( EnableCap.Texture2D );
			GL.Enable( EnableCap.Texture3DExt );

			#endregion

			#region Projection Setup

			GL.Viewport( 0, 0, width, height ); // Use all of the glControl painting area
			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();

			float x = (float) width / height;
			GL.Ortho( -x, x, -1.0, 1.0, 0.0, 5.0 );
			GL.Ortho( -0.7, 0.7, -0.7, 0.7, 0, 5 );

			#endregion

			#region Cube scene preparation

			GL.MatrixMode( MatrixMode.Modelview );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			GL.LoadIdentity();

			GL.Translate( 0.1, 0.1, translateZ );
			GL.Rotate( angleX, 1, 0, 0 );
			GL.Rotate( angleY, 0, 1, 0 );
			GL.Rotate( angleZ, 0, 0, 1 );

			#endregion

			#region Render Cube Back Side to FBO

			GL.FrontFace( FrontFaceDirection.Cw );
			//GL.ActiveTexture( TextureUnit.Texture0 );
			//FrameBufferObject backSide  = new FrameBufferObject( width, height, TextureUnit.Texture0,"back" );
			backSide.Attach();
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			DrawCube();
			//backSide.Save( "back.bmp" );
			backSide.Detach();

			#endregion

			#region Render Cube Front Side to FBO

			GL.FrontFace( FrontFaceDirection.Ccw );
			//FrameBufferObject frontSide = new FrameBufferObject( width, height, TextureUnit.Texture1, "front" );
			frontSide.Attach();
			//GL.ActiveTexture( TextureUnit.Texture1 );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			DrawCube();
			//frontSide.Save("front.bmp");
			frontSide.Detach();

			#endregion

			#region Prepare scene for Fullscreen textured rectangle - changed by GLSL shader

			GL.ClearColor( 1.0f, 1.0f, 1.0f, 1.0f );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			GL.LoadIdentity();
			GL.Translate( -2.00 * width / height / 2, -2.0 / 2, -0.0 );
			GL.Disable( EnableCap.CullFace );

			#endregion

			#region Setup/Release GLSL Program & setup uniform values, setup textures to textureUnits

			// Draw fullscreen textured rectangle

			// Activate 3D texture

			GL.ActiveTexture( TextureUnit.Texture2 );
			this.volumeTexture.Bind();

			GL.ActiveTexture( TextureUnit.Texture3 );
			this.classificationTexture.Bind();

			this.ShaderPrograms[0].Use();
			ShaderPrograms[0].setUniform1( "backBuffer", 0 );
			ShaderPrograms[0].setUniform1( "frontBuffer", 1 );
			//renderingStep = 0.001f;
			ShaderPrograms[0].setUniform1( "delta", renderingStep );
			ShaderPrograms[0].setUniform1( "classificationTexture", 3 );

			//DrawCube();
			frontSide.BindTextureToTextureUnit();
			backSide.BindTextureToTextureUnit();

			FullScreenArea( width, height );
			ShaderPrograms[0].setUniform1( "volume", 2 );
			GL.UseProgram( 0 );

			GL.ActiveTexture( TextureUnit.Texture2 );
			this.volumeTexture.Unbind();

			GL.ActiveTexture( TextureUnit.Texture3 );
			this.classificationTexture.Unbind();

			GL.ActiveTexture( TextureUnit.Texture0 );
			GL.BindTexture( TextureTarget.Texture2D, 0 );
			GL.ActiveTexture( TextureUnit.Texture1 );
			GL.BindTexture( TextureTarget.Texture2D, 0 );

			#endregion

			#region remove

			// GL.Enable( EnableCap.DepthTest );
			GL.Enable( EnableCap.Blend );
			GL.BlendFunc( BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha );
			// GL.Enable(EnableCap.AlphaTest);

			GL.PushMatrix();
			GL.LoadIdentity();
			GL.Translate( 0.1, 0.1, -3.1 );
			//GL.Rotate( angle, 1, 1, 0 );

			GL.Begin( BeginMode.Triangles );
			GL.Color4( 1.0, 0.5, 1.0, 0.3 );
			GL.Vertex3( 0.1, 0.31, -5.0 );
			GL.Vertex3( -0.13, 0.21, -5.0 );
			GL.Vertex3( 0.23, 0.81, -5.0 );

			GL.End();
			GL.Disable( EnableCap.Blend );
			GL.PopMatrix();

			#endregion

			widthOld  = width;
			heightOld = height;
		}

		private void FullScreenArea( int width, int height )
		{
			int W = width, H = height;

			double x = (double) 2.0 * W / H;
			double y = (double) 2.0;

			GL.Disable( EnableCap.DepthTest );
			GL.Begin( BeginMode.Quads );

			//    glTexCoord2f(0,0);
			GL.Color3( 0.5, 0.2, 0.9 );

			GL.MultiTexCoord4( TextureUnit.Texture0, 0.0, 0.0, 0.0, 0.0 );
			GL.MultiTexCoord4( TextureUnit.Texture1, 0.0, 0.0, 0.0, 0.0 );
			GL.Vertex2( 0, 0 );

			//    glTexCoord2f(1,0);
			GL.MultiTexCoord4( TextureUnit.Texture0, 1.0, 0.0, 0.0, 0.0 );
			GL.MultiTexCoord4( TextureUnit.Texture1, 1.0, 0.0, 0.0, 0.0 );
			GL.Vertex2( x, 0 );

			//    glTexCoord2f(1, 1);
			GL.MultiTexCoord4( TextureUnit.Texture0, 1.0, 1.0, 0.0, 0.0 );
			GL.MultiTexCoord4( TextureUnit.Texture1, 1.0, 1.0, 0.0, 0.0 );
			GL.Vertex2( x, y );

			//    glTexCoord2f(0, 1);
			GL.MultiTexCoord4( TextureUnit.Texture0, 0.0, 1.0, 0.0, 0.0 );
			GL.MultiTexCoord4( TextureUnit.Texture1, 0.0, 1.0, 0.0, 0.0 );
			GL.Vertex2( 0, y );

			GL.End();
		}

		public void RotateXYZ()
		{
			angle += 10.0f;
		}

		// Private Methods (2) 

		#region Utility functions

		private void ColorVector( float x, float y, float z )
		{
			const double correction = 0.5f;
			GL.Color4( x + correction, y + correction, z + correction, 1.0 );
			GL.Vertex3( x, y, z );
		}

		public void DrawCube()
		{
			//float length = 1.0f;
			System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
			string renderCubeSideLength = (string) configurationAppSettings.GetValue( "RenderCubeSideLength", typeof( string ) );
			float length = float.Parse( renderCubeSideLength );

			float k      = 1.0f;

			GL.Begin( BeginMode.Quads );

			// Front Side
			this.ColorVector( -length, -length, +length );
			this.ColorVector( +k*length, -length, +length );
			this.ColorVector( +k*length, +length, +length );
			this.ColorVector( -length, +length, +length );

			// Back Side
			this.ColorVector( -length, -length, -length );
			this.ColorVector( -length, +length, -length );
			this.ColorVector( +k*length, +length, -length );
			this.ColorVector( +k*length, -length, -length );

			// Top Side
			this.ColorVector( -length, +length, -length );
			this.ColorVector( -length, +length, +length );
			this.ColorVector( +k*length, +length, +length );
			this.ColorVector( +k*length, +length, -length );

			// Bottom Side
			this.ColorVector( -length, -length, -length );
			this.ColorVector( +k*length, -length, -length );
			this.ColorVector( +k*length, -length, +length );
			this.ColorVector( -length, -length, +length );

			// Right Side
			this.ColorVector( +k*length, -length, -length );
			this.ColorVector( +k*length, +length, -length );
			this.ColorVector( +k*length, +length, +length );
			this.ColorVector( +k*length, -length, +length );

			// Left Side
			this.ColorVector( -length, -length, -length );
			this.ColorVector( -length, -length, +length );
			this.ColorVector( -length, +length, +length );
			this.ColorVector( -length, +length, -length );

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