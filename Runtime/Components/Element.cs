using System.IO;
using UnityEngine;

namespace EvolveRTS.Database
{
	public abstract class Element
	{
		public int id;
		public bool enabled;
		public int order;

		public AssetElement<Sprite> icon;

		public string name;
		public string description;

		internal static void Read(BinaryReader binary, Element element)
		{
			element.id = binary.ReadInt32();
			element.enabled = binary.ReadBoolean();
			element.order = binary.ReadInt32();
			element.icon = new AssetElement<Sprite>()
			{
				name = binary.ReadString(),
				path = binary.ReadString()
			};
			element.name = binary.ReadString();
			element.description = binary.ReadString();
		}
	}
}