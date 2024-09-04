using System;

namespace EvolveRTS.Database
{
	public struct AssetElement<T> where T : UnityEngine.Object
	{
		public Type type => typeof(T);

		public string name;
		public string path;
		public string assetGUID;
	}
}