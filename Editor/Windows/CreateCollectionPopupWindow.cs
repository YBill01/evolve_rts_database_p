using Sirenix.Utilities.Editor;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EvolveRTS.Editor.Database
{
	public class CreateCollectionPopupWindow : EditorWindow
	{
		private Action<string, string> onCreate;

		private string _inputNameCollection;
		private string _inputNameCollections;

		private bool _isFocused;

		private void OnEnable()
		{
			titleContent = new GUIContent("Create new empty collection", EditorIcons.Plus.Raw);

			minSize = new Vector2(400, 126);
			maxSize = minSize;
		}

		private void OnGUI()
		{
			bool closed = false;

			Event e = Event.current;
			if (e.type == EventType.KeyDown)
			{
				switch (e.keyCode)
				{
					case KeyCode.Escape:
						closed = true;

						e.Use();
						break;
					case KeyCode.Return:
					case KeyCode.KeypadEnter:

						if (IsNameValid())
						{
							closed = true;

							onCreate?.Invoke(_inputNameCollection, _inputNameCollections);
						}

						e.Use();
						break;
				}
			}

			using (new GUILayout.VerticalScope())
			{
				GUILayout.FlexibleSpace();
				SirenixEditorGUI.InfoMessageBox(
					$"Enter <b>'Name'</b> collection.\n\n" +
					"-collection: e.g. <b>'NewCollection'</b>\n" +
					"-collection container: e.g. <b>'NewCollections'</b>\n" +
					"    use <b>'/'</b> to pathing: e.g. <b>'SomeDirCollections/NewCollections'</b>"
				, false);

				GUI.SetNextControlName("inText");
				_inputNameCollection = SirenixEditorFields.TextField(new GUIContent("collection"), _inputNameCollection);
				_inputNameCollections = SirenixEditorFields.TextField(new GUIContent("collection container"), _inputNameCollections);

				GUI.enabled = IsNameValid();
				if (GUILayout.Button("Create", GUILayout.Height(24)))
				{
					closed = true;

					onCreate?.Invoke(_inputNameCollection, _inputNameCollections);
				}
				GUI.enabled = true;
			}

			if (!_isFocused)
			{
				GUI.FocusControl("inText");
				_isFocused = true;
			}

			if (closed)
			{
				Close();
			}
		}

		public void SetFields(string inputCollection, string inputCollections)
		{
			_inputNameCollection = inputCollection;
			_inputNameCollections = inputCollections;
		}

		private bool IsNameValid()
		{
			if (!string.IsNullOrEmpty(_inputNameCollection) && !string.IsNullOrEmpty(_inputNameCollections))
			{
				if (!_inputNameCollection.Equals(Path.GetFileNameWithoutExtension(_inputNameCollections), StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		public static CreateCollectionPopupWindow Show(Action<string, string> createCallback)
		{
			CreateCollectionPopupWindow window = CreateInstance<CreateCollectionPopupWindow>();
			window.ShowAuxWindow();
			
			window.onCreate = createCallback;

			return window;
		}
	}
}