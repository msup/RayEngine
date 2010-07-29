using OpenTK;
using Plugins;

namespace Plugin
{
	public interface IRenderEngine : IPlugin
	{
		#region Operations (1)

		//VolumeData data;
		void Render( int width, int height, GLControl glControl, float renderingStep );

		void Setup( int width, int height );

		void RotateXYZ();

		#endregion Operations
	}
}