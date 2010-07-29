using System;
using System.Collections.Generic;
using System.Text;
using Data;


namespace Data
{
	public class DatasetManager
	{
		public RenderingDataset RenderingDataset
		{
			get;
			set;
		}

		public DatasetManager()
		{
			RenderingDataset = new RenderingDataset();
		}

	}
}