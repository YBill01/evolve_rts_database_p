using System.IO;
using UnityEngine;

namespace EvolveRTS.Database
{
	public class DatabaseResourcesSerializer : IDatabaseSerializer
	{
		public string path { get; private set; }
		public string name { get; private set; }

		public DatabaseResourcesSerializer(string path, string name)
		{
			this.path = path;
			this.name = name;
		}

		public Stream GetStream()
		{
			TextAsset asset = Resources.Load<TextAsset>(Path.Combine(path, Path.GetFileNameWithoutExtension(name)));
			return new MemoryStream(asset.bytes);
		}
	}
}