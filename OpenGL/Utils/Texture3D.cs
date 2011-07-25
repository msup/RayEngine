using System;
using OpenTK.Graphics.OpenGL;

namespace WpfOpenTK.OpenGL.Utils
{
	public class GlTexture3D
	{
		int mTextureHandle = 2;

		public GlTexture3D( int width, int height, int depth )
		{
			GL.Enable( EnableCap.Texture3DExt );

			GL.GenTextures( 1, out mTextureHandle );

			GL.ActiveTexture( TextureUnit.Texture2 );
			GL.BindTexture( TextureTarget.Texture3D, mTextureHandle );

			GL.Ext.TexImage3D(
								TextureTarget.Texture3D,
								0,
								PixelInternalFormat.Four,
								width, height, depth,
								0,
								PixelFormat.Blue, PixelType.Byte,
								IntPtr.Zero
								);

			//GL.PixelStore( PixelStoreParameter.UnpackAlignment, 1 );
			//GL.PixelStore( PixelStoreParameter.PackAlignment, 1 );

			GL.TexParameter( TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear );
			GL.TexParameter( TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear );
			GL.TexParameter( TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToBorder );
			GL.TexParameter( TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToBorder );
			GL.TexParameter( TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int) TextureWrapMode.ClampToBorder );
			GL.BindTexture( TextureTarget.Texture3D, 0 );
		}

		public void Load( int width, int height, int depth, byte[, ,] textureData )
		{
			//GL.ActiveTexture( TextureUnit.Texture2 );
			GL.BindTexture( TextureTarget.Texture3D, mTextureHandle );

			//	GL.PixelStore( PixelStoreParameter.UnpackAlignment, 1 );
			//	GL.PixelStore( PixelStoreParameter.PackAlignment, 1 );

			GL.Ext.TexImage3D( TextureTarget.Texture3D,
							   0,
							   PixelInternalFormat.Four,
							   width, height, depth, 0,
							   PixelFormat.Blue,
							   PixelType.Byte,
							   textureData );


       

			GL.BindTexture( TextureTarget.Texture3D, 0 );
		}

		public void Load( int width, int height, int depth, byte[] textureData )
		{
			//GL.ActiveTexture( TextureUnit.Texture2 );
			GL.BindTexture( TextureTarget.Texture3D, mTextureHandle );

			//	GL.PixelStore( PixelStoreParameter.UnpackAlignment, 1 );
			//	GL.PixelStore( PixelStoreParameter.PackAlignment, 1 );

			GL.Ext.TexImage3D( TextureTarget.Texture3D, 0, PixelInternalFormat.Four, width, height, depth, 0, PixelFormat.Blue, PixelType.Byte, textureData );
			GL.BindTexture( TextureTarget.Texture3D, 0 );
		}

		public void Bind()
		{
			//GL.ActiveTexture( TextureUnit.Texture2 );
			GL.BindTexture( TextureTarget.Texture3D, mTextureHandle );
			GL.Enable( EnableCap.Texture3DExt );
		}

		public void Unbind()
		{
			//GL.ActiveTexture( TextureUnit.Texture2 );
			GL.BindTexture( TextureTarget.Texture3D, 0 );
			GL.Disable( EnableCap.Texture3DExt );
		}
	}
}