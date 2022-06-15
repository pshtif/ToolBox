/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Reflection;
using BinaryEgo.ToolBox.Attributes;
using UnityEditor;

namespace BinaryEgo.ToolBox.Editor
{
    public class AttributeUtils
    {

        public static bool MeetsDependency(SerializedProperty property)
        {
            DependencyAttribute dependencyAttribute = PropertyUtils.GetAttribute<DependencyAttribute>(property);
            if (dependencyAttribute != null)
            {
                var obj = PropertyUtils.GetTargetObjectWithProperty(property);
                
                FieldInfo dependencyField = obj.GetType().GetField(dependencyAttribute.dependencyName);
                if (dependencyField != null && dependencyField.FieldType == typeof(bool) &&
                    dependencyAttribute.value != (bool) dependencyField.GetValue(obj))
                    return false;
            }

            return true;
        }
        
        public static bool MeetsDependency(MethodInfo p_methodInfo, Object p_object)
        {
            DependencyAttribute dependencyAttribute = p_methodInfo.GetCustomAttribute<DependencyAttribute>();
            if (dependencyAttribute != null)
            {
                FieldInfo dependencyField = p_object.GetType().GetField(dependencyAttribute.dependencyName);
                if (dependencyField != null && dependencyField.FieldType == typeof(bool) &&
                    dependencyAttribute.value != (bool) dependencyField.GetValue(p_object))
                    return false;
            }

            return true;
        }
    }
}