/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace BinaryEgo.ToolBox.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ReorderableListAttribute : OverrideGUIAttribute
	{
	}
}
