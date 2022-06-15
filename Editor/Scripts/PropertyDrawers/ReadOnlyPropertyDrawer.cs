/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using BinaryEgo.ToolBox.Attributes;
using UnityEngine;
using UnityEditor;

namespace BinaryEgo.ToolBox.Editor
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyPropertyDrawer : PropertyDrawer
	{
		public void OnGUI(Rect rect, SerializedProperty property)
		{
			// Check if enabled and draw
			EditorGUI.BeginChangeCheck();
			
			EditorGUI.BeginProperty(rect, null, property);

			GUI.enabled = false;
			EditorGUI.PropertyField(rect, property, null, true);
			GUI.enabled = true;

			EditorGUI.EndProperty();
			
			GUI.enabled = true;
			
			// Implement change callbacks later
			// if (EditorGUI.EndChangeCheck())
			// 	ChangedCallback(property);
		}
	}
}