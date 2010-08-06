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
			lut.Load();

			if ( lstLookUpTables == null )
				lstLookUpTables = new List<LookupTable>();

			lstLookUpTables.Add( lut );
		}

		public void Load()
		{
			throw new NotImplementedException();
		}
	}
}