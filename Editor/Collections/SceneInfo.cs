/*created by YB v0.0.0.1 : 2024-05-18 13:26*/

using Sirenix.OdinInspector;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace EvolveRTS.Editor.Database.Collection
{
	[Serializable]
	public class SceneInfo : Element
	{
		//__FIELDS__ begin
		//[PreviewField()]
		//public AssetReferenceTexture2D sceneIcon;
		//public Sprite sceneIcon;
		public AssetReferenceT<SceneAsset> scene;
		public LoadSceneMode loadMode;
		//__FIELDS__ end

		internal override void ApplyDefault()
		{
			//__MAIN_FIELDS_SET_DEFAULT__ begin
			base.ApplyDefault();
			//__MAIN_FIELDS_SET_DEFAULT__ end

			//__FIELDS_SET_DEFAULT__ begin
			//__FIELDS_SET_DEFAULT__ end
		}

		internal override void Write(BinaryWriter binary)
		{
			//__WRITE_MAIN_FIELDS__ begin
			base.Write(binary);
			//__WRITE_MAIN_FIELDS__ end

			//__WRITE_FIELDS__ begin
			binary.Write(scene.AssetGUID);
			binary.Write((int)loadMode);
			//__WRITE_FIELDS__ end
		}
	}
}