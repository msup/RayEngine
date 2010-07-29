using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace WpfOpenTK
{
	public class ShaderManager
	{
		#region public properties

		public List<string> ShaderPaths
		{
			get;
			set;
		}

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

		#endregion

		#region private fields

		private List<ShaderProgram> shaderPrograms = new List<ShaderProgram>();

		#endregion

		#region public functions

		public void createProgram( string path )
		{
			// FIXME - change path
			Shader pixelShader  = new Shader( path + "Shaders/shader.frag", ShaderType.FragmentShader );
			Shader vertexShader = new Shader( path + "Shaders/shader.vert", ShaderType.VertexShader );

			// string result = pshader.CompileInfo;

			ShaderProgram sprogram = new ShaderProgram();
			sprogram.addShader( vertexShader.ShaderID );
			sprogram.addShader( pixelShader.ShaderID );

			sprogram.build( pixelShader.ShaderID );

			//sprogram.setUniform1( "backBuffer", 0 );
			//sprogram.setUniform1( "frontBuffer", 1 );

			//sprogram.attach();

			shaderPrograms.Add( sprogram );
		}

		#endregion
	}
}