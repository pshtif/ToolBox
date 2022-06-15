/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using BinaryEgo.ToolBox.Attributes;
using UnityEditor;

namespace BinaryEgo.ToolBox.Editor
{
    public abstract class OverrideGUIDrawer : PropertyDrawer
    {
        public abstract void OnGUI(SerializedProperty p_property);
    }
    
    public static class OverrideGUIAttributeExtensions
    {
        private static Dictionary<Type, OverrideGUIDrawer> _drawersByAttributeType;

        static OverrideGUIAttributeExtensions()
        {
            _drawersByAttributeType = new Dictionary<Type, OverrideGUIDrawer>();
            _drawersByAttributeType[typeof(ReorderableListAttribute)] = ReorderableListPropertyDrawer.Instance;
        }

        public static OverrideGUIDrawer GetDrawer(this OverrideGUIAttribute p_attr)
        {
            OverrideGUIDrawer drawer;
            if (_drawersByAttributeType.TryGetValue(p_attr.GetType(), out drawer))
                return drawer;
            else
                return null;
        }
    }
}