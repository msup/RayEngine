using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;
using Data;
using WpfOpenTK.Managers;
using Fluent;
using AvalonDock;

namespace WpfOpenTK
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow
	{
		#region private fields - managers
		private RenderWindowManager rwm = null;
		private DatasetManager dsm = null;
		private LoggingManager lm = null;
		private WindowManager wm = null;
		private PluginManager pm = null;
		#endregion

		#region Constructors 

		public MainWindow()
		{
			InitializeComponent();
			
			lm = new LoggingManager();

			wm  = new WindowManager( RibbonBar, StatusBar, DockWindow, DockingWindowsManager );

			rwm = new RenderWindowManager();

			dsm = new DatasetManager();

			pm  = new PluginManager( this.wm, this.rwm, this.dsm );

			pm.loadPluginList( Directory.GetCurrentDirectory() );
		}

		#endregion Constructors

		#region Properties (5)

		public DockPanel DockWindow
		{
			get { return ( new DockPanel() ); }
		}

		public RibbonBarPanel RibbonBar
		{
			get { return ( new RibbonBarPanel() ); }
		}

		public StatusBar StatusBar
		{
			get { return statusBar; }
		}

		//public TabControl TabControl
		//{
		//    get { return tabControl; }
		//}

		public DockingManager DockingWindowsManager
		{
			get { return dockingManager; }
		}

		#endregion Properties
	}
}