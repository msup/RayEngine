namespace Data
{
	public class LookupTable
	{
		uint[] LUTdata1D = null;
		uint[,] LUTdata2D = null;

		public uint[] LUTData1D
		{
			get
			{
				return LUTdata1D;
			}
		}

		public bool Load()
		{
			int width = 256;

			LUTdata1D = new uint[256];

			//for ( int i = 150; i < 160; ++i )
			//    LUTdata1D[i] = (uint) ( ( ( i-50 ) << 24 ) | ( ( i-0 ) << 16 ) );

			for ( int i = 190; i < width; ++i )
				LUTdata1D[i] = (uint) ( ( ( i-120 ) << 24 ) | ( ( i-0 ) << 0 ) );

			for ( int i = 200; i < width; ++i )
				LUTdata1D[i] = (uint) ( ( ( i-120 ) << 24 ) | ( i << 08 ) );

			return true;
		}
	}
}