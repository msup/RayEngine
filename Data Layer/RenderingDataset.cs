using System;
using System.Collections.Generic;
using WpfOpenTK;

namespace Data
{
	public class RenderingDataset : IRenderingDataset
	{
		#region private fields

		private List<AlgorithmOptions> lstOptions = null;
		private List<LookupTable> lstLookUpTables = null;

		#endregion

		#region properties

		public List<LookupTable> LookUpTables
		{
			get
			{
				return lstLookUpTables;
			}
		}

		public VolumetricData Data3D
		{
			get;
			set;
		}

		#endregion

		public RenderingDataset()
		{
			Data3D = new VolumetricData();

			LookupTable lut = new LookupTable();
            //lut.Load();
            //lut.Load( "lut1.png" );
            //lut.Load("lut_4096_8bit.png");
            lut.Load("lut_1024_8bit_metaballs4.png");

            var lut_z_distance = new LookupTable();
            lut_z_distance.Load( "lut_zdistance.png" );

			if ( lstLookUpTables == null )
				lstLookUpTables = new List<LookupTable>();

			lstLookUpTables.Add( lut );
            lstLookUpTables.Add( lut_z_distance );

		}

		public void Load()
		{
			throw new NotImplementedException();
		}
	}
}