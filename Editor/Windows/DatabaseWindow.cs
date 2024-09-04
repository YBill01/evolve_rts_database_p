using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace EvolveRTS.Editor.Database
{
	public class DatabaseWindow : OdinMenuEditorWindow
	{
		internal static DatabaseWindow Instance = null;

		static readonly Vector2 editorWindowMinimumSize = new Vector2(640, 480);

		//private AppInitSettings.ConnectionType _connectionType = AppInitSettings.ConnectionType.Local;

		public static void OpenWindow()
		{
			GetWindow<DatabaseWindow>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			Instance = this;

			titleContent = new GUIContent("Database", EditorGUIUtility.IconContent("CloudConnect"/*"PreMatCylinder"*/).image/* EditorIcons.GridImageTextList.Raw*/);

			minSize = editorWindowMinimumSize;
			wantsMouseMove = true;

			MenuWidth = 200;
		}
		protected override void OnDisable()
		{
			base.OnDisable();

			Instance = null;
		}
		/*protected override void OnDestroy()
		{
			base.OnDestroy();


		}*/

		protected override void OnImGUI()
		{
			Rect controlRect;

			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(1)))
				{
					//GUILayout.Label($"Generate", EditorStyles.boldLabel);

					using (new GUILayout.HorizontalScope())
					{
						//GUI.color = new Color(1.75f, 1.75f, 1.0f, 1.0f);
						controlRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(64), GUILayout.Height(24));
						//controlRect.y += 7;
						if (SirenixEditorGUI.SDFIconButton(controlRect, new GUIContent("New", "Create new empty collection"), SdfIconType.PlusCircle))
						{
							string dirPath = default;

							foreach (OdinMenuItem menuItem in MenuTree.Selection)
							{
								OdinMenuItem item = menuItem;
								while (item != null)
								{
									if (item.Value is UnityEditor.DefaultAsset)
									{
										dirPath = $"{item.SearchString}/{dirPath}";
									}

									item = item.Parent;
								}
							}

							CreateCollectionPopupWindow createWindow = CreateCollectionPopupWindow.Show((collectionName, collectionContainerPath) =>
							{
								Generator.CreateCollection(collectionName, collectionContainerPath);
							});
							createWindow.SetFields(default, dirPath);
						}
						/*if (GUILayout.Button(new GUIContent(EditorIcons.Plus.Raw, "Create empty collection"), GUILayout.Width(24), GUILayout.Height(24)))
						{
							CreateCollectionPopupWindow.Show((createName) => {
								Generator.CreateCollection(createName);
							});
						}*/
						//GUI.color = Color.white;

						controlRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(92), GUILayout.Height(24));
						if (SirenixEditorGUI.SDFIconButton(controlRect, new GUIContent("Generate", "Generate collections assets"), SdfIconType.Box, IconAlignment.LeftEdge))
						{
							Generator.GenerateAssets();
						}

						var selectedValue = MenuTree?.Selection.SelectedValue;
						GUI.enabled = selectedValue is UnityEngine.ScriptableObject;
						controlRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(24), GUILayout.Height(24));
						if (SirenixEditorGUI.SDFIconButton(controlRect, new GUIContent("Edit selected collection scripts"), SdfIconType.Pencil, IconAlignment.LeftEdge))
						{
							Type type = selectedValue.GetType();
							if (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(Collection<>))
							{
								Type genType = type.BaseType.GetGenericArguments()[0];

								string nameFile = genType.Name;

								string[] guids = AssetDatabase.FindAssets(string.Format("{0} t:script", nameFile), new string[] { DatabaseConfig.ROOT_PATH });
								List<string> pathList = new List<string>();

								foreach (string guid in guids)
								{
									string path = AssetDatabase.GUIDToAssetPath(guid);
									if (Path.GetFileNameWithoutExtension(path) == nameFile)
									{
										pathList.Add(AssetDatabase.GUIDToAssetPath(guid));
									}
								}

								foreach (string path in pathList)
								{
									AssetDatabase.OpenAsset((MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)));
								}
							}
						}
						GUI.enabled = true;

						//GUI.color = new Color(1.0f, 1.75f, 1.25f, 1.0f);
						/*if (GUILayout.Button("Generate Assets", GUILayout.Width(120), GUILayout.Height(24)))
						{
							Generator.GenerateAssets();
						}*/
						//GUI.color = Color.white;
					}
				}


				using (new GUILayout.HorizontalScope())
				{
					using (new GUILayout.HorizontalScope(GUI.skin.box))
					{
						using (new GUILayout.HorizontalScope())
						{
							GUIHelper.PushColor(new Color(0.75f, 0.75f, 0.75f, 1.0f));
							GUILayout.Label("Git branch:", GUILayout.Height(24), GUILayout.ExpandWidth(true));
							GUIHelper.PopColor();

							GUIHelper.PushColor(new Color(0.25f, 1.0f, 0.0f, 1.0f));
							//EditorGUILayout.LabelField(GitUtils.GetBranchName(), GUILayout.Height(24), GUILayout.ExpandWidth(true), EditorStyles.boldLabel);

							GUILayout.Label(GitUtils.GetBranchName(), EditorStyles.boldLabel, GUILayout.Height(24), GUILayout.ExpandWidth(true));
							GUIHelper.PopColor();
							//SirenixEditorGUI.Title
							//GUIHelper.PushColor(new Color(1.0f, 1.25f, 1.75f, 1.0f));
							controlRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(24), GUILayout.Height(24));
							if (SirenixEditorGUI.SDFIconButton(controlRect, new GUIContent("Update current branch name"), SdfIconType.ArrowRepeat, IconAlignment.LeftEdge, style: SirenixGUIStyles.IconButton))
							{
								GitUtils.GetBranchName(true);
							}
							//GUIHelper.PopColor();
						}
					}

					using (new GUILayout.HorizontalScope(GUI.skin.box, GUILayout.Width(1)))
					{
						GUIHelper.PushColor(new Color(1.0f, 1.25f, 1.75f, 1.0f));
						controlRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(84), GUILayout.Height(24));
						if (SirenixEditorGUI.SDFIconButton(controlRect, new GUIContent("Save", "Write binary databse"), SdfIconType.Save, IconAlignment.LeftEdge))
						{
							Db.Write(/*_connectionType*/);
						}
						GUIHelper.PopColor();

						/*using (new GUILayout.HorizontalScope())
						{
							//GUILayout.Label("Connection Type:", GUILayout.Height(24), GUILayout.ExpandWidth(false));

							//_connectionType = (AppInitSettings.ConnectionType)EditorGUILayout.EnumPopup(_connectionType, GUI.skin.button, GUILayout.Height(24));

							
						}*/
					}
				}




				/*using (new GUILayout.HorizontalScope(GUI.skin.box))
				{
					


					using (new GUILayout.HorizontalScope())
					{
						GUILayout.Label("Git branch:", GUILayout.Height(24), GUILayout.ExpandWidth(false));
						GUILayout.Label(GitUtils.GetBranchName(), GUILayout.Height(24), GUILayout.ExpandWidth(false));

						//GUIHelper.PushColor(new Color(1.0f, 1.25f, 1.75f, 1.0f));
						controlRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(24), GUILayout.Height(24));
						if (SirenixEditorGUI.SDFIconButton(controlRect, new GUIContent("Update current branch name"), SdfIconType.ArrowRepeat, IconAlignment.LeftEdge, style: SirenixGUIStyles.IconButton))
						{
							GitUtils.GetBranchName(true);
						}
						//GUIHelper.PopColor();
					}





					//GUILayout.Label($"Save database", EditorStyles.boldLabel);

					using (new GUILayout.HorizontalScope())
					{
						//GUILayout.Label("Connection Type:", GUILayout.Height(24), GUILayout.ExpandWidth(false));

						//_connectionType = (AppInitSettings.ConnectionType)EditorGUILayout.EnumPopup(_connectionType, GUI.skin.button, GUILayout.Height(24));

						GUIHelper.PushColor(new Color(1.0f, 1.25f, 1.75f, 1.0f));
						controlRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(84), GUILayout.Height(24));
						if (SirenixEditorGUI.SDFIconButton(controlRect, new GUIContent("Save", "Write binary databse"), SdfIconType.Save, IconAlignment.LeftEdge))
						{
							Database.Instance.Write(_connectionType);
						}
						GUIHelper.PopColor();
					}
				}*/

				
				using (new GUILayout.VerticalScope(/*GUI.skin.box,*/ GUILayout.Width(34), GUILayout.Height(38)))
				{
					controlRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(24), GUILayout.Height(28));
					controlRect.y += 5;
					if (SirenixEditorGUI.SDFIconButton(controlRect, new GUIContent("Settings editor database"), SdfIconType.GearFill, style: SirenixGUIStyles.IconButton))
					{
						InspectObject(DatabaseSettings.Instance);
					}

					/*controlRect = EditorGUILayout.GetControlRect(false, GUILayout.Width(24), GUILayout.Height(24));
					controlRect.y += 7;
					if (SirenixEditorGUI.SDFIconButton(controlRect, new GUIContent("Settings editor database"), SdfIconType.GearFill))
					{
						InspectObject(DatabaseSettings.Instance);
					}*/
					//SirenixEditorGUI.DrawSolidRect(rect, Color.red);
					

					//GUILayout.FlexibleSpace();
					//EditorGUI.indentLevel++;
					/*GUILayout.Space(7);
					if (GUILayout.Button(new GUIContent(EditorIcons.SettingsCog.Raw, "Settings editor database"), GUILayout.Width(24), GUILayout.Height(24)))
					{
						InspectObject(DatabaseSettings.Instance);
					}*/
				}

				
			}

			MenuWidth = MenuWidth < 200 ? 200 : MenuWidth > 400 ? 400 : MenuWidth;

			/*using (new GUILayout.HorizontalScope(GUI.skin.box, GUILayout.Width(MenuWidth - 4)))
			{
				if (GUILayout.Button("", GUILayout.Width(16), GUILayout.Height(16)))
				{

				}

				GUILayout.Label($"Menu tree control...");
			}*/

			base.OnImGUI();
		}

		public void UpdateMenuTree()
		{
			UpdateMenuTree(MenuTree);
		}

		protected override OdinMenuTree BuildMenuTree()
		{
			OdinMenuTree menuTree = new OdinMenuTree();
			
			menuTree.Selection.SupportsMultiSelect = false;
			menuTree.Config.DrawSearchToolbar = true;
			menuTree.DefaultMenuStyle = new OdinMenuStyle()
			{
				Height = 28,
				Offset = 20.00f,
				IndentAmount = 24.00f,
				IconSize = 26.00f,
				IconOffset = 0.00f,
				NotSelectedIconAlpha = 0.85f,
				IconPadding = 0.00f,
				TriangleSize = 16.00f,
				TrianglePadding = -2.00f,
				AlignTriangleLeft = true,
				Borders = false,
				BorderPadding = 4.00f,
				BorderAlpha = 0.25f,
				SelectedColorDarkSkin = new Color(0.243f, 0.373f, 0.588f, 1.000f),
				SelectedColorLightSkin = new Color(0.243f, 0.490f, 0.900f, 1.000f)
			};
			
			string pathSOAssets = @$"{DatabaseConfig.ROOT_PATH}/Editor/SOCollections/SOAssets/";
			
			IEnumerable<string> pathSOAssetsList = AssetDatabase.GetAllAssetPaths()
				.Where(x => x.StartsWith(pathSOAssets))
				.OrderBy(x => x);

			foreach (var path in pathSOAssetsList)
			{
				menuTree.AddAssetAtPath(path.Substring(pathSOAssets.Length), path);
			}

			UpdateMenuTree(menuTree);

			return menuTree;
		}

		protected void UpdateMenuTree(OdinMenuTree menuTree)
		{
			IEnumerable<OdinMenuItem> odinMenuItems = menuTree.EnumerateTree();
			foreach (OdinMenuItem menuItem in odinMenuItems)
			{
				if (menuItem.Value is UnityEngine.Object value)
				{
					Type valueType = value.GetType();

					string name = Regex.Replace(menuItem.Name, "^(_|[0-9]){1,}", "");

					Texture icon = GUIHelper.GetAssetThumbnail(value, valueType, false);
					//SdfIconType sdfIcon = SdfIconType.FolderFill;

					if (valueType.BaseType.IsGenericType && valueType.BaseType.GetGenericTypeDefinition() == typeof(Collection<>))
					{
						string id = valueType.GetField("id").GetValue(value).ToString();
						string count = (valueType.GetField("elements").GetValue(value) as IEnumerable<object>)?.Count().ToString();
						//sdfIcon = SdfIconType.FileEarmark;
						//icon = value.GetType().GetField("icon").GetValue(value) as Sprite;
						name = Path.GetFileNameWithoutExtension(valueType.Name) + $" [{id}]({count})";// + (count != null ? $" ({count})" : "");// + (id != null ? $" [{id}]" : "");;
					}

					menuItem.Name = name;
					menuItem.Icon = icon;
					//menuItem.SdfIcon = sdfIcon;
					//menuItem.Icon = icon ? icon.texture : null;
				}
			}
		}
	}
}