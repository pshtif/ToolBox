/*
 *	Created by:  Peter @sHTiF Stefcek
 */
using System;

namespace BinaryEgo.ToolBox.Attributes
{
	public enum EButtonEnableMode
	{
		Always,
		Editor,
		Playmode
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class ButtonAttribute : Attribute, IToolBoxAttribute
	{
		public string label { get; private set; } // Added sHTiF
		
		public string text { get; private set; }
		public EButtonEnableMode selectedEnableMode { get; private set; }		

		public ButtonAttribute(string p_text = null, string p_label = null, EButtonEnableMode p_enabledMode = EButtonEnableMode.Always)
		{
			text = p_text;
			label = p_label;
			selectedEnableMode = p_enabledMode;
		}
	}
}
