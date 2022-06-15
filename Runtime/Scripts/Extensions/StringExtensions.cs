/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Linq;

namespace BinaryEgo.ToolBox
{
    public static class StringExtensions
    {
        public static string RemoveWhitespace(this string p_input)
        {
            if (p_input == null)
                return null;
            
            return new string(p_input.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
        
    }
}