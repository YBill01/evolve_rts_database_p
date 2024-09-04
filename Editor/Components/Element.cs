using System.IO;
using UnityEditor;
using UnityEngine;

namespace EvolveRTS.Editor.Database
{
	public abstract class Element
	{
		public int id;
		public bool enabled;
		public int order;

		public Sprite icon;

		public string name;
		[TextArea]
		public string description;

		internal virtual void ApplyDefault()
		{
			id = -1;
			enabled = true;
			order = -1;
			icon = DatabaseSettings.Instance.defaultElementIcon;
			name = default;
			description = "Some description...";
		}

		internal virtual void Write(BinaryWriter binary)
		{
			binary.Write(id);
			binary.Write(enabled);
			binary.Write(order);
			binary.Write(icon.name);
			binary.Write(AssetDatabase.GetAssetPath(icon));
			binary.Write(name);
			binary.Write(description);
		}
	}
}