using System;

namespace EvolveRTS.Editor.Database
{
	[Serializable]
	public class ElementLink<T> where T : Element
	{
		public int id = -1;
	}
}