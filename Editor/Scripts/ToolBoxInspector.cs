/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinaryEgo.ToolBox.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BinaryEgo.ToolBox.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Object), true)]
	public class ToolBoxInspector : UnityEditor.Editor
	{
		private List<SerializedProperty> _serializedProperties = new List<SerializedProperty>();
		private IEnumerable<FieldInfo> _nonSerializedFields;
		private IEnumerable<PropertyInfo> _nativeProperties;
		private IEnumerable<MethodInfo> _methods;
		private IEnumerable<FieldInfo> _serializedDictionaries;

		private bool _showScriptField = true;

		protected virtual void OnEnable()
		{
			_methods = ReflectionUtils.GetAllMethods(
				target, m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);

			// _serializedDictionaries = ReflectionUtils.GetAllFields(target,
			// 		f => f.FieldType.BaseType.IsGenericType && f.FieldType.BaseType.GetGenericTypeDefinition() ==
			// 			typeof(SerializableDictionary<,>))
			// 	.Where(f => f.FieldType.GetCustomAttribute<SerializableAttribute>() != null

			var hideAttribute = target.GetType().GetCustomAttribute(typeof(HideScriptAttribute), true);
			_showScriptField = hideAttribute == null;
		}

		protected virtual void OnDisable()
		{
			ReorderableListPropertyDrawer.Instance.ClearCache();
		}

		public override void OnInspectorGUI()
		{
			GetSerializedProperties(ref _serializedProperties);
			
			bool anyPixelAttribute = _serializedProperties.Any(p => PropertyUtils.GetAttribute<IToolBoxAttribute>(p) != null);
			bool boundAttributes = _serializedProperties.Any(p => PropertyUtils.GetAttribute<BindUIElement>(p) != null);
			bool anySerializedDictionary = _serializedProperties.Any(p => IsSerializableDictionary(target, p));
			if (!anyPixelAttribute && !anySerializedDictionary && _showScriptField)
			{
				DrawDefaultInspector();
			}
			else
			{
				DrawSerializedProperties();
			}
			
			DrawButtons();

			if (GUI.changed && boundAttributes && !EditorApplication.isPlaying)
			{
				BindableUIManager.ValidateBindings();
			}
		}

		protected void GetSerializedProperties(ref List<SerializedProperty> p_outSerializedProperties)
		{
			p_outSerializedProperties.Clear();
			using (var iterator = serializedObject.GetIterator())
			{
				if (iterator.NextVisible(true))
				{
					do
					{
						p_outSerializedProperties.Add(serializedObject.FindProperty(iterator.name));
					}
					while (iterator.NextVisible(false));
				}
			}
		}

		protected void DrawSerializedProperties()
		{
			serializedObject.Update();

			// Draw non-grouped serialized properties
			foreach (var property in _serializedProperties)
			{
				if (property.name.Equals("m_Script", StringComparison.Ordinal))
				{
					if (_showScriptField)
					{
						GUI.enabled = false;
						EditorGUILayout.PropertyField(property);
						GUI.enabled = true;
					}
				}
				else
				{
					ToolBoxEditorGUI.PropertyGUI(target, property, true);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawButtons()
		{
			if (_methods.Any())
			{
				foreach (var method in _methods)
				{
					ToolBoxEditorGUI.DrawButton(target, method);
				}
			}
		}

		private static GUIStyle GetHeaderGUIStyle()
		{
			GUIStyle style = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.UpperCenter;

			return style;
		}

		public static bool IsSerializableDictionary(Object p_object, SerializedProperty p_property)
		{
			FieldInfo fi = p_object.GetType().GetField(p_property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return fi != null && fi.FieldType.BaseType.IsGenericType && fi.FieldType.BaseType.GetGenericTypeDefinition() ==
					typeof(SerializableDictionary<,>);
		}
	}
}
