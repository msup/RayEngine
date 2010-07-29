using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using OpenTK;

namespace Plugin
{
	public interface IRenderEngine : IPlugin
	{
		#region Operations (1)

		//VolumeData data;
		void Render( int width, int height, GLControl glControl );
    
		void Setup( int width, int height );

		void RotateXYZ();


		#endregion Operations
	}
}