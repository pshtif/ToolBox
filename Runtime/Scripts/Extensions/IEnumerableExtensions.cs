/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;

namespace BinaryEgo.ToolBox
{
    public static class IEnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> p_collection, Func<T, bool> p_function)
        {
            int num = 0;
            foreach (T obj in p_collection)
            {
                if (p_function(obj))
                    return num;
                ++num;
            }
            return -1;
        }
        
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> p_source, Action<T> p_action)
        {
            foreach (var item in p_source)
            {
                p_action(item);
            }

            return p_source;
        }
    }
}