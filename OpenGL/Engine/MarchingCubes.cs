using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugin;
using OpenTK.Graphics.OpenGL;
using Data;
using System.Diagnostics;

namespace WpfOpenTK.OpenGL.Engine
{
	internal class MarchingCubes : IRenderEngine
	{
		static double angle = 0.0;
		private double aa, bb, cc;

		public MarchingCubes( DatasetManager datasetManager )
		{
			Random rand = new Random();
			aa = rand.NextDouble();
			bb = rand.NextDouble();
			cc = rand.NextDouble();
		}
		
		#region IRenderEngine Members

		public void Render( int width, int height, OpenTK.GLControl glControl )
		{

			angle += 0.5;

			Stopwatch sw = new Stopwatch();
			sw.Start();

			glControl.MakeCurrent();


			#region Projection Setup
			GL.Enable( EnableCap.DepthTest );

			GL.MatrixMode( MatrixMode.Projection );
			GL.LoadIdentity();

			//double c1 = 2.00;
			//if ( width <= height )
			//    GL.Ortho( -c1, c1, -c1 * height / width, c1 * height / width, -c1, c1 );
			//else
			//    GL.Ortho( -c1 * width / height, c1 * width / height, -c1, c1, -c1, c1 );

			GL.Ortho( -10, 10, -10, 10, -25, 25 ); // Bottom-left corner pixel has coordinate (0, 0)
			GL.Viewport( 0, 0, width, height ); // Use all of the glControl painting area

			#endregion

			#region Render Setup
			GL.MatrixMode( MatrixMode.Modelview );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			GL.LoadIdentity();

			GL.Translate( -0.5, -0.5, +10.5 );
			GL.Rotate( +45, 1, 0, 0 );
			GL.Rotate( angle, 0, 1, 0 );

			#endregion

			Random rand = new Random();

			double k1 = 200, k2 = 200;
			double[,] meshGrid = new double[(int) k1, (int) k2];
			k1 /= 8;
			k2 /= 8;


			for ( int i = 0; i < meshGrid.GetLength( 0 ); i++ )
				for ( int j = 0; j < meshGrid.GetLength( 1 ); j++ )
				{
					double r = Math.Sqrt( ( i / k1 ) * ( i / k1 ) + ( j / k2 ) * ( j / k2 ) );
					meshGrid[i, j] = 0.8 * Math.Sin( 10 * r ) * Math.Sin( 2 * r ) * 10*Math.Exp( -0.8 * Math.Sqrt( r * r ) );
				}

			GL.Enable( EnableCap.PointSmooth );
			GL.Begin( BeginMode.Lines );


			aa = 1;
			bb = 0.5;
			cc = 0.2;

			for ( int i = 0; i < meshGrid.GetLength( 0 ) - 1; i++ )
				for ( int j = 0; j < meshGrid.GetLength( 1 ) - 1; j++ )
				{
					aa = rand.NextDouble();
					bb = rand.NextDouble();
					cc = rand.NextDouble();

					GL.Color3( new double[] { meshGrid[i, j] + 0.1, meshGrid[i, j] + 0.1, 0.8 } );

					GL.Vertex3( ( i + 0 ) / k1, meshGrid[i + 0, j + 0], ( j + 0 ) / k2 );
					GL.Vertex3( ( i + 0 ) / k1, meshGrid[i + 0, j + 1], ( j + 1 ) / k2 );
					GL.Vertex3( ( i + 1 ) / k1, meshGrid[i + 1, j + 1], ( j + 1 ) / k2 );

					aa = rand.NextDouble();
					bb = rand.NextDouble();
					cc = rand.NextDouble();

					//GL.Color3( new double[] { i / k1, i / k1, 0 } );

					GL.Vertex3( ( i + 0 ) / k1, meshGrid[i + 0, j + 0], ( j + 0 ) / k2 );
					GL.Vertex3( ( i + 1 ) / k1, meshGrid[i + 1, j + 1], ( j + 1 ) / k2 );
					GL.Vertex3( ( i + 1 ) / k1, meshGrid[i + 1, j + 0], ( j + 0 ) / k2 );

				}
			GL.End();
		}

		public void Setup( int width, int height )
		{
			GL.Enable( EnableCap.DepthTest );
			GL.Enable( EnableCap.CullFace );
			GL.Enable( EnableCap.Texture2D );
			GL.Enable( EnableCap.Texture3DExt );
		}

		public void RotateXYZ()
		{
		}

		#endregion

		#region IPlugin Members

		public string Name
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public bool IsLoaded
		{
			get;
			set;
		}

		public void execute()
		{
			throw new NotImplementedException();
		}

		#endregion

	}
}