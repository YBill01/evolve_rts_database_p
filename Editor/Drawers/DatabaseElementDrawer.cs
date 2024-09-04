using EvolveRTS.Database;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.SceneManagement;

namespace EvolveRTS.Editor.Database
{
	public class DatabaseElementDrawer<T, L> : OdinValueDrawer<L> where T : EvolveRTS.Database.Element, new() where L : DatabaseElement<T>
	{
		private string _elementNameTemp;

		private bool _isEmpty;
		private bool _isNotFound;

		private Element _currentElement;

		protected override void DrawPropertyLayout(GUIContent label)
		{
			var entry = ValueEntry;
			L elementLink = entry.SmartValue;

			ISOCollection collection = Db.collections.Get(Type.GetType($"{DatabaseConfig.ROOT_NAMESPACE}.Editor.Database.Collection.{elementLink.type.Name}"));
			List<Element> elements = (collection.GetType().GetField("elements").GetValue(collection) as IEnumerable<Element>).ToList();

			if (elementLink.id != -1)
			{
				_isEmpty = false;

				string typeStr = elementLink.type.Name;

				_currentElement = elements.FirstOrDefault(x => x.id == elementLink.id);
				
				if (_currentElement == null)
				{
					_isNotFound = true;

					_elementNameTemp = $"[{elementLink.id}]({typeof(T).Name}) --NOT FOUND--";
				}
				else
				{
					_isNotFound = false;

					_elementNameTemp = $"[{elementLink.id}]({typeof(T).Name}) {_currentElement.name}";
				}
			}
			else
			{
				_isEmpty = true;
				_isNotFound = false;

				_elementNameTemp = $"None ({typeof(T).Name})";

				_currentElement = null;
			}

			Rect rect = EditorGUILayout.GetControlRect();

			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, label);
			}

			if (DatabaseElement(rect, elementLink))
			{
				if (elements.Count > 0)
				{
					elements.Sort((x, y) =>
						y.id.CompareTo(x.id)
					);

					List<GenericSelectorItem<Element>> selectorItemList = new List<GenericSelectorItem<Element>>();

					Element selectedSelectorElement = default;
					foreach (Element element in elements)
					{
						GenericSelectorItem<Element> selectorItem = new GenericSelectorItem<Element>($"[{element.id}] {element.name}", element);
						selectorItemList.Add(selectorItem);

						if (elementLink.id != -1 && element.id == elementLink.id)
						{
							selectedSelectorElement = element;
						}
					}

					GenericSelector<Element> selector = new GenericSelector<Element>(typeof(T).Name, false, selectorItemList);
					//selector.EnableSingleClickToSelect();
					selector.SetSelection(selectedSelectorElement);
					selector.ShowInPopup(rect, 280);
					selector.SelectionConfirmed += t =>
					{
						Element selectedElement = t.FirstOrDefault();
						elementLink.id = selectedElement.id;

						if (!Application.isPlaying)
						{
							EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
						}
					};
				}
			}
		}

		private bool DatabaseElement(Rect rect, DatabaseElement<T> elementLink)
		{
			Rect rectInteract = rect.AlignLeft(rect.width - 20);
			Rect rectText = rectInteract.AlignRight(rectInteract.width - 18);
			Rect rectIcon = rect.AlignLeft(18).SetHeight(12).AddY(3);
			Rect rectInspect = rect.AlignRight(16);
			Rect rectClear = rectInteract.AlignRight(18);

			EditorGUIUtility.AddCursorRect(rectInteract, MouseCursor.Arrow);

			bool isHover = rectInteract.Contains(Event.current.mousePosition);
			bool isClick = false;

			int controlID = GUIUtility.GetControlID(FocusType.Passive);

			if (!_isEmpty && rectClear.Contains(Event.current.mousePosition))
			{
				EventType type = Event.current.type;
				if (type == EventType.MouseDown && Event.current.button == 0)
				{
					GUIUtility.hotControl = controlID;
					Event.current.Use();
				}
				else if (type == EventType.MouseUp)
				{
					if (GUIUtility.hotControl == controlID)
					{
						elementLink.id = -1;

						if (!Application.isPlaying)
						{
							EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
						}
					}

					GUIUtility.hotControl = 0;
					Event.current.Use();
				}
			}

			if (GUI.Button(rectInteract, GUIContent.none, EditorStyles.objectField))
			{
				isClick = true;
			}

			if (_isEmpty || _isNotFound)
			{
				SdfIcons.DrawIcon(rectIcon, SdfIconType.Diamond);
			}
			else
			{
				SdfIcons.DrawIcon(rectIcon, SdfIconType.DiamondFill);
			}

			if (_isNotFound)
			{
				GUI.color = Color.red;
			}
			GUI.Label(rectText, _elementNameTemp);
			GUI.color = Color.white;

			if (isHover && !_isEmpty)
			{
				SdfIcons.DrawIcon(rectClear.SetHeight(12).AddY(3), SdfIconType.X);
			}

			GUI.enabled = !_isEmpty && !_isNotFound;
			if (SirenixEditorGUI.SDFIconButton(rectInspect, new GUIContent("Inspect at new window"), SdfIconType.PencilSquare, style: SirenixGUIStyles.IconButton))
			{
				OdinEditorWindow.InspectObject(_currentElement);
			}
			GUI.enabled = true;

			return isClick;
		}
	}
}