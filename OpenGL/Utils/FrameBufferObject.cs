using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace VolumeRenderingEngines
{
	class FrameBufferObject
	{
		int width;
		int height;
		uint fbo_texture;
		uint fbo_handle;
		uint depthBuffer;
		bool detached = false;
		TextureUnit textureUnit;

		public string Description { get; set; }
		 

		public FrameBufferObject( int width, int height, TextureUnit textureUnit, string  description )
		{
			setFrameBufferHandles( width, height, textureUnit, description );
		
			// create, bind & setup parameters of texture
			
			GL.GenTextures( 1, out fbo_texture );

			SetupFrameBufferTexture();
			
			GL.Ext.GenFramebuffers( 1, out fbo_handle );
		}

		private void SetupFrameBufferTexture()
		{
			GL.ActiveTexture( this.textureUnit );
			GL.BindTexture( TextureTarget.Texture2D, fbo_texture );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp );
			GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, this.width, this.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero );
			// test for GL Error here (might be unsupported format)

			GL.BindTexture( TextureTarget.Texture2D, 0 ); // prevent feedback, reading and writing to the same image is a bad idea

			// Create Depth Renderbuffer
			//GL.Ext.GenRenderbuffers( 1, out depthBuffer );
			//GL.Ext.BindRenderbuffer( RenderbufferTarget.RenderbufferExt, depthBuffer );
			//GL.Ext.RenderbufferStorage( RenderbufferTarget.RenderbufferExt, (RenderbufferStorage) All.DepthComponent32, width, height );
		}

		private void setFrameBufferHandles( int width, int height, TextureUnit textureUnit, string description )
		{
			this.width = width;
			this.height = height;
			this.textureUnit = textureUnit;
			Description = description;

			if ( Description.CompareTo( "back" )  == 0 )
				fbo_texture = 1;
			else if ( Description.CompareTo( "front" ) == 0 )
				fbo_texture = 2;
		}

		public void ChangeResolution(int width, int height, TextureUnit textureUnit, string description)
		{
			setFrameBufferHandles( width, height, textureUnit, description );

			Detach();

			SetupFrameBufferTexture();

			//GL.Ext.GenFramebuffers( 1, out fbo_handle );
		}

		public void BindTextureToTextureUnit()
		{
			GL.ActiveTexture( this.textureUnit );
			GL.BindTexture( TextureTarget.Texture2D, fbo_texture );
		}

		public void Attach()
		{
			detached = false;

			GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, fbo_handle );
			GL.Ext.FramebufferTexture2D( FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, fbo_handle, 0 );
		}

		public void Detach()
		{
			if ( detached == false )
			{
				detached = true;
				GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, 0 ); // Move back to the default framebuffer.
				//GL.BindTexture( TextureTarget.Texture2D, 0 ); // prevent feedback, reading and writing to the same image is a bad idea			
			}
		}

		public void Save( string fileName )
		{
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap( this.width, this.height );

			System.Drawing.Imaging.BitmapData data = bitmap.LockBits( new System.Drawing.Rectangle( 0, 0, width, height ),
																	 System.Drawing.Imaging.ImageLockMode.WriteOnly,
																	 System.Drawing.Imaging.PixelFormat.Format32bppArgb );

			GL.ReadBuffer( (ReadBufferMode) FramebufferAttachment.ColorAttachment0Ext ); // Set up where to read the pixels from.

			GL.ReadPixels( 0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0 ); // Read the pixels into the bitmap.

			Detach();

			GL.ReadBuffer( ReadBufferMode.Back ); // Set the read buffer to the back (I don't think this is necessary, but cleaning up is generally a good idea).

			bitmap.UnlockBits( data ); // Unlock the bitmap data.

			try
			{
				bitmap.Save( fileName, System.Drawing.Imaging.ImageFormat.Bmp );
			}
			catch ( Exception e )
			{
				// FIXME: Log this
			}


		}
	}
}
