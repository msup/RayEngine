using OpenTK.Graphics.OpenGL;

namespace WpfOpenTK
{
    public class GLTexture1D
    {
        private uint m_texture = 0;

        public GLTexture1D( uint[] texels )
        {
            int width = texels.Length;

            GL.Enable( EnableCap.Texture1D );

            GL.GenTextures( 1, out m_texture );

            GL.ActiveTexture( TextureUnit.Texture3 );

            GL.BindTexture( TextureTarget.Texture1D, m_texture );
            GL.TexParameter( TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest );
            GL.TexParameter( TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest );
            GL.TexParameter( TextureTarget.Texture1D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp );
            GL.TexParameter( TextureTarget.Texture1D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp );

            //GL.TexImage1D( TextureTarget.Texture1D, 0, PixelInternalFormat.Rgba8, width, 0, PixelFormat.Rgba,
            //               PixelType.UnsignedInt, texels );

            GL.TexImage1D( TextureTarget.Texture1D,
                       0,
                       PixelInternalFormat.Rgba8,
                       width,
                       0,
                       PixelFormat.Rgba,
                       PixelType.UnsignedInt8888,
                       texels );

//GL.TexImage1D( TextureTarget.ProxyTexture1D, 0, PixelInternalFormat.Rgba, width, 0, PixelFormat.Rgba, PixelType.UnsignedInt, texels );


            GL.BindTexture( TextureTarget.Texture1D, 0 );

            // FIXME: check opengl error code
            ErrorCode error = GL.GetError();
        }

        public void Bind()
        {
            GL.ActiveTexture( TextureUnit.Texture3 );
            GL.BindTexture( TextureTarget.Texture1D, m_texture );
            GL.Enable( EnableCap.Texture1D );
            ErrorCode error = GL.GetError();
        }

        /// <summary>
        /// refactor this function to be general
        /// </summary>
        public void Bind_zDistance()
        {
            GL.ActiveTexture( TextureUnit.Texture4 );
            GL.BindTexture( TextureTarget.Texture1D, m_texture );
            GL.Enable( EnableCap.Texture1D );
            ErrorCode error = GL.GetError();
        }


        public void Unbind()
        {
            GL.ActiveTexture( TextureUnit.Texture3 );
            GL.BindTexture( TextureTarget.Texture1D, 0 );
            GL.Disable( EnableCap.Texture1D );
        }
    }
}