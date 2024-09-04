using System;

namespace EvolveRTS.Database
{
	[Serializable]
	public class DatabaseElement<T> where T : Element
	{
		public Type type => typeof(T);

		public int id;

		public T value => Db.collections.Get<T>(id);
	}
}