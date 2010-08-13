using System;
using System.Drawing;
using System.Drawing.Imaging;
using NLog;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace WpfOpenTK.OpenGL.Utils
    {
    internal class FrameBufferObject
        {
        #region private fields

        private readonly uint fbo_handle;
        private uint mDepthBuffer;
        private bool mDetached;
        private uint mFboTexture;
        private int mHeight;
        private int mWidth;
        private TextureUnit textureUnit;

        #endregion private fields

        #region public properties

        public string Description
            {
            get;
            set;
            }

        #endregion public properties

        #region public functions

        public FrameBufferObject( int width, int height, TextureUnit textureUnit, string description )
            {
            SetFrameBufferHandles( width, height, textureUnit, description );

            GL.GenTextures( 1, out mFboTexture );

            SetupFrameBufferTexture();

            GL.Ext.GenFramebuffers( 1, out fbo_handle );
            }

        public void ChangeResolution( int width, int height, TextureUnit textureUnit, string description )
            {
            SetFrameBufferHandles( width, height, textureUnit, description );

            Detach();

            SetupFrameBufferTexture();
            }

        public void BindTextureToTextureUnit()
            {
            GL.ActiveTexture( textureUnit );
            GL.BindTexture( TextureTarget.Texture2D, mFboTexture );
            }

        public void Attach()
            {
            mDetached = false;

            GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, fbo_handle );
            GL.Ext.FramebufferTexture2D( FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                                        TextureTarget.Texture2D, fbo_handle, 0 );
            }

        public void Detach()
            {
            if ( mDetached == false )
                {
                mDetached = true;
                GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, 0 ); // Move back to the default framebuffer.
                //GL.BindTexture( TextureTarget.Texture2D, 0 ); // prevent feedback, reading and writing to the same image is a bad idea
                }
            }

        public void Save( string fileName )
            {
            var bitmap = new Bitmap( mWidth, mHeight );

            BitmapData data = bitmap.LockBits( new Rectangle( 0, 0, mWidth, mHeight ),
                                               ImageLockMode.WriteOnly,
                                               PixelFormat.Format32bppArgb );

            GL.ReadBuffer( (ReadBufferMode) FramebufferAttachment.ColorAttachment0Ext ); // Set up where to read the pixels from.

            GL.ReadPixels( 0, 0, mWidth, mHeight, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0 );
            // Read the pixels into the bitmap.

            Detach();

            GL.ReadBuffer( ReadBufferMode.Back );
            // Set the read buffer to the back (I don't think this is necessary, but cleaning up is generally a good idea).

            bitmap.UnlockBits( data ); // Unlock the bitmap data.

            try
                {
                bitmap.Save( fileName, ImageFormat.Bmp );
                }
            catch ( Exception e )
                {
                // FIXME: Log this
                }
            }

        #endregion public functions

        #region private functions

        private void SetupFrameBufferTexture()
            {
            GL.ActiveTexture( textureUnit );
            GL.BindTexture( TextureTarget.Texture2D, mFboTexture );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp );

            GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
                           mWidth, mHeight, 0,
                           OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                           PixelType.UnsignedByte,
                           IntPtr.Zero );

            ErrorCode glError = GL.GetError();
            if ( glError != ErrorCode.NoError )
                {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error( glError.ToString() + "Frame Buffer Object - setup problem" );
                throw new Exception( "Frame Buffer Object - setup problem" );
                }

            GL.BindTexture( TextureTarget.Texture2D, 0 ); // prevent feedback, reading and writing to the same image is a bad idea

            #region depth buffer setup

            // Create Depth Renderbuffer
            //GL.Ext.GenRenderbuffers( 1, out depthBuffer );
            //GL.Ext.BindRenderbuffer( RenderbufferTarget.RenderbufferExt, depthBuffer );
            //GL.Ext.RenderbufferStorage( RenderbufferTarget.RenderbufferExt, (RenderbufferStorage) All.DepthComponent32, width, height );

            #endregion depth buffer setup
            }

        private void SetFrameBufferHandles( int width, int height, TextureUnit TextureUnit, string description )
            {
            mWidth      = width;
            mHeight     = height;
            textureUnit = TextureUnit;
            Description = description;

            if ( Description.CompareTo( "back" ) == 0 )
                mFboTexture = 1;
            else if ( Description.CompareTo( "front" ) == 0 )
                mFboTexture = 2;
            }

        #endregion private functions
        }
    }