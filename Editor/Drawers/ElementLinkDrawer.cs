using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EvolveRTS.Editor.Database
{
	public class ElementLinkDrawer<T, L> : OdinValueDrawer<L> where T : Element, new() where L : ElementLink<T>
	{
		private string _elementNameTemp;

		private bool _isEmpty;
		private bool _isNotFound;

		/*public override bool CanDrawTypeFilter(Type type)
		{
			bool result = false;

			if (type == typeof(DatabaseElementT))
			{
				result = true;
			}

			return result;
		}*/

		protected override void DrawPropertyLayout(GUIContent label)
		{
			var entry = ValueEntry;
			L elementLink = entry.SmartValue;

			if (elementLink.id != -1)
			{
				_isEmpty = false;

				T element = Db.collections.Get<T>(elementLink.id);

				if (element == null)
				{
					_isNotFound = true;

					_elementNameTemp = $"[{elementLink.id}]({typeof(T).Name}) --NOT FOUND--";
				}
				else
				{
					_isNotFound = false;

					_elementNameTemp = $"[{elementLink.id}]({typeof(T).Name}) {element.name}";
				}

				//object elementObj = Database.Instance.collections.Get<T>(entry.SmartValue.id);
				//_elementNameTemp = $"[{entry.SmartValue.id}]({typeof(T).Name}) {(string)elementObj.GetType().GetField("name").GetValue(elementObj)}";
			}
			else
			{
				_isEmpty = true;
				_isNotFound = false;

				_elementNameTemp = $"None ({typeof(T).Name})";
			}



			Rect rect = EditorGUILayout.GetControlRect();

			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, label);
			}
			

			/*if (SirenixEditorGUI.SDFIconButton(rect, new GUIContent(""), SdfIconType.Box, IconAlignment.LeftEdge))
			{
				
			}*/

			if (DatabaseElement(rect, elementLink))
			{
				List<T> elements = Db.collections.Get<T>().elements.ToList();
				
				if (elements.Count > 0)
				{
					elements.Sort((x, y) =>
						y.id.CompareTo(x.id)
					);

					List<GenericSelectorItem<T>> selectorItemList = new List<GenericSelectorItem<T>>();

					T selectedSelectorElement = default;
					foreach (T element in elements)
					{
						GenericSelectorItem<T> selectorItem = new GenericSelectorItem<T>($"[{element.id}] {element.name}", element);
						selectorItemList.Add(selectorItem);

						if (elementLink.id != -1 && element.id == elementLink.id)
						{
							selectedSelectorElement = element;
						}
					}

					GenericSelector<T> selector = new GenericSelector<T>(typeof(T).Name, false, selectorItemList);
					//selector.EnableSingleClickToSelect();
					selector.SetSelection(selectedSelectorElement);
					selector.ShowInPopup(rect, 280);
					selector.SelectionConfirmed += t =>
					{
						T selectedElement = t.FirstOrDefault();
						elementLink.id = selectedElement.id;
					};
				}
			}
		}

		private bool DatabaseElement(Rect rect, ElementLink<T> elementLink)
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
			//SirenixEditorGUI.SlideRectInt(rectClear, controlID, 123);

			
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
					}

					GUIUtility.hotControl = 0;
					Event.current.Use();
				}
			}



			if (GUI.Button(rectInteract, GUIContent.none, EditorStyles.objectField))
			{
				isClick = true;
			}



			//int controlID = GUIUtility.GetControlID(FocusType.Passive);


			/*int controlID = GUIUtility.GetControlID(FocusType.Passive, rectInteract);

			bool isHover = rectInteract.Contains(Event.current.mousePosition);
			bool isClick = false;

			Event current = Event.current;

			switch (current.type)
			{
				case EventType.Repaint:
					EditorStyles.numberField.Draw(rectInteract, GUIContent.none, controlID, isClick, isHover);

					break;
				case EventType.MouseDown:
					if (isHover)
					{
						current.Use();
					}

					break;
				case EventType.MouseUp:
					if (isHover)
					{
						isClick = true;
						current.Use();
					}

					break;
			}*/

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
			//EditorGUI.HandlePrefixLabel(rectText, rectText, new GUIContent(_elementNameTemp), 1);
			GUI.color = Color.white;

			if (isHover && !_isEmpty)
			{
				SdfIcons.DrawIcon(rectClear.SetHeight(12).AddY(3), SdfIconType.X);
				/*if (SirenixEditorGUI.SDFIconButton(rectClear, new GUIContent("Clear element"), SdfIconType.X, style: SirenixGUIStyles.IconButton))
				{
					Debug.Log("Clear");
				}*/
			}

			GUI.enabled = !_isEmpty && !_isNotFound;
			if (SirenixEditorGUI.SDFIconButton(rectInspect, new GUIContent("Inspect at new window"), SdfIconType.PencilSquare, style: SirenixGUIStyles.IconButton))
			{
				T element = Db.collections.Get<T>(elementLink.id);
				OdinEditorWindow.InspectObject(element);
			}
			GUI.enabled = true;

			return isClick;
		}
	}
}