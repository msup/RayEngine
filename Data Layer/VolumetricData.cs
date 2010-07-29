using System;
using System.IO;

namespace WpfOpenTK
{
	public class VolumetricData
	{
		uint[, ,] volumeData = null;

		public int Width
		{
			get;
			set;
		}

		public int Height
		{
			get;
			set;
		}

		public int Depth
		{
			get;
			set;
		}

		public uint[, ,] RawVolumeData
		{
			get
			{
				return volumeData;
			}
		}

		public VolumetricData( int width, int height, int depth )
		{
			Width = width;
			Height = height;
			Depth = depth;

			volumeData = new uint[Width, Height, Depth];
		}

		public VolumetricData( string fileName )
		{
			Width  = 256;
			Height = 128;
			Depth  = 256;

			volumeData = new uint[Width, Height, Depth];

			// FIXME: dimension check
			// TODO: check the format - raw

			FileStream inStream = new FileStream( fileName, FileMode.Open );

			BinaryReader binReader = new BinaryReader( inStream );

			byte[] rawData = new byte[Width * Height];

			uint index = 0;
			while ( ( binReader.Read( rawData, 0, Width * Height ) != 0 ) && index < Depth )
			{
				CopyToArray( rawData, ref volumeData, Width, Height, index );
				index++;
			}

			int a = 0;
		}

		void CopyToArray( byte[] source, ref uint[, ,] destination, int width, int height, uint depthIndex )
		{
			// changed for, to height, width reversed
			// changed destination j, i
			// changed source height
			for ( int i = 0; i < height; i++ )
				for ( int j = 0; j < width; j++ )
				{
					// TODO: fixme 256*256
					destination[j, i, depthIndex] = (uint) source[i + j * height] * 256 * 256;
				}
		}

		public void GenerateRandom()
		{
			Random rand = new Random();

			for ( int i = 0; i < Width; i++ )
				for ( int j = 0; j < Height; j++ )
					for ( int k = 0; k < Depth; k++ )
						volumeData[i, j, k] = (uint) ( rand.Next() );
		}
	}
}