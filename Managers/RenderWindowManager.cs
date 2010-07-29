using System;
using System.Collections.Generic;
using System.Text;
using VolumeRenderingEngines;
using System.Windows;

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