/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using BinaryEgo.ToolBox.Attributes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BinaryEgo.ToolBox.Editor
{
	public static class ToolBoxEditorGUI
	{
		public static void PropertyGUI(Object p_object, SerializedProperty p_property, bool p_includeChildren)
		{
			OverrideGUIAttribute overrideGUIAttribute = PropertyUtils.GetAttribute<OverrideGUIAttribute>(p_property);

			if (overrideGUIAttribute != null)
			{
				if (AttributeUtils.MeetsDependency(p_property))
				{
					overrideGUIAttribute.GetDrawer().OnGUI(p_property);
				}
			}
			else
			{
				if (AttributeUtils.MeetsDependency(p_property))
				{
					if (ToolBoxInspector.IsSerializableDictionary(p_object, p_property))
					{
						DrawSerializedDictionary(p_object, p_property);
					}
					else
					{
						EditorGUILayout.PropertyField(p_property, null, p_includeChildren);
					}
				}
			}
		}

		public static void DrawButton(Object p_object, MethodInfo p_methodInfo)
		{
			if (!AttributeUtils.MeetsDependency(p_methodInfo, p_object))
				return;
			
			if (p_methodInfo.GetParameters().All(p => p.IsOptional))
			{
				ButtonAttribute buttonAttribute = (ButtonAttribute)p_methodInfo.GetCustomAttributes(typeof(ButtonAttribute), true)[0];
				string buttonText = string.IsNullOrEmpty(buttonAttribute.text)
					? ObjectNames.NicifyVariableName(p_methodInfo.Name)
					: buttonAttribute.text;
				
				EditorGUI.BeginDisabledGroup(false);

				// Added later for button labeling - sHTiF
				if (!string.IsNullOrEmpty(buttonAttribute.label))
					GUILayout.Label(buttonAttribute.label);
				
				if (GUILayout.Button(buttonText))
				{
					object[] defaultParams = p_methodInfo.GetParameters().Select(p => p.DefaultValue).ToArray();
					IEnumerator methodResult = p_methodInfo.Invoke(p_object, defaultParams) as IEnumerator;

					if (!Application.isPlaying)
					{
						// Set target object and scene dirty to serialize changes to disk
						EditorUtility.SetDirty(p_object);

						PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
						if (stage != null)
						{
							EditorSceneManager.MarkSceneDirty(stage.scene);
						}
						else
						{
							EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
						}
					}
					else if (methodResult != null && p_object is MonoBehaviour behaviour)
					{
						behaviour.StartCoroutine(methodResult);
					}
				}

				EditorGUI.EndDisabledGroup();
			}
		}
		
		public static void DrawSerializedDictionary(Object p_object, SerializedProperty p_property)
		{
			FieldInfo dictionaryInfo = p_object.GetType().GetField(p_property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			
			GUILayout.BeginVertical();
			var titleStyle = new GUIStyle();
			titleStyle.normal.textColor = Color.white;
			titleStyle.fontStyle = FontStyle.Bold;
			titleStyle.alignment = TextAnchor.MiddleCenter;
			GUILayout.Label(ObjectNames.NicifyVariableName(dictionaryInfo.Name), titleStyle);

			var dictionary = (IDictionary)dictionaryInfo.GetValue(p_object);
			Type itemType = dictionaryInfo.FieldType.BaseType.GetGenericArguments()[1];
			if (dictionary != null)
			{
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				while (enumerator.MoveNext())
				{
					GUILayout.BeginHorizontal();
					EditorGUI.BeginChangeCheck();

					string newKey = GUILayout.TextField(enumerator.Key.ToString(), GUILayout.Width(100));
					
					var newValue = EditorGUILayout.ObjectField(enumerator.Value as Object, itemType, false);

					if (EditorGUI.EndChangeCheck())
					{
						dictionary[enumerator.Key] = newValue;
						if (newKey != enumerator.Key.ToString())
						{
							dictionary.Remove(enumerator.Key);
							dictionary.Add(GetUniqueKey(dictionary, newKey), enumerator.Value);
						}

						EditorUtility.SetDirty(p_object);
						break;
					}

					if (GUILayout.Button("Remove", GUILayout.Width(80)))
					{
						dictionary.Remove(enumerator.Key);
						
						EditorUtility.SetDirty(p_object);
						break;
					}

					GUILayout.EndHorizontal();
				}
			}

			GUILayout.EndVertical();

			GUI.backgroundColor = Color.gray;
			if (GUILayout.Button("Add Item"))
			{
				if (dictionary == null)
				{
					dictionary = Activator.CreateInstance(dictionaryInfo.FieldType) as IDictionary;
					dictionaryInfo.SetValue(p_object, dictionary);
				}
				dictionary.Add(GetUniqueKey(dictionary, "item"), null);
				EditorUtility.SetDirty(p_object);
			}
			GUILayout.Space(4);
		}
		
		static private string GetUniqueKey(IDictionary p_dictionary, string p_defaultKey)
		{
			int count = 0;
			string key = p_defaultKey;

			if (p_dictionary != null)
			{
				while (p_dictionary.Contains(key))
				{
					key = p_defaultKey + (++count);
				}
			}

			return key;
		}
	}
}
