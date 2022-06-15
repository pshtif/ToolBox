/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Globalization;

namespace BinaryEgo.ToolBox
{
    public static class FormatExtensions
    {
        public static bool TryParseFloat(this string p_value, out float p_result)
        {
            return float.TryParse(p_value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out p_result);
        }
        
        public static bool TryParseInt(this string p_value, out int p_result)
        {
            return int.TryParse(p_value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out p_result);
        }
        
        public static bool TryParseDateTime(this string p_value, out DateTime p_result)
        {
            return DateTime.TryParse(p_value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out p_result);
        }       
    }
}