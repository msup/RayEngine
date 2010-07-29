using System;
using GLSL;
using OpenTK.Graphics.OpenGL;

namespace WpfOpenTK
{
	public class Shader : IShader
	{
		#region Fields (2)

		protected String _compileInfo;
		protected String shaderSource;
		private int shaderID = -1;

		#endregion Fields

		#region Properties (1)

		public string CompileInfo
		{
			get
			{
				return _compileInfo;
			}
		}

		#endregion Properties

		public int ShaderID
		{
			get
			{
				return shaderID;
			}
			set
			{
				shaderID = value;
			}
		}

		#region Methods (1)

		// Public Methods (1) 

		public void LoadFromFile( string path )
		{
			try
			{
				System.IO.StreamReader vs = new System.IO.StreamReader( path );
				shaderSource = vs.ReadToEnd();
			}
			catch ( IndexOutOfRangeException e )
			{
			}
		}

		#endregion

		#region Constructors (1)

		public Shader( string path, ShaderType shaderType )
		{
			// load shader source from file
			LoadFromFile( path );

			// create shader object
			ShaderID = GL.CreateShader( shaderType );

			// load source code of shaderID
			GL.ShaderSource( ShaderID, shaderSource );

			// compile the shaderID
			GL.CompileShader( ShaderID );

			// check for errors
			GL.GetShaderInfoLog( ShaderID, out _compileInfo );
		}

		#endregion Constructors
	}
}