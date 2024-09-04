using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using EvolveRTS.Database;
using Sirenix.OdinInspector;

namespace EvolveRTS.Editor.Database
{
	[DrawerPriority(1, 0, 0)]
	public class CollectionDrawer<T, L> : OdinValueDrawer<L> where T : Element, new() where L : Collection<T>
	{
		private bool _propertyNotesExpanded = false;

		/*protected override void Initialize()
		{
			
		}*/

		protected override void DrawPropertyLayout(GUIContent label)
		{
			var property = Property;
			var entry = ValueEntry;
			Collection<T> collection = entry.SmartValue;


			GUIHelper.PushColor(new Color(0.75f, 0.75f, 0.75f, 1.0f));
			GUILayout.Label(typeof(T).Name, EditorStyles.boldLabel);
			GUIHelper.PopColor();

			

			SirenixEditorGUI.BeginBox();

			Rect rect = EditorGUILayout.GetControlRect();

			Rect rectMain = rect.AlignTop(44);
			Rect rectIcon = rectMain.AlignLeft(44);
			Rect rectId = rect.AlignRight(rect.width - (rectIcon.width + 2)).AlignLeft(rect.width - (rectIcon.width + 2) - 22);
			Rect rectInspect = rectId.AlignRight(18).AddX(22).AddY(1);
			Rect rectNotes = rect.AlignRight(rect.width - (rectIcon.width + 2)).AddY(20);


			collection.icon = (Sprite)SirenixEditorFields.UnityPreviewObjectField(rectIcon, collection.icon, typeof(Sprite));

			GUI.enabled = false;
			GUIHelper.PushLabelWidth(18);
			EditorGUI.TextField(rectId.AlignRight(rectId.width - 4), GUIHelper.TempContent("Id"), collection.id.ToString());
			GUIHelper.PopLabelWidth();
			GUI.enabled = true;

			if (SirenixEditorGUI.SDFIconButton(rectInspect, new GUIContent("Inspect at new window"), SdfIconType.PencilSquare, style: SirenixGUIStyles.IconButton))
			{
				OdinEditorWindow.InspectObject(collection);
			}

			//SirenixEditorFields.TextField(rectNotes, collection.notes);
			SirenixEditorGUI.BeginIndentedHorizontal();
			GUILayout.Space(rectIcon.width + 2);
			SirenixEditorGUI.BeginBox();

			bool foldoutNameExpanded = _propertyNotesExpanded;
			if (_propertyNotesExpanded)
			{
				SirenixEditorGUI.BeginBoxHeader();
			}
			_propertyNotesExpanded = SirenixEditorGUI.Foldout(_propertyNotesExpanded, GUIHelper.TempContent("notes"));
			if (foldoutNameExpanded)
			{
				SirenixEditorGUI.EndBoxHeader();
			}
			if (_propertyNotesExpanded)
			{
				//collection.notes = EditorGUI.TextArea(rectNotes, /*_propertyMainFields["name"].Label, *///collection.notes);

				property.Children.Get("notes").Draw();

				//collection.notes = GUILayout.TextArea(collection.notes, 42);

				//collection.notes = SirenixEditorFields.TextField(GUIHelper.TempContent("notes"), collection.notes, EditorStyles.textArea/*, GUILayout.Height(64)*/);

				//collection.notes = SirenixEditorFields.TextField(*//*rectNotes, *//*_propertyMainFields["name"].Label, *//*//collection.notes);
				//_propertyMainFields["name"].Draw();
				//_propertyMainFields["notes"].Draw();
			}
			SirenixEditorGUI.EndBox();
			SirenixEditorGUI.EndIndentedHorizontal();

			SirenixEditorGUI.EndBox();


			//GUI.Label(collection.name);
			//GUILayout.Label(collection.name, EditorStyles.boldLabel);

			property.Children.Get("elements").Draw();

		}
	}
}