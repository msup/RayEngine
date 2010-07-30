using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Plugin;
using VolumeRenderingEngines;

namespace WpfOpenTK.User_Controls
{
	/// <summary>
	/// Interaction logic for ModelOrientationControl.xaml
	/// </summary>
	public partial class ModelOrientationControl : UserControl
	{
		private IRenderEngine renderPlugin;
		private delegate void renderDelegate();

		public ModelOrientationControl( IRenderEngine renderPlugin )
		{
			InitializeComponent();

			this.renderPlugin = renderPlugin;
		}

		private void btnRender_Click( object sender, RoutedEventArgs e )
		{
			var engine = renderPlugin as RayCaster;

			// ok working
			//if ( engine == null )
			//    throw new Exception( "Render Engine object is null." );
			//else
			//    Dispatcher.BeginInvoke( new renderDelegate( engine.Render ) );

			ThreadStart ts = delegate
			{
				Dispatcher.BeginInvoke( new renderDelegate( engine.Render ) );
				// crashed! engine.Render();
			};

			lock ( this )
			{
				Thread newThread = new Thread( ts );
				newThread.Priority = ThreadPriority.BelowNormal;
				newThread.Start();
			}
		}
	}
}