/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace BinaryEgo.ToolBox.Editor
{
	public class ReorderableListPropertyDrawer : OverrideGUIDrawer
	{
		public static readonly ReorderableListPropertyDrawer Instance = new ReorderableListPropertyDrawer();

		private readonly Dictionary<string, ReorderableList> _reorderableListsByPropertyName =
			new Dictionary<string, ReorderableList>();

		private string GetPropertyKeyName(SerializedProperty property)
		{
			return property.serializedObject.targetObject.GetInstanceID() + "/" + property.name;
		}

		public override void OnGUI(SerializedProperty p_property)
		{
			EditorGUI.BeginChangeCheck();
			
			if (p_property.isArray)
			{
				string key = GetPropertyKeyName(p_property);

				if (!_reorderableListsByPropertyName.ContainsKey(key))
				{
					ReorderableList reorderableList = new ReorderableList(p_property.serializedObject, p_property, true, true, true, true)
					{
						drawHeaderCallback = (Rect rect) =>
						{
							EditorGUI.LabelField(rect, string.Format("{0}: {1}", p_property.name, p_property.arraySize),
								EditorStyles.boldLabel);
						},

						drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
						{
							SerializedProperty element = p_property.GetArrayElementAtIndex(index);
							rect.y += 1.0f;
							rect.x += 10.0f;
							rect.width -= 10.0f;

							EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 0.0f), element, true);
						},

						elementHeightCallback = (int index) =>
						{
							return EditorGUI.GetPropertyHeight(p_property.GetArrayElementAtIndex(index)) + 4.0f;
						}
					};

					_reorderableListsByPropertyName[key] = reorderableList;
				}

				_reorderableListsByPropertyName[key].DoLayoutList();
			}

			EditorGUI.EndChangeCheck();
		}

		public void ClearCache()
		{
			_reorderableListsByPropertyName.Clear();
		}
	}
}
