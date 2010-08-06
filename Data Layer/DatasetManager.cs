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