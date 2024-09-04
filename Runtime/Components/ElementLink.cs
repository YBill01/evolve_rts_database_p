using System;

namespace EvolveRTS.Database
{
	public struct ElementLink<T> where T : Element
	{
		public Type type => typeof(T);

		public int id;

		public T value => _collections.Get<T>(id);

		private Collections _collections;

		internal ElementLink(Collections collections) : this()
		{
			_collections = collections;
		}
	}
}