using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

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
			// Get the volume data file path
			System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
			string volumeDataPath = (string) configurationAppSettings.GetValue( "VolumeDataPath", typeof( string ) );

			#region Read volume dimensions from accompanying xml volume data

			var volumeDataDescriptionFile = XDocument.Load( volumeDataPath+".xml" );

			Width  = Int32.Parse(
					 (
					 from c in volumeDataDescriptionFile.Descendants( "Dimensions" )
					 select c.Element( "Width" ).Value
					 ).Single()
			);

			Height = Int32.Parse(
					 (
					 from c in volumeDataDescriptionFile.Descendants( "Dimensions" )
					 select c.Element( "Height" ).Value
					 ).Single()
			);

			Depth = Int32.Parse(
					 (
					 from c in volumeDataDescriptionFile.Descendants( "Dimensions" )
					 select c.Element( "Depth" ).Value
					 ).Single()
			);

			#endregion Read volume dimensions from accompanying xml volume data

			volumeData = new uint[Width, Height, Depth];

			// FIXME: dimension check
			// TODO: check the format - raw

			FileStream inStream = new FileStream( volumeDataPath, FileMode.Open );

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
					destination[j, i, depthIndex] = (uint) source[i + j * height] << 16;
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