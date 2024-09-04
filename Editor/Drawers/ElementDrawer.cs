using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvolveRTS.Editor.Database
{
	public class ElementDrawer<T> : OdinValueDrawer<T> where T : Element, new()
	{
		private static readonly string[] _propertyMainFieldsNames = { "id", "enabled", "order", "icon", "name", "description" };
		private Dictionary<string, InspectorProperty> _propertyMainFields;
		private List<InspectorProperty> _propertyFields;

		private bool _propertyNameFieldsExpanded = false;
		private bool _propertyFieldsExpanded = false;

		protected override void Initialize()
		{
			_propertyMainFields = new Dictionary<string, InspectorProperty>();
			_propertyFields = new List<InspectorProperty>();
			for (int i = 0; i < Property.Children.Count; i++)
			{
				InspectorProperty child = Property.Children[i];
				if (Array.IndexOf(_propertyMainFieldsNames, child.Name) != -1)
				{
					_propertyMainFields.Add(child.Name, child);
				}
				else
				{
					_propertyFields.Add(child);
				}
			}
		}

		protected override void DrawPropertyLayout(GUIContent label)
		{
			var property = Property;
			var entry = ValueEntry;
			T element = entry.SmartValue;

			Rect rect = EditorGUILayout.GetControlRect();
			
			Rect rectMain = rect.AlignTop(38);
			Rect rectIcon = rectMain.AlignLeft(38);
			Rect rectId = rect.AlignRight(rect.width - (rectIcon.width + 2)).AlignLeft(rect.width - (rectIcon.width + 2) - 62);
			//Rect rectEnable = rectId.AlignRight(18).AddX(22);
			Rect rectEnable = rectId.AlignMiddle(28).AlignRight(16).AddX(22);
			Rect rectCopy = rectId.AlignRight(18).AddX(44).AddY(1);
			Rect rectInspect = rectId.AlignRight(18).AddX(62).AddY(1);
			Rect rectOrder = rect.AlignRight(rect.width - (rectIcon.width + 2)).AddY(20);

			if (!element.enabled)
			{
				SirenixEditorGUI.DrawSolidRect(rectId, new Color(0.75f, 0.0f, 0.0f, 0.25f));
			}


			element.icon = (Sprite)SirenixEditorFields.UnityPreviewObjectField(rectIcon, element.icon, typeof(Sprite));

			GUI.enabled = false;
			GUIHelper.PushLabelWidth(18);
			EditorGUI.TextField(rectId.AlignRight(rectId.width - 4), GUIHelper.TempContent("Id"), element.id.ToString());
			GUIHelper.PopLabelWidth();
			GUI.enabled = true;

			//element.enabled = EditorGUI.Toggle(rectEnable, element.enabled);

			if (SirenixEditorGUI.SDFIconButton(rectEnable, new GUIContent("Enable"), element.enabled ? SdfIconType.ToggleOn : SdfIconType.ToggleOff, style: SirenixGUIStyles.IconButton))
			{
				element.enabled = !element.enabled;
			}

			if (SirenixEditorGUI.SDFIconButton(rectCopy, new GUIContent("Copy this element as new"), SdfIconType.Stickies, style: SirenixGUIStyles.IconButton))
			{
				InspectorProperty elementsProprty = property.ParentValueProperty.TryGetTypedValueEntry<List<T>>().Property;
				Collection<T> collection = elementsProprty.ParentValueProperty.TryGetTypedValueEntry<Collection<T>>().SmartValue;

				collection.AddElement((T)Sirenix.Serialization.SerializationUtility.CreateCopy(element));
			}

			if (SirenixEditorGUI.SDFIconButton(rectInspect, new GUIContent("Inspect at new window"), SdfIconType.PencilSquare, style: SirenixGUIStyles.IconButton))
			{
				OdinEditorWindow.InspectObject(element);
			}

			element.order = SirenixEditorFields.IntField(rectOrder, element.order);





			GUILayoutUtility.GetRect(rect.width, rectMain.height - 18);

			SirenixEditorGUI.BeginBox();
			bool foldoutNameExpanded = _propertyNameFieldsExpanded;
			if (_propertyNameFieldsExpanded)
			{
				SirenixEditorGUI.BeginBoxHeader();
			}
			_propertyNameFieldsExpanded = SirenixEditorGUI.Foldout(_propertyNameFieldsExpanded, element.name);
			if (foldoutNameExpanded)
			{
				SirenixEditorGUI.EndBoxHeader();
			}
			if (_propertyNameFieldsExpanded)
			{
				//element.name = SirenixEditorFields.TextField(_propertyMainFields["name"].Label, element.name);
				_propertyMainFields["name"].Draw();
				_propertyMainFields["description"].Draw();
			}
			SirenixEditorGUI.EndBox();

			SirenixEditorGUI.BeginBox();
			bool foldoutFieldsExpanded = _propertyFieldsExpanded;
			if (_propertyFieldsExpanded)
			{
				SirenixEditorGUI.BeginBoxHeader();
			}
			_propertyFieldsExpanded = SirenixEditorGUI.Foldout(_propertyFieldsExpanded, GUIHelper.TempContent("fields"/*, EditorIcons.List.Raw*/));
			if (foldoutFieldsExpanded)
			{
				SirenixEditorGUI.EndBoxHeader();
			}
			if (_propertyFieldsExpanded)
			{
				if (_propertyFields.Count > 0)
				{
					for (int i = 0; i < _propertyFields.Count; i++)
					{
						_propertyFields[i].Draw();
					}
				}
				else
				{
					SirenixEditorGUI.InfoMessageBox("Fields is empty!");
				}
			}
			SirenixEditorGUI.EndBox();
		}
	}
}