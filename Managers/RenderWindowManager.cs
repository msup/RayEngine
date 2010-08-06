using System.Collections.Generic;

namespace WpfOpenTK
{
	public class RenderWindowManager
	{
		private List<RenderWindow> renderWindows = new List<RenderWindow>();

		public List<RenderWindow> RenderWindows
		{
			get
			{
				return renderWindows;
			}
			set
			{
				renderWindows = value;
			}
		}
	}
}