using System.IO;

namespace EvolveRTS.Database
{
	public interface IDatabaseSerializer
	{
		Stream GetStream();
	}
}