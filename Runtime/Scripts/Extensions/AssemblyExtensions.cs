/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BinaryEgo.ToolBox
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetClassesAssignableTo<T>(this Assembly p_assembly)
        {
            return p_assembly.GetClassesAssignableTo(typeof(T));
        }
        
        public static IEnumerable<Type> GetClassesAssignableTo(this Assembly p_assembly, Type p_type)
        {
            return p_assembly.GetTypes()
                .Where(assemblyType =>
                    assemblyType.IsClass && !assemblyType.IsAbstract && p_type.IsAssignableFrom(assemblyType));
        }
    }
}