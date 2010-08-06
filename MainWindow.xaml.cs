using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using AvalonDock;
using Data;
using NLog;
using WpfOpenTK.Managers;

namespace WpfOpenTK
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region private fields - managers

		private RenderWindowManager rwm = null;
		private DatasetManager dsm      = null;
		private LoggingManager lm       = null;
		private WindowManager wm        = null;
		private PluginManager pm        = null;

		#endregion private fields - managers

		#region Constructors

		public MainWindow()
		{
			InitializeComponent();

			Logger logger = LogManager.GetCurrentClassLogger();
			logger.Trace( "Application Starting..." );

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
			get
			{
				return ( new DockPanel() );
			}
		}

		public RibbonBarPanel RibbonBar
		{
			get
			{
				return ( new RibbonBarPanel() );
			}
		}

		public StatusBar StatusBar
		{
			get
			{
				return statusBar;
			}
		}

		//public TabControl TabControl
		//{
		//    get { return tabControl; }
		//}

		public DockingManager DockingWindowsManager
		{
			get
			{
				return dockingManager;
			}
		}

		#endregion Properties
	}
}