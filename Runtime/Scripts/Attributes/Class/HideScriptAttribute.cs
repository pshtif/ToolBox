/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace BinaryEgo.ToolBox.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class HideScriptAttribute : Attribute, IToolBoxAttribute
    {

    }
}