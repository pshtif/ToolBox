/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections;
using System.Reflection;
using BinaryEgo.ToolBox;
using UnityEditor;

namespace BinaryEgo.ToolBox.Editor
{
    public class PropertyUtils
    {
        public static T GetAttribute<T>(SerializedProperty p_property) where T : class
        {
            T[] attributes = GetAttributes<T>(p_property);
            return (attributes.Length > 0) ? attributes[0] : null;
        }
        
        public static T[] GetAttributes<T>(SerializedProperty property) where T : class
        {
            // ADDED FIX sHTiF reflection would crash on null (can happen for some scriptables)
            var obj = GetTargetObjectWithProperty(property);
            if (obj == null)
                return new T[] { };
            
			
            FieldInfo fieldInfo = ReflectionUtils.GetField(obj, property.name);
            if (fieldInfo == null)
                return new T[] { };

            return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }
        
        public static object GetTargetObjectWithProperty(SerializedProperty p_property)
        {
            string path = p_property.propertyPath.Replace(".Array.data[", "[");
            object obj = p_property.serializedObject.targetObject;
            string[] elements = path.Split('.');

            for (int i = 0; i < elements.Length - 1; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "")
                        .Replace("]", ""));
                    obj = GetValueAtIndex(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }

            return obj;
        }

        public static object GetTargetObjectOfProperty(SerializedProperty p_property)
        {
            if (p_property == null)
            {
                return null;
            }

            string path = p_property.propertyPath.Replace(".Array.data[", "[");
            object obj = p_property.serializedObject.targetObject;
            string[] elements = path.Split('.');

            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "")
                        .Replace("]", ""));
                    obj = GetValueAtIndex(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }

            return obj;
        }
        
        private static object GetValue(object p_source, string p_name)
        {
            if (p_source == null)
            {
                return null;
            }

            Type type = p_source.GetType();

            while (type != null)
            {
                FieldInfo field = type.GetField(p_name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(p_source);
                }

                PropertyInfo property = type.GetProperty(p_name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    return property.GetValue(p_source, null);
                }

                type = type.BaseType;
            }

            return null;
        }
        
        private static object GetValueAtIndex(object p_source, string p_name, int p_index)
        {
            IEnumerable enumerable = GetValue(p_source, p_name) as IEnumerable;
            if (enumerable == null)
            {
                return null;
            }

            IEnumerator enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= p_index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
            }

            return enumerator.Current;
        }
    }
}