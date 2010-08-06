using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.Integration;
using AvalonDock;
using WpfOpenTK.Managers;

namespace WpfOpenTK
{
	public class WindowManager
	{
		#region Fields (4)

		private DockPanel dockWindow = null;
		private RibbonBarPanel ribbonBar = null;
		private StatusBar statusBar = null;
		private TabControl tabControl = null;
		private DockingManager dockingManager = null;
		////  private List<OpenGLWindow> lstGlWindows = new List<OpenGLWindow>();
		//  private List<string> lstShadersPaths = new List<string>();

		#endregion Fields

		#region Constructors (1)

		private WindowManager()
		{
		}

		public WindowManager( RibbonBarPanel rib, StatusBar stat, DockPanel dock, DockingManager dockingManager )
		{
			RibbonBar = rib;
			StatusBar = stat;
			DockWindow = dock;
			DockingManager = dockingManager;
			//TabControl = tab;

			// TODO
			//var ribbonTab = new RibbonTabItem();
			//ribbonTab.Header = "ahoj";

			//var ribGroup = new RibbonGroup();
			//ribGroup.Caption = "Preprocessing";
			//ribbonTab.RibbonGroups.Add(ribGroup);

			//FIXME RibbonBar.Tabs.Add(ribbonTab);

			//var temp = new TabItem();
			//temp.Header = "Pokus";
			//var tabItems = TabControl.Items;

			// TODO: refactor -> RenderWindow
			//var wfh = new WindowsFormsHost();

			//RayCaster rc = new RayCaster();

			//OpenGLWindow glWindow = new OpenGLWindow();
			//glWindow.setRenderingMethod( rc );
			//lstGlWindows.Add( glWindow );

			//wfh.Child = lstGlWindows[ 0 ];

			//temp.Content = wfh;

			//TabControl.Items.Add( temp );
		}

		public void Register( RenderWindowManager rwm )
		{
			//var tabItem = new TabItem();
			//tabItem.Header = "Pokus";

			// var tabItems = TabControl.Items;

			var resHorizontalPanel = new ResizingPanel()
			{
				Orientation = Orientation.Horizontal
			};

			var resVerticalPanel = new ResizingPanel()
			{
				Orientation = Orientation.Vertical
			};

			/*
			OpenGLWindow tempGLWin = rwm.RenderWindows[0].GlWindows[0];
			var wfh = new WindowsFormsHost();
			wfh.Child = tempGLWin;

			var dockPane = new DockablePane();
			var documentPane = new DocumentPane();

			dockPane.Items.Add( new DockableContent()
			{
			  Name = "classesContent",
			  Title = "Classes"
			} );
			*/

			var dockPane = new DockablePane();

			var documentPane = new DocumentPane();

			foreach ( var openglWindow in rwm.RenderWindows )
			{
				var formHost = new WindowsFormsHost()
				{
					Child = openglWindow.GlWindows[0]
				};

				documentPane.Items.Add
				(
				  new DocumentContent()
				  {
					  Content = formHost,
					  Title = "a"
				  }
			   );
			}

			//var a = new DocumentContent();
			//a.Content = wfh;
			//a.Title = "Raycast";
			//documentPane.Items.Add( a );

			//documentPane.Items.Add( new DocumentContent()
			//{
			//    Title = "My Document!"

			//} );

			dockPane.Items.Add( new DockableContent()
			{
				Name = "classesContent",
				Title = "Logs",
			} );

			var logDockPane = new DockablePane();

			logDockPane.Items.Add( new DockableContent()
			{
				Name = "classesContent",
				Title = "Logs",
			}
			);

			//resVerticalPanel.Children.Add( logDockPane );

			ResizingPanel XXX =  DockingManager.Content as ResizingPanel;

			XXX.Children.Add( dockPane );
			XXX.Children.Add( documentPane );
			//resHorizontalPanel.Children.Add( logDockPane);

			DockingManager.Content = XXX;
		}

		//public void setShaderList( List<string> list )
		//{
		//  //foreach ( OpenGLWindow glWindow in lstGlWindows )
		//  //{
		//  //  glWindow.setShaderList( list );
		//  //}
		//}

		#endregion Constructors

		#region Properties (4)

		public DockPanel DockWindow
		{
			get
			{
				return dockWindow;
			}
			set
			{
				dockWindow = value;
			}
		}

		public RibbonBarPanel RibbonBar
		{
			get
			{
				return ribbonBar;
			}
			set
			{
				ribbonBar = value;
			}
		}

		public System.Windows.Controls.Primitives.StatusBar StatusBar
		{
			get
			{
				return statusBar;
			}
			set
			{
				statusBar = value;
			}
		}

		public System.Windows.Controls.TabControl TabControl
		{
			get
			{
				return tabControl;
			}
			set
			{
				tabControl = value;
			}
		}

		public DockingManager DockingManager
		{
			get
			{
				return dockingManager;
			}
			set
			{
				dockingManager = value;
			}
		}

		#endregion Properties
	}
}