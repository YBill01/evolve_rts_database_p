using System.IO;

namespace EvolveRTS.Database
{
	public class DatabaseFileSerializer : IDatabaseSerializer
	{
		public string path { get; private set; }
		public string name { get; private set; }

		public DatabaseFileSerializer(string path, string name)
		{
			this.path = path;
			this.name = name;
		}

		public Stream GetStream()
		{
			return new FileStream(Path.Combine("../", path, name), FileMode.Open);
		}
	}
}