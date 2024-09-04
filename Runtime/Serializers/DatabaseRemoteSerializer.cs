using System.IO;
using System.Net;

namespace EvolveRTS.Database
{
	public class DatabaseRemoteSerializer : IDatabaseSerializer
	{
		public string damain { get; private set; }
		public string name { get; private set; }

		public DatabaseRemoteSerializer(string damain, string name)
		{
			this.damain = damain;
			this.name = name;
		}

		public Stream GetStream()
		{
			string url = $"{damain}/{name}";

			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

			return httpWebResponse.GetResponseStream();
		}
	}
}