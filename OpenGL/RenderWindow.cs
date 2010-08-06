using System.Collections.Generic;
using Data;
using Plugin;

namespace WpfOpenTK
{
	public class RenderWindow
	{
		//one or more opengl windows form one RenderWindow
		//OpenGLWindow frame is then used according the # of windows
		private List<OpenGLWindow> glWindows = new List<OpenGLWindow>();

		public List<OpenGLWindow> GlWindows
		{
			get
			{
				return glWindows;
			}
			set
			{
				glWindows = value;
			}
		}

		public IRenderEngine RenderEngine
		{
			get;
			set;
		}

		public void SetRenderingDataset( ref RenderingDataset renderingDataset )
		{
			this.renderingDataset = renderingDataset;
		}

		private RenderingDataset renderingDataset = null;

		public RenderingDataset RenderDataset
		{
			get
			{
				return renderingDataset;
			}
			set
			{
				renderingDataset = value;
			}
		}

		public string Description
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}
	}
}