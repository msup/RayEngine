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

namespace VolumeRenderingEngines
{
	class RayCaster : IRenderEngine, IOrientation
	{
		#region Fields (3)

		private float angle = 1.5f;
		//bool _init = false;
		//PixelShader pshader = null;
		DatasetManager datasetManager = null;
		Texture3D volumeTexture = null;
		FrameBufferObject backSide = null;
		FrameBufferObject frontSide = null;
		int widthOld = 0;
		int heightOld = 0;
		GLControl openglControl = null;

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

		//public void Render old( int width, int height )
		//{
		//    GL.Color3( Color.Blue );
		//    GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		//    GL.Viewport( 0, 0, width, height ); // Use all of the glControl painting area
		//    GL.MatrixMode( MatrixMode.Projection );
		//    GL.LoadIdentity();

		//    float x = (float) width/height;
		//    GL.Ortho( -x, x, -1.0, 1.0, 0.0, 200.0 );

		//    GL.MatrixMode( MatrixMode.Modelview );
		//    GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		//    GL.LoadIdentity();
		//    GL.Enable( EnableCap.CullFace );

		//    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap( width, height );
		//    System.Drawing.Imaging.BitmapData data = bitmap.LockBits( new System.Drawing.Rectangle( 0, 0, width, height ),
		//                                                             System.Drawing.Imaging.ImageLockMode.WriteOnly,
		//                                                             System.Drawing.Imaging.PixelFormat.Format32bppArgb );

		//     int FboWidth = width;
		//     int FboHeight = height;

		//    uint FboHandle;
		//    uint ColorTexture;
		//    uint DepthRenderbuffer;

		//    // Create Color Texture
		//    GL.GenTextures( 1, out ColorTexture );
		//    GL.BindTexture( TextureTarget.Texture2D, ColorTexture );
		//    GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest );
		//    GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest );
		//    GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp );
		//    GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp );
		//    GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, FboWidth, FboHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero );

		//    // test for GL Error here (might be unsupported format)

		//    GL.BindTexture( TextureTarget.Texture2D, 0 ); // prevent feedback, reading and writing to the same image is a bad idea

		//    // Create Depth Renderbuffer
		//    //GL.Ext.GenRenderbuffers( 1, out DepthRenderbuffer );
		//    //GL.Ext.BindRenderbuffer( RenderbufferTarget.RenderbufferExt, DepthRenderbuffer );
		//    //GL.Ext.RenderbufferStorage( RenderbufferTarget.RenderbufferExt, (RenderbufferStorage) All.DepthComponent32, FboWidth, FboHeight );

		//    // test for GL Error here (might be unsupported format)

		//    // Create a FBO and attach the textures
		//    GL.Ext.GenFramebuffers( 1, out FboHandle );
		//    GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, FboHandle );
		//    GL.Ext.FramebufferTexture2D( FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, ColorTexture, 0 );
		//    //GL.Ext.FramebufferRenderbuffer( FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, DepthRenderbuffer );

		//    // now GL.Ext.CheckFramebufferStatus( FramebufferTarget.FramebufferExt ) can be called, check the end of this page for a snippet.

		//    // since there's only 1 Color buffer attached this is not explicitly required
		//    GL.DrawBuffer( (DrawBufferMode) FramebufferAttachment.ColorAttachment0Ext );

		//    GL.PushAttrib( AttribMask.ViewportBit ); // stores GL.Viewport() parameters
		//    GL.Viewport( 0, 0, FboWidth, FboHeight );
		//    DrawCube();
		//    // render whatever your heart desires, when done ...

		//    GL.PopAttrib(); // restores GL.Viewport() parameters
		//    GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, 0 ); // return to visible framebuffer
		//    GL.DrawBuffer( DrawBufferMode.Back );

		//    GL.ReadBuffer( (ReadBufferMode) FramebufferAttachment.ColorAttachment0Ext ); // Set up where to read the pixels from.
		//    GL.ReadPixels( 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0 ); // Read the pixels into the bitmap.
		//    GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, 0 ); // Move back to the default framebuffer.
		//    GL.ReadBuffer( ReadBufferMode.Back ); // Set the read buffer to the back (I don't think this is necessary, but cleaning up is generally a good idea).

		//    bitmap.UnlockBits( data ); // Unlock the bitmap data.
		//    bitmap.Save( "pokus.bmp", System.Drawing.Imaging.ImageFormat.Bmp );

