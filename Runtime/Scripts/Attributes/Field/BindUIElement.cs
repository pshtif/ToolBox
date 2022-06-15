/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace BinaryEgo.ToolBox.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class BindUIElement : Attribute
    {
        public bool overwrite { get; } 
        public string customName { get; }

        public BindUIElement(bool p_overwrite = false, string p_customName = "")
        {
            overwrite = p_overwrite;
            customName = p_customName;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class BindUIButton : BindUIElement
    {
        public BindUIButton(bool p_overwrite = false, string p_customName = "") : base(p_overwrite, p_customName)
        {
            
        }
    }
}