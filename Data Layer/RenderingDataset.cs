using System.Collections.Generic;
using WpfOpenTK;

namespace Data
{
	public class RenderingDataset : IRenderingDataset
	{
		private List<LookupTable> lstLookUpTables = null;
		private List<AlgorithmOptions> lstOptions = null;

		public VolumetricData Data3D
		{
			get;
			set;
		}

		public RenderingDataset()
		{
			//Data3D = new VolumetricData( 200, 200, 100 );

			//Data3D = new VolumetricData("skull.raw");
			//Data3D = new VolumetricData("Carp8bit.raw");
			Data3D = new VolumetricData( "../../Data/VisMale.raw" );

			//Data3D.GenerateRandom();
		}

		public void Load()
		{
		}
	}
}