		//    // FBO rendering released
		//    GL.Translate( 0.1, 0.1, -1.5 );
		//    GL.Rotate( angle, 1, 1, 0 );

		//    // FIXME: debug remove comment
		//    //this.ShaderPrograms[ 0 ].attach();
		//    DrawCube();

		//    // cleaning

		//    bitmap.Dispose();
		//}

		public void Setup( int width, int height )
		{
			int dataWidth = datasetManager.RenderingDataset.Data3D.Width;
			int dataHeight = datasetManager.RenderingDataset.Data3D.Height;
			int dataDepth = datasetManager.RenderingDataset.Data3D.Depth;

			GL.Enable( EnableCap.DepthTest );
			GL.Enable( EnableCap.CullFace );
			GL.Enable( EnableCap.Texture2D );
			GL.Enable( EnableCap.Texture3DExt );

			backSide = new FrameBufferObject( width, height, TextureUnit.Texture0, "back" );
			frontSide = new FrameBufferObject( width, height, TextureUnit.Texture1, "front" );

			volumeTexture = new Texture3D( dataWidth, dataHeight, dataDepth );
			volumeTexture.Load( dataWidth, dataHeight, dataDepth, datasetManager.RenderingDataset.Data3D.RawVolumeData );
		}

		public void Render()
		{
			if ( openglControl != null )
			{
				//openglControl.MakeCurrent();

				//Render( widthOld, heightOld, openglControl );

				openglControl.Invalidate();

				//Render( widthOld, heightOld, openglControl );

				//openglControl.Update();
			}
		}

