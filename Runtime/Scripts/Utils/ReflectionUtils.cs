/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BinaryEgo.ToolBox
{
    public static class ReflectionUtils
    {
        static private Type[] _typeCache;
        static private Assembly[] _assemblyCache;
        
        public static bool IsConstant(this FieldInfo p_field) 
        {
            return p_field.IsReadOnly() && p_field.IsStatic;
        }
        
        public static bool IsStatic(this EventInfo p_info) 
        {
            var m = p_info.GetAddMethod();
            return m != null ? m.IsStatic : false;
        }
        
        public static bool IsReadOnly(this FieldInfo p_field) 
        {
            return p_field.IsInitOnly || p_field.IsLiteral;
        }

        public static bool IsStatic(this PropertyInfo p_info) 
        {
            var m = p_info.GetGetMethod();
            return m != null ? m.IsStatic : false;
        }
        
        public static List<Type> GetAllTypes(Type p_type)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => p_type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
        }

        public static Type GetTypeByName(string p_typeName)
        {
            var type = Type.GetType(p_typeName);
            
            if(type != null)
                return type;

            Assembly assembly; 
            if (p_typeName.IndexOf(".") >= 0)
            {

                var assemblyName = p_typeName.Substring(0, p_typeName.LastIndexOf('.'));

                assembly = Assembly.Load(assemblyName);
            }
            else
            {
                assembly = Assembly.Load("Assembly-CSharp");
            }
            
            if (assembly == null)
                return null;

            return assembly.GetType(p_typeName);
        }

        public static string GetTypeNameWithoutAssembly(string p_type)
        {
            return p_type.Substring(p_type.LastIndexOf('.')+1);
        }

        private static Assembly[] GetAllAssemblies()
        {
            return _assemblyCache != null ? _assemblyCache : _assemblyCache = AppDomain.CurrentDomain.GetAssemblies();   
        } 

        public static Type[] GetAllTypes() {
            if ( _typeCache != null ) {
                return _typeCache;
            }

            var assemblies = GetAllAssemblies();

            var result = new List<Type>();

            assemblies.Where(a => !a.IsDynamic).ToList().ForEach(a => result.AddRange(a.GetExportedTypes()));
            
            return _typeCache = result.OrderBy(t => t.Namespace).ThenBy(t => t.Name).ToArray();
        }
        
        public static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
        {
            List<Type> types = new List<Type>()
            {
                target.GetType()
            };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<FieldInfo> fieldInfos = types[i]
                    .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var fieldInfo in fieldInfos)
                {
                    yield return fieldInfo;
                }
            }
        }
        
        public static IEnumerable<PropertyInfo> GetAllProperties(object target, Func<PropertyInfo, bool> predicate)
        {
            List<Type> types = new List<Type>()
            {
                target.GetType()
            };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<PropertyInfo> propertyInfos = types[i]
                    .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var propertyInfo in propertyInfos)
                {
                    yield return propertyInfo;
                }
            }
        }
        
        public static IEnumerable<MethodInfo> GetAllMethods(object target, Func<MethodInfo, bool> predicate)
        {
            IEnumerable<MethodInfo> methodInfos = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(predicate);

            return methodInfos;
        }

        public static FieldInfo GetField(object target, string fieldName)
        {
            return GetAllFields(target, f => f.Name.Equals(fieldName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

        public static PropertyInfo GetProperty(object target, string propertyName)
        {
            return GetAllProperties(target, p => p.Name.Equals(propertyName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

        public static MethodInfo GetMethod(object target, string methodName)
        {
            return GetAllMethods(target, m => m.Name.Equals(methodName, StringComparison.InvariantCulture)).FirstOrDefault();
        }
    }
}