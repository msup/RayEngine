using System;
using NLog;
using OpenTK.Graphics.OpenGL;

namespace WpfOpenTK
{
    public class GLTexture2D
    {
        private uint m_texture = 0;

        private GLTexture2D( int width, int height, uint textureID )
        {
            m_texture = textureID;
            uint FboHandle;

            GL.GenTextures( 1, out m_texture );
            GL.BindTexture( TextureTarget.Texture2D, m_texture );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                            (int) TextureMinFilter.Nearest );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                            (int) TextureMagFilter.Nearest );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp );
            GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba,
                          PixelType.UnsignedByte, IntPtr.Zero );

            #region error handling

            ErrorCode glError = GL.GetError();
            if(glError != ErrorCode.NoError) {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error( glError.ToString() + "GL Texture2D - problem" );
                throw new Exception( " GL Texture2D problem" );
            }

            #endregion error handling

            // test for GL Error here (might be unsupported format)

            GL.BindTexture( TextureTarget.Texture2D, 0 );
            // prevent feedback, reading and writing to the same image is a bad idea

            // Create a FBO and attach the textures
            GL.Ext.GenFramebuffers( 1, out FboHandle );
            GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, FboHandle );
            GL.Ext.FramebufferTexture2D( FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                                        TextureTarget.Texture2D, m_texture, 0 );

            // now GL.Ext.CheckFramebufferStatus( FramebufferTarget.FramebufferExt ) can be called, check the end of this page for a snippet.

            // since there's only 1 Color buffer attached this is not explicitly required
            GL.DrawBuffer( (DrawBufferMode) FramebufferAttachment.ColorAttachment0Ext );

            GL.PushAttrib( AttribMask.ViewportBit ); // stores GL.Viewport() parameters
            GL.Viewport( 0, 0, width, height );

            // render whatever your heart desires, when done ...

            GL.PopAttrib(); // restores GL.Viewport() parameters
            GL.Ext.BindFramebuffer( FramebufferTarget.FramebufferExt, 0 ); // return to visible framebuffer
        }
    }
}