		public void Render2( int width, int height, GLControl glControl )
		{
			if ( !glControl.Context.IsCurrent )
				glControl.MakeCurrent();

			#region First run - FBO setup

			if ( firstRun == true )
			{
				this.Setup( width, height );

				widthOld = width;
				heightOld = height;
				//openglControl = glControl;

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

			GL.Translate( 0.1, 0.1, -1.5 );
			GL.Rotate( angle, 1, 1, 0 );

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

			this.ShaderPrograms[0].Use();
			ShaderPrograms[0].setUniform1( "backBuffer", 0 );
			ShaderPrograms[0].setUniform1( "frontBuffer", 1 );

			//DrawCube();
			frontSide.BindTextureToTextureUnit();
			backSide.BindTextureToTextureUnit();

			FullScreenArea( width, height );
			ShaderPrograms[0].setUniform1( "volume", 2 );
			GL.UseProgram( 0 );

			GL.ActiveTexture( TextureUnit.Texture2 );
			this.volumeTexture.Unbind();

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

			widthOld = width;
			heightOld = height;

			//Thread.Sleep( 2000 );
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

			GL.Translate( 0.1, 0.1, -1.5 );
			GL.Rotate( angle, 1, 1, 0 );

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

			this.ShaderPrograms[0].Use();
			ShaderPrograms[0].setUniform1( "backBuffer", 0 );
			ShaderPrograms[0].setUniform1( "frontBuffer", 1 );
			ShaderPrograms[0].setUniform1( "delta", renderingStep );

			//DrawCube();
			frontSide.BindTextureToTextureUnit();
			backSide.BindTextureToTextureUnit();

			FullScreenArea( width, height );
			ShaderPrograms[0].setUniform1( "volume", 2 );
			GL.UseProgram( 0 );

			GL.ActiveTexture( TextureUnit.Texture2 );
			this.volumeTexture.Unbind();

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

			widthOld = width;
			heightOld = height;

			//glControl.Context.MakeCurrent( null );
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

		public void Render3( int width, int height )
		{
			#region Projection Setup

			//GL.Color3( Color.Blue );
			//GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			GL.Viewport( 0, 0, width, height ); // Use all of the glControl painting area
			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();

			float x = (float) width / height;
			GL.Ortho( -x, x, -1.0, 1.0, 0.0, 200.0 );

			#endregion

			GL.MatrixMode( MatrixMode.Modelview );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			GL.LoadIdentity();

			GL.Enable( EnableCap.CullFace );

			//uint[] pixels = new uint[ width * height ];

			//GL.Disable(EnableCap.CullFace);
			//GL.Enable(EnableCap.Texture2D);

			// http://paste-it.net/public/h4f2876/

			//System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap( width, height );
			//System.Drawing.Imaging.BitmapData data = bitmap.LockBits( new System.Drawing.Rectangle( 0, 0, width, height ),
			//                                                         System.Drawing.Imaging.ImageLockMode.WriteOnly,
			//                                                         System.Drawing.Imaging.PixelFormat.Format32bppArgb );

			#region Framebuffer object setup - BackSide

			#endregion

			//GL.GenTextures( 1, out textureFront );
			//GL.BindTexture( TextureTarget.Texture2D, textureBack );
			//GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest );
			//GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest );
			//GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp );
			//GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp );
			//GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero );

			////// test for GL Error here (might be unsupported format)
			//GL.BindTexture( TextureTarget.Texture2D, 0 ); // prevent feedback, reading and writing to the same image is a bad idea

			//// Create a FBO and attach the textures
			//GL.Ext.GenFramebuffers( 1, out textureFront );
			//#endregion

			////GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, fboBack );
			////GL.Ext.FramebufferTexture2D( FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, fboBack, 0 );

			// //now GL.Ext.CheckFramebufferStatus( FramebufferTarget.FramebufferExt ) can be called, check the end of this page for a snippet.

			//// since there's only 1 Color buffer attached this is not explicitly required
			////GL.DrawBuffer( (DrawBufferMode) FramebufferAttachment.ColorAttachment0Ext );

			////GL.PushAttrib( AttribMask.ViewportBit ); // stores GL.Viewport() parameters
			////GL.Viewport( 0, 0, width, height );

			////// render whatever your heart desires, when done ...
			GL.Translate( 0.1, 0.1, -1.5 );
			GL.Rotate( angle, 1, 1, 0 );

			#region Render Back Side to FBO

			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			GL.FrontFace( FrontFaceDirection.Cw );

			//DrawCube();

			#endregion

			////GL.PopAttrib(); // restores GL.Viewport() parameters
			////GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, 0 ); // return to visible framebuffer

			////GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, FboHandle ); // Move to the FBO.
			//GL.ReadBuffer( (ReadBufferMode) FramebufferAttachment.ColorAttachment0Ext ); // Set up where to read the pixels from.
			//GL.ReadPixels( 0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0 ); // Read the pixels into the bitmap.

			//GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, 0 ); // Move back to the default framebuffer.
			//GL.ReadBuffer( ReadBufferMode.Back ); // Set the read buffer to the back (I don't think this is necessary, but cleaning up is generally a good idea).

			//bitmap.UnlockBits( data ); // Unlock the bitmap data.
			//bitmap.Save( "pokus.bmp", System.Drawing.Imaging.ImageFormat.Bmp );

			// //FBO rendering released
			//GL.Translate( 0.1, 0.1, -1.5 );
			//GL.Rotate( angle, 1, 1, 0 );

			// //FIXME: debug remove comment
			this.ShaderPrograms[0].Use();
			//DrawCube();

			// //cleaning

			//bitmap.Dispose();
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
			float length = 0.5f;

			GL.Begin( BeginMode.Quads );

			// Front Side
			this.ColorVector( -length, -length, +length );
			this.ColorVector( +length, -length, +length );
			this.ColorVector( +length, +length, +length );
			this.ColorVector( -length, +length, +length );

			// Back Side
			this.ColorVector( -length, -length, -length );
			this.ColorVector( -length, +length, -length );
			this.ColorVector( +length, +length, -length );
			this.ColorVector( +length, -length, -length );

			// Top Side
			this.ColorVector( -length, +length, -length );
			this.ColorVector( -length, +length, +length );
			this.ColorVector( +length, +length, +length );
			this.ColorVector( +length, +length, -length );

			// Bottom Side
			this.ColorVector( -length, -length, -length );
			this.ColorVector( +length, -length, -length );
			this.ColorVector( +length, -length, +length );
			this.ColorVector( -length, -length, +length );

			// Right Side
			this.ColorVector( +length, -length, -length );
			this.ColorVector( +length, +length, -length );
			this.ColorVector( +length, +length, +length );
			this.ColorVector( +length, -length, +length );

			// Left Side
			this.ColorVector( -length, -length, -length );
			this.ColorVector( -length, -length, +length );
			this.ColorVector( -length, +length, +length );
			this.ColorVector( -length, +length, -length );

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