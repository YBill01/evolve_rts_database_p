/*created by YB v0.0.0.1 : 2024-05-18 13:26*/

using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EvolveRTS.Database.Collection
{
	public class SceneInfo : Element
	{
		//__FIELDS__ begin
		public AssetElement<Object> scene;
		//public string sceneAssetGUID;
		public LoadSceneMode loadMode;
		//__FIELDS__ end

		internal static SceneInfo Read(BinaryReader binary, Collections collections)
		{
			//__READ_MAIN_FIELDS__ begin
			SceneInfo result = new SceneInfo();
			Read(binary, result);
			//__READ_MAIN_FIELDS__ end

			//__READ_FIELDS__ begin
			//result.sceneAssetGUID = binary.ReadString();
			result.scene = new AssetElement<Object>()
			{
				assetGUID = binary.ReadString()
			};
			result.loadMode = (LoadSceneMode)binary.ReadInt32();
			//__READ_FIELDS__ end

			return result;
		}
	}
}