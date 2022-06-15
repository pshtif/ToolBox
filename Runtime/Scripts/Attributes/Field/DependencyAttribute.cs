/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace BinaryEgo.ToolBox.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DependencyAttribute : Attribute, IToolBoxAttribute
    {
        private bool _value;

        public bool value => _value;
        
        private string _dependencyName;

        public string dependencyName => _dependencyName;

        public DependencyAttribute(string p_dependencyName, bool p_value = true)
        {
            _dependencyName = p_dependencyName;
            _value = p_value;
        }
    }
}