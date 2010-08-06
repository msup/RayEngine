using Plugins;

namespace Data
{
	public class DataClassifier : IPlugin
	{
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
		}
	}
}