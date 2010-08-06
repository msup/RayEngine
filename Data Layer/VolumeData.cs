using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Data
{
	public enum Direction
	{
		Horizontal,
		Vertical,
		InDepth
	} ;

	public class VolumeData : IData
	{
		#region Fields (3)

		private int depth = 0;
		private int height = 0;
		private int width = 0;

		private Bitmap img = null;

		#endregion Fields

		#region Constructors (2)

		public VolumeData( int width, int height, int depth )
		{
			Width = width;
			Height = height;
			Depth = depth;

			volume = new double[Width, Height, Depth];
		}

		public VolumeData()
		{
		}

		#endregion Constructors

		#region Properties (4)

		//VolumeData( int width, int height, int depth, Type type )
		//{
		//    Image<Gray, Byte>[] volume=null;

		//    for ( int i=0; i<depth; ++i )
		//    {
		//        volume[ i ]=new Image<Gray, Byte>( width, height );
		//    }
		//}
		//VolumeData( int width, int height, int depth )
		//{
		//    volume=new Byte[ width, height, depth ];
		//}

		// TODO: volume shall be private, but Im testing direct access
		public Double[, ,] volume = null;

		public Double[, ,] Volume
		{
			get
			{
				return volume;
			}
			set
			{
				volume = value;
			}
		}

		public int Depth
		{
			get
			{
				return depth;
			}
			set
			{
				depth = value;
			}
		}

		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		public double Maximum
		{
			get
			{
				return GetMaximum();
			}
		}

		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		#endregion Properties

		#region Methods (9)

		// Public Methods (7) 

		//FIXME: replace operator+,-,* with delegate operator

		public VolumeData[] GetXYDifferences( VolumeData data )
		{
			VolumeData XDifferences = new VolumeData( data.Height, data.Width, data.Depth );
			VolumeData YDifferences = new VolumeData( data.Height, data.Width, data.Depth );

			for ( int i = 0; i < data.Height; ++i )
				for ( int j = 0; j < data.Width - 1; ++j )
					for ( int k = 0; k < data.Depth; ++k )
						XDifferences.Volume[i, j, k]
                            =
                            data.Volume[i, j, k]
                            -
                            data.Volume[i, j + 1, k];

			for ( int i = 0; i < data.Height - 1; ++i )
				for ( int j = 0; j < data.Width; ++j )
					for ( int k = 0; k < data.Depth; ++k )
						YDifferences.Volume[i, j, k]
                            =
                            data.Volume[i, j, k]
                            -
                            data.Volume[i + 1, j, k];

			VolumeData[] set = new VolumeData[2];
			set[0] = XDifferences;
			set[1] = YDifferences;

			return set;
		}

		public void Load2( string path )
		{
			// load an image
			//Image<Gray, double> img = new Image<Gray, double>(path);
			img = new Bitmap( path );

			// set the volume properties
			// ok
			//Width = img.Bitmap.Width;
			//Height = img.Bitmap.Height;

			Width = img.Width;
			Height = img.Height;

			Depth = 1;

			// create a new volume according to image size
			volume = new Double[Width, Height, 1];

			double temp;
			for ( int i = 0; i < Width; i++ )
				for ( int j = 0; j < Height; j++ )
				{
					try
					{
						//temp = img.Data[ j, i, 0 ];

						// FIXME: not just Red channel extraction
						temp = (double) img.GetPixel( i, j ).R;
						volume[i, j, 0] = temp;
					}
					catch ( Exception e )
					{
						int a;
					}
				}

			// FIXME
			// added only due to memory profiling
			img = null;
		}

		public void Load( string path )
		{
			// load an image
			Image<Gray, double> img = new Image<Gray, double>( path );

			// set the volume properties
			// ok
			Width = img.Width;
			Height = img.Height;

			Depth = 1;

			// create a new volume according to image size
			volume = new Double[img.Height, img.Width, 1];

			double temp;
			for ( int i = 0; i < img.Height; i++ )
				for ( int j = 0; j < img.Width; j++ )
				{
					try
					{
						temp = img.Data[i, j, 0];
						volume[i, j, 0] = temp;
					}
					catch ( Exception e )
					{
					}
				}

			// FIXME
			// added only due to memory profiling
			img = null;
		}

		private double GetMaximum()
		{
			double max = 0.0;

			for ( int i = 0; i < Width; ++i )
				for ( int j = 0; j < Height; ++j )
					for ( int k = 0; k < Depth; ++k )
					{
						if ( volume[i, j, k] > max )
							max = volume[i, j, k];
					}
			return max;
		}

		private double GetMinimum()
		{
			double min = 0.0;

			for ( int i = 0; i < Width; ++i )
				for ( int j = 0; j < Height; ++j )
					for ( int k = 0; k < Depth; ++k )
					{
						if ( volume[i, j, k] < min )
							min = volume[i, j, k];
					}
			return min;
		}

		public void Scale255()
		{
			double max = GetMaximum();
			double min = GetMinimum();
			double c = 255.0/( max - min );

			for ( int i = 0; i < Width; i++ )
				for ( int j = 0; j < Height; j++ )
					this.volume[i, j, 0] = ( this.volume[i, j, 0] - min )*c;
		}

		public void ScaleMax()
		{
			double max = GetMaximum();
			double min = GetMinimum();

			for ( int i = 0; i < Width; i++ )
				for ( int j = 0; j < Height; j++ )
					this.volume[i, j, 0] /= max;
		}

		public static VolumeData Convolve1D( VolumeData data, Direction direction )
		{
			VolumeData temp = new VolumeData( data.Width, data.Height, data.Depth );

			temp.Height = data.Height;
			temp.Width = data.Width;
			temp.Depth = data.Depth;

			int kernelSize = 5;
			double[] kernel1D = { 0.0833333333333333, -0.666666666666667, 0, 0.666666666666667, -0.0833333333333333 };
			short[] doubleIndexes = { -2, -1, 0, +1, +2 };

			// check the odd size of kernel1D

			int _depth = data.Depth;
			int _width = data.Width;
			int _height = data.Height;

			switch ( direction )
			{
				case Direction.Horizontal:
					unsafe
					{
						//for ( int i = kernelSize / 2 + 1; i < ( data.Width - ( kernelSize / 2 + 1 ) ) - 2; i++ )
						for ( int k = 0; k < _depth; k++ )
							for ( int i = 0; i < _width; i++ )
								for ( int j = 0; j < _height; j++ )
								{
									double sum = 0.0;
									for ( int m = 0; m < kernelSize; m++ )
									{
										// try
										//  {
										int indexes = i + doubleIndexes[m];
										if ( indexes <= 0 )
											indexes = Math.Abs( indexes );

										if ( indexes >= _width )
											indexes = _width - 1; //-( indexes - data.Width );

										//double a = data.Volume[ i + m, j, k ];

										double a = data[indexes, j, k];

										double b = kernel1D[m];

										sum += a*b;

										//temp[ i, j, k ] += data[ indexes, j, k ] * kernel1D[ m ];

										//}
										//catch ( Exception e )
										//{
										//    int a = 5;
										//}
									}
									// testing direct access
									temp[i, j, k] = sum;
								}
					}
					break;
			}

			return temp;
		}

		// Private Methods (2) 

		#endregion Methods

		#region Operators

		// Indexer
		public double this[int index1, int index2, int index3] // Indexer declaration
		{
			get
			{
				return volume[index1, index2, index3];
			}

			set
			{
				volume[index1, index2, index3] = value;
			}
		}

		public static VolumeData operator *( VolumeData data1, VolumeData data2 )
		{
			VolumeData temp = new VolumeData( data1.Width, data1.Height, data1.Depth );

			for ( int i = 0; i < data1.Width; ++i )
				for ( int j = 0; j < data1.Height - 1; ++j )
					for ( int k = 0; k < data1.Depth; ++k )

						temp.Volume[i, j, k]
                            =
                            data1.Volume[i, j, k]
                            *
                            data2.Volume[i, j, k];

			return temp;
		}

		public static VolumeData operator *( double c, VolumeData data2 )
		{
			VolumeData temp = new VolumeData( data2.Width, data2.Height, data2.Depth );

			for ( int i = 0; i < data2.Width; ++i )
				for ( int j = 0; j < data2.Height - 1; ++j )
					for ( int k = 0; k < data2.Depth; ++k )

						temp.Volume[i, j, k]
                            =
                            c
                            *
                            data2.Volume[i, j, k];

			return temp;
		}

		public static VolumeData operator -( VolumeData data1, VolumeData data2 )
		{
			VolumeData temp = new VolumeData( data1.Width, data1.Height, data1.Depth );

			for ( int i = 0; i < data1.Width; ++i )
				for ( int j = 0; j < data1.Height - 1; ++j )
					for ( int k = 0; k < data1.Depth; ++k )

						temp.Volume[i, j, k]
                            =
                            data1.Volume[i, j, k]
                            -
                            data2.Volume[i, j, k];

			return temp;
		}

		public static VolumeData operator +( VolumeData data1, VolumeData data2 )
		{
			VolumeData temp = new VolumeData( data1.Width, data1.Height, data1.Depth );

			for ( int i = 0; i < data1.Width; ++i )
				for ( int j = 0; j < data1.Height - 1; ++j )
					for ( int k = 0; k < data1.Depth; ++k )

						temp.Volume[i, j, k]
                            =
                            data1.Volume[i, j, k]
                            +
                            data2.Volume[i, j, k];

			return temp;
		}

		#endregion
	}
}