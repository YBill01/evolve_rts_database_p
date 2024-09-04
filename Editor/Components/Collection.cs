using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EvolveRTS.Editor.Database
{
	public abstract class Collection<T> : ScriptableObject, ISOCollection where T : Element, new()
	{
		//internal static T copyElement = null;

		/*private string _typeName = $"{typeof(T).Name}";

		[TitleGroup("Title", GroupName = "@_typeName", HorizontalLine = false)]
		[BoxGroup("Title/Group0", *//*GroupName = "@_typeName",*//* showLabel: false)]
		[HideLabel, PreviewField(44)]
		[HorizontalGroup("Title/Group0/H0", width: 44)]
		[AssetsOnly]*/
		public Sprite icon;

		/*[VerticalGroup("Title/Group0/H0/W0")]
		[HorizontalGroup("Title/Group0/H0/W0/id")]
		[ReadOnly]
		[LabelWidth(18)]*/
		public int id;

		/*[HorizontalGroup("Title/Group0/H0/W0/id", width: 18)]
		[PropertyTooltip("")]
		[Button(SdfIconType.PencilSquare, "Inspect at new window")]
		private void OnInspect()
		{
			//GUIHelper.OpenInspectorWindow(this);
			OdinEditorWindow.InspectObject(this); // very slowly
		}*/

		//[HorizontalGroup("Group0/Left")]
		//[FoldoutGroup("Title/Group0/H0/W0/notes", expanded: false)]
		[HideLabel]
		[TextArea]
		public string notes;

		/*[BoxGroup("Group0", showLabel: false)]
		[HorizontalGroup("Group0/Id")]
		[ReadOnly]
		public int id;

		[HorizontalGroup("Group0/Id", width: 18)]
		[Button(SdfIconType.PencilSquare, "Inspect at new window")]
		private void OnInspect()
		{
			GUIHelper.OpenInspectorWindow(this);
			//OdinEditorWindow.InspectObject(this); // very slowly
		}

		[FoldoutGroup("Group0/notes", expanded: false)]
		[HideLabel]
		[TextArea]
		public string notes;*/

		[ListDrawerSettings(NumberOfItemsPerPage = 10, CustomAddFunction = "ListElementsOnAdd", DraggableItems = false,/* OnTitleBarGUI = "ListElementsOnSort",*/ AlwaysAddDefaultValue = true /*, DraggableItems = false*/, ShowFoldout = true)]
		[OnCollectionChanged(After = "ListElementsOnAfterAdd")]
		[Searchable]
		[LabelText("elements")]
		public List<T> elements;


		/*[OnStateUpdate("@#(elements).State.Expanded = $value != null")]
		public T copyElement = null;*/

		/*[HideLabel, PreviewField(46)]
		[HorizontalGroup("Group0", width: 46)]
		[AssetsOnly]
		public Sprite icon;

		[BoxGroup("Group0/Left", showLabel: false)]
		[HorizontalGroup("Group0/Left/Id")]
		[ReadOnly]
		public int id;

		[HorizontalGroup("Group0/Left/Id", width: 16)]
		[HideLabel, PropertyTooltip("Enabled")]
		public bool enabled;

		[HorizontalGroup("Group0/Left/Id", width: 18)]
		[Button(SdfIconType.Front, "Duplicate as new element")]
		private void OnDuplicate()
		{
			Debug.Log($"Duplicate {id}");
		}

		[HorizontalGroup("Group0/Left/Id", width: 18)]
		[Button(SdfIconType.PencilSquare, "Inspect at new window")]
		private void OnInspect()
		{
			OdinEditorWindow.InspectObject(this);
		}

		[BoxGroup("Group0/Left", showLabel: false)]
		public int order;



		[FoldoutGroup("@name")]
		public string name;
		[FoldoutGroup("@name")]
		[TextArea]
		public string description;*/

		internal void AddElement(T element)
		{
			elements.Add(element);

			CollectionChangeInfo info = new CollectionChangeInfo
			{
				ChangeType = CollectionChangeType.Add,
				Value = element
			};

			ListElementsOnAfterAdd(info);
		}

		private T ListElementsOnAdd()
		{
			T newElement = new T();
			newElement.ApplyDefault();

			return newElement;
		}
		private void ListElementsOnAfterAdd(CollectionChangeInfo info)
		{
			if (info.ChangeType == CollectionChangeType.Add || info.ChangeType == CollectionChangeType.Insert)
			{
				T newElement = (T)info.Value;
				bool isNewElement = newElement.id == -1;

				if (isNewElement || elements.FindIndex(x => x != newElement && x.id == newElement.id) != -1)
				{
					int lastId = -1;
					foreach (T element in elements)
					{
						if (element.id > lastId)
						{
							lastId = element.id;
						}
					}
					lastId++;

					newElement.id = lastId;
					
					if (isNewElement)
					{
						newElement.order = lastId;
						newElement.name = $"New Name {GetType().BaseType.GetGenericArguments()[0].Name}[{id}][{lastId}]";
					}
				}
			}

			ListElementsSort();

			DatabaseWindow.Instance.UpdateMenuTree();
		}

		private void ListElementsSort()
		{
			elements.Sort((x, y) =>
				y.id.CompareTo(x.id)
			);


			/*if (elementsOrderReverse)
			{
				if (SirenixEditorGUI.ToolbarButton(EditorIcons.ArrowUp))
				{
					elementsOrderReverse = false;
					//ListElementsSort(elementsOrderReverse);
				}
			}
			else
			{
				if (SirenixEditorGUI.ToolbarButton(EditorIcons.ArrowDown))
				{
					elementsOrderReverse = true;
					//ListElementsSort(elementsOrderReverse);
				}
			}*/
		}

		internal void Write(BinaryWriter binary)
		{
			binary.Write(id);
			binary.Write(elements.Count);
			foreach (T element in elements)
			{
				element.Write(binary);
			}
		}
	}
}