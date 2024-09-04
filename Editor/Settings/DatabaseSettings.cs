using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EvolveRTS.Editor.Database
{
	public class DatabaseSettings : ScriptableObject
	{
		private static DatabaseSettings _instance = null;
		public static DatabaseSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = AssetDatabase.LoadAssetAtPath<DatabaseSettings>(@$"{DatabaseConfig.ROOT_PATH}/Editor/Settings/DatabaseSettings.asset");
				}
				return _instance;
			}
		}

		[BoxGroup("Save", true, false, 0f)]
		[FolderPath(ParentFolder = "../")]
		public string savePath;
		[BoxGroup("Save", true, false, 0f)]
		public string saveName;

		[BoxGroup("Default values", true, false, 0f)]
		[AssetsOnly]
		public Sprite defaultCollectionIcon;
		[BoxGroup("Default values", true, false, 0f)]
		[AssetsOnly]
		public Sprite defaultElementIcon;



	}
}