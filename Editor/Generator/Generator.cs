using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace EvolveRTS.Editor.Database
{
	public static class Generator
	{
		public static void CreateCollection(string newCollectionName, string newContainerPath)
		{
			string collectionName = Path.GetFileNameWithoutExtension(newCollectionName);
			collectionName = char.ToUpper(collectionName[0]) + collectionName.Substring(1);
			string collectionsName = Path.GetFileNameWithoutExtension(newContainerPath);
			collectionsName = char.ToUpper(collectionsName[0]) + collectionsName.Substring(1);

			if (collectionName == collectionsName)
			{
				Debug.LogError($"Name '{collectionName}' and container name '{collectionsName}' should be different!");

				return;
			}

			string pathTT = $@"{DatabaseConfig.ROOT_PATH}/Editor/Generator/Templates";

			string pathRuntimeCS = $@"{DatabaseConfig.ROOT_PATH}/Runtime/Collections";
			string pathEditorCS = $@"{DatabaseConfig.ROOT_PATH}/Editor/Collections";
			string pathEditorSO = $@"{DatabaseConfig.ROOT_PATH}/Editor/SOCollections/SO";

			string pathEditorSOSubdir = Path.GetDirectoryName(newContainerPath);
			
			if (File.Exists(Path.Combine(pathRuntimeCS, collectionName + ".cs")))
			{
				Debug.LogError($"File '{collectionName}' is already exists!");
				return;
			}

			if (Directory.GetFiles(pathEditorSO, collectionsName + ".cs", SearchOption.AllDirectories).Length > 0)
			{
				Debug.LogError($"File '{collectionsName}' is already exists!");
				return;
			}

			string elementRuntimeTemplate = File.ReadAllText(Path.Combine(pathTT, "ElementRuntimeTemplate.tt"));
			string elementEditorTemplate = File.ReadAllText(Path.Combine(pathTT, "ElementEditorTemplate.tt"));
			string collectionEditorSOTemplate = File.ReadAllText(Path.Combine(pathTT, "CollectionEditorSOTemplate.tt"));

			string createdDate = $"/*created by {Environment.UserName} v{Application.version} : {DateTime.UtcNow:yyyy-MM-dd HH:mm}*/";

			File.WriteAllText(
				Path.Combine(pathRuntimeCS, collectionName + ".cs"),
				elementRuntimeTemplate
					.Replace("__CREATED_DATE__", createdDate)
					.Replace("__ROOT_NAMESPACE__", DatabaseConfig.ROOT_NAMESPACE)
					.Replace("__ELEMENT_NAME__", collectionName)
			);

			File.WriteAllText(
				Path.Combine(pathEditorCS, collectionName + ".cs"),
				elementEditorTemplate
					.Replace("__CREATED_DATE__", createdDate)
					.Replace("__ROOT_NAMESPACE__", DatabaseConfig.ROOT_NAMESPACE)
					.Replace("__ELEMENT_NAME__", collectionName)
			);

			if (!Directory.Exists(Path.Combine(pathEditorSO, pathEditorSOSubdir)))
			{
				Directory.CreateDirectory(Path.Combine(pathEditorSO, pathEditorSOSubdir));
			}

			File.WriteAllText(
				Path.Combine(pathEditorSO, pathEditorSOSubdir, collectionsName + ".cs"),
				collectionEditorSOTemplate
					.Replace("__CREATED_DATE__", createdDate)
					.Replace("__ROOT_NAMESPACE__", DatabaseConfig.ROOT_NAMESPACE)
					.Replace("__COLLECTION_NAME__", collectionsName)
					.Replace("__ELEMENT_NAME__", collectionName)
			);

			AssetDatabase.ImportAsset(Path.Combine(pathRuntimeCS, collectionName + ".cs"));
			AssetDatabase.ImportAsset(Path.Combine(pathEditorCS, collectionName + ".cs"));
			AssetDatabase.ImportAsset(Path.Combine(pathEditorSO, pathEditorSOSubdir, collectionsName + ".cs"));
			AssetDatabase.Refresh();

			EditorUtility.DisplayDialog("Datatabase", $"Collection '{collectionName}' is created!", "Ok");
			Debug.Log($"Collection '{collectionName}' is created!");
		}

		public static void GenerateAssets()
		{
			string pathSO = $@"{DatabaseConfig.ROOT_PATH}/Editor/SOCollections/SO";
			string pathSOAssets = $@"{DatabaseConfig.ROOT_PATH}/Editor/SOCollections/SOAssets";

			string[] pathSOList = Directory.GetFiles(pathSO, "*.cs", SearchOption.AllDirectories);

			Dictionary<string, string> noExistingAssets = new Dictionary<string, string>();

			foreach (string path in pathSOList)
			{
				string relativePathSO = path.Substring(pathSO.Length + 1);
				string directoryNameSO = Path.GetDirectoryName(relativePathSO);
				string fileNameSO = Path.GetFileNameWithoutExtension(relativePathSO);

				if (!File.Exists(Path.Combine(pathSOAssets, directoryNameSO, fileNameSO + ".asset")))
				{
					if (!Directory.Exists(Path.Combine(pathSOAssets, directoryNameSO)))
					{
						Directory.CreateDirectory(Path.Combine(pathSOAssets, directoryNameSO));
					}

					noExistingAssets.Add(fileNameSO, directoryNameSO);
				}
			}

			if (noExistingAssets.Count > 0)
			{
				int lastId = -1;

				string[] pathSOAssetsList = Directory.GetFiles(pathSOAssets, "*.asset", SearchOption.AllDirectories);
				foreach (string path in pathSOAssetsList)
				{
					string fileText = File.ReadAllText(path);

					int id = int.Parse(Regex.Match(fileText, @" id: (.*?)\n").Groups[1].Value);

					if (id > lastId)
					{
						lastId = id;
					}
				}

				foreach (var asset in noExistingAssets)
				{
					Type type = Type.GetType($"{DatabaseConfig.ROOT_NAMESPACE}.Editor.Database.Collection.{asset.Key}");

					ScriptableObject so = ScriptableObject.CreateInstance(type);

					type.GetField("id").SetValue(so, ++lastId);
					type.GetField("icon").SetValue(so, DatabaseSettings.Instance.defaultElementIcon);

					AssetDatabase.CreateAsset(so, $"{pathSOAssets}/{noExistingAssets[type.Name]}/{type.Name}.asset");
				}

				ReGenerateCollections();
				
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				EditorUtility.DisplayDialog("Datatabase", $"Database is generated!", "Ok");
				Debug.Log("Database is generated!");
			}
		}

		public static void ReGenerateCollections()
		{
			string pathTT = $@"{DatabaseConfig.ROOT_PATH}/Editor/Generator/Templates";

			string fileCollectionsRuntime = $@"{DatabaseConfig.ROOT_PATH}/Runtime/Collections.cs";
			string fileCollectionsEditor = $@"{DatabaseConfig.ROOT_PATH}/Editor/Collections.cs";
			string pathCollectionsEditorSO = $@"{DatabaseConfig.ROOT_PATH}/Editor/SOCollections/SO";
			string pathCollectionsEditorSOAssets = $@"{DatabaseConfig.ROOT_PATH}/Editor/SOCollections/SOAssets";

			string collectionsRuntimeTemplate = File.ReadAllText(Path.Combine(pathTT, "CollectionsRuntimeTemplate.tt"));
			string collectionsEditorTemplate = File.ReadAllText(Path.Combine(pathTT, "CollectionsEditorTemplate.tt"));

			string generatedDate = $"/*generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm}*/";

			IEnumerable<string> pathEditorSOList = Directory.GetFiles(pathCollectionsEditorSO, "*.cs", SearchOption.AllDirectories)
				.OrderBy(x => x);

			Match matchEditorAddTemplate = Regex.Match(collectionsEditorTemplate, @"__T_ADD__<(.*?)>__T_ADD__");
			collectionsEditorTemplate = collectionsEditorTemplate.Replace(matchEditorAddTemplate.Groups[0].Value, "");
			Match matchEditorWriteTemplate = Regex.Match(collectionsEditorTemplate, @"__T_WRITE__<(.*?)>__T_WRITE__");
			collectionsEditorTemplate = collectionsEditorTemplate.Replace(matchEditorWriteTemplate.Groups[0].Value, "");
			StringBuilder collectionsEditorAddInsert = new StringBuilder();
			StringBuilder collectionsEditorWriteInsert = new StringBuilder();

			Match matchRuntimeAddTemplate = Regex.Match(collectionsRuntimeTemplate, @"__T_ADD__<(.*?)>__T_ADD__");
			collectionsRuntimeTemplate = collectionsRuntimeTemplate.Replace(matchRuntimeAddTemplate.Groups[0].Value, "");
			Match matchRuntimeReadTemplate = Regex.Match(collectionsRuntimeTemplate, @"__T_READ__<(.*?)>__T_READ__", RegexOptions.Singleline);
			collectionsRuntimeTemplate = collectionsRuntimeTemplate.Replace(matchRuntimeReadTemplate.Groups[0].Value, "");
			StringBuilder collectionsRuntimeAddInsert = new StringBuilder();
			StringBuilder collectionsRuntimeReadInsert = new StringBuilder();

			foreach (string path in pathEditorSOList)
			{
				string fileText = File.ReadAllText(path);

				string collectionsName = Regex.Match(fileText, @"class (.*?) : ").Groups[1].Value;
				string elementName = Regex.Match(fileText, @"<(.*?)>").Groups[1].Value;

				collectionsEditorAddInsert.AppendLine("\t\t\t" + matchEditorAddTemplate.Groups[1].Value
					.Replace("__ELEMENT_NAME__", elementName)
					.Replace("__COLLECTION_NAME__", collectionsName)
					.Replace("__COLLECTION_PATH__", Path.Combine(pathCollectionsEditorSOAssets, Path.GetDirectoryName(path.Substring(pathCollectionsEditorSO.Length + 1)), $"{collectionsName}.asset"))
				);
				collectionsEditorWriteInsert.AppendLine("\t\t\t" + matchEditorWriteTemplate.Groups[1].Value
					.Replace("__COLLECTION_NAME__", collectionsName)
					.Replace("__ELEMENT_NAME__", elementName)
				);

				collectionsRuntimeAddInsert.AppendLine("\t\t\t" + matchRuntimeAddTemplate.Groups[1].Value
					.Replace("__ELEMENT_NAME__", elementName)
				);
				collectionsRuntimeReadInsert.AppendLine("\t\t\t" + matchRuntimeReadTemplate.Groups[1].Value
					.Replace("__ELEMENT_NAME__", elementName)
				);
			}

			File.WriteAllText(
				fileCollectionsEditor,
				collectionsEditorTemplate
					.Replace("__GENERATED_DATE__", generatedDate)
					.Replace("__ROOT_NAMESPACE__", DatabaseConfig.ROOT_NAMESPACE)
					.Replace("__COLLECTIONS_ADD__", collectionsEditorAddInsert.ToString())
					.Replace("__COLLECTIONS_WRITE__", collectionsEditorWriteInsert.ToString())
			);

			File.WriteAllText(
				fileCollectionsRuntime,
				collectionsRuntimeTemplate
					.Replace("__GENERATED_DATE__", generatedDate)
					.Replace("__ROOT_NAMESPACE__", DatabaseConfig.ROOT_NAMESPACE)
					.Replace("__COLLECTIONS_ADD__", collectionsRuntimeAddInsert.ToString())
					.Replace("__COLLECTIONS_READ__", collectionsRuntimeReadInsert.ToString())
			);
		}
	}
}