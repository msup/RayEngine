using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using AvalonDock;
using Data;
using Plugin;
using Plugins;
using WpfOpenTK.OpenGL.Engine;
using WpfOpenTK.User_Controls;

namespace WpfOpenTK
{
	public class PluginManager
	{
		private List<String> plugins = new List<String>();
		private WindowManager wm = null;
		private RenderWindowManager rwm = null;
		private DatasetManager dsm = null;
        private AnimationManager anim_man = null;

		public List<String> Plugins
		{
			get
			{
				return plugins;
			}
			set
			{
				plugins = value;
			}
		}

		public PluginManager( WindowManager wm, RenderWindowManager rwm, DatasetManager dsm, AnimationManager anim_man )
		{
			this.wm  = wm;
			this.rwm = rwm;
			this.dsm = dsm;
            this.anim_man = anim_man;
		}

		public bool loadPluginList( string Path )
		{
			if ( !Directory.Exists( Path ) )
				return false;

			List<IPlugin> Plugins = new List<IPlugin>();

			foreach ( string f in Directory.GetFiles( Path ) )
			{
				FileInfo fi = new FileInfo( f );

				if ( fi.Extension.Equals( ".dll" ) )
				{
					plugins.Add( fi.Name );

					activatePlugin( f, ref Plugins );
					return true; // remove debug
				}
			}

			return true;
		}

		public bool activatePlugin( string PluginFile, ref List<IPlugin> Plugins )
		{
			Assembly asm = null;

			try
			{
				// FIXME: check only for valid .net assemblies with manifest
				asm = Assembly.LoadFile( PluginFile );

				if ( asm == null )
					return false;
			}
			catch ( Exception e )
			{
			}

			Type PluginClass = null;
			int i = 0;

			// iterate through all types in assembly

			// !! FIXME: for debug only

			//foreach ( Type type in asm.GetTypes() )
			//{
			//PluginClass = type;

			// !! FIXME: for debug only
			//if ( PluginClass is IRenderEngine )
			// if a rendering able plugin was found
			//{
			try
			{
				// create an instance of rendering plugin
				//IRenderEngine tplugin = Activator.CreateInstance( PluginClass ) as IRenderEngine;

				IRenderEngine tplugin = (IRenderEngine) new RayCaster( dsm, anim_man );

				ModelOrientationControl orientationControl = new ModelOrientationControl(tplugin,wm);
				var dockPane = new DockablePane();

				dockPane.Items.Add( new DockableContent()
				{
					Name = "orientationContent",
					Title = "Orientation",
					Content = orientationControl
				} );

				var resHorizontalPanel = new ResizingPanel()
				{
					Orientation = Orientation.Horizontal
				};

				resHorizontalPanel.Children.Add( dockPane );

				//resHorizontalPanel.Children.Add( logDockPane);
				this.wm.DockingManager.Content = resHorizontalPanel;

				var ccc = this.wm.DockingManager.Content;

				if ( tplugin != null )
					Plugins.Add( tplugin );

				// setup a new RenderWindow
				RenderWindow renderWindow = new RenderWindow();
				renderWindow.RenderEngine = tplugin;

				//var renderingDataset = dsm.getRenderingDatasetRef();
				//renderWindow.SetRenderingDataset( ref renderingDataset );

				OpenGLWindow glWindow = new OpenGLWindow();
				glWindow.SetRenderingMethod( tplugin );

				renderWindow.GlWindows.Add( glWindow );
				this.rwm.RenderWindows.Add( renderWindow );

				// setup marching cubes opengl window
				IRenderEngine marchingCubesPlugin = (IRenderEngine) new MarchingCubes( dsm );

				if ( marchingCubesPlugin != null )
					Plugins.Add( marchingCubesPlugin );

				// setup a new RenderWindow
				RenderWindow renderWindow2 = new RenderWindow();
				renderWindow2.RenderEngine = marchingCubesPlugin;

				OpenGLWindow glWindow2 = new OpenGLWindow();
				glWindow2.SetRenderingMethod( marchingCubesPlugin );

				renderWindow2.GlWindows.Add( glWindow2 );
				//this.rwm.RenderWindows.Add(renderWindow2);

				wm.Register( this.rwm );
			}
			//}

			catch ( Exception e )
			{
				MessageBox.Show( "Shit happened during plugin registration: " + PluginFile + e.Message );
			}

			return true;
		}
	}
}