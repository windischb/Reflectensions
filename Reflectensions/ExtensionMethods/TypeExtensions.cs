using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflectensions.HelperClasses;

namespace Reflectensions.ExtensionMethods {
    public static class TypeExtensions {
        #region Constants

        public static Dictionary<Type, List<Type>> ImplicitNumericConversionsTable = new Dictionary<Type, List<Type>> {
            { typeof(sbyte),   new List<Type> { typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(byte),    new List<Type> { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(short),   new List<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(ushort),  new List<Type> { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(int),     new List<Type> { typeof(long), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(uint),    new List<Type> { typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(long),    new List<Type> { typeof(float), typeof(double), typeof(decimal) } },
            { typeof(char),    new List<Type> { typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(float),   new List<Type> { typeof(double) } },
            { typeof(ulong),   new List<Type> { typeof(float), typeof(double), typeof(decimal) } },
            { typeof(double),  new List<Type>() },
            { typeof(decimal), new List<Type>() }
        };
        public static List<Type> NumericTypes => ImplicitNumericConversionsTable.Keys.ToList();

        #endregion

        #region Check Type
        public static bool IsNumericType(this Type type, bool throwOnError = true) {
            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }

            return NumericTypes.Contains(type);
        }

        public static bool IsNumericType<T>(bool throwOnError = true) {
            return IsNumericType(typeof(T), throwOnError);
        }

        public static bool IsGenericTypeOf(this Type type, Type genericType, bool throwOnError = true) {

            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }
            

            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }

        public static bool IsGenericTypeOf<T>(this Type type, bool throwOnError = true) {
            return IsGenericTypeOf(type, typeof(T), throwOnError);
        }

        public static bool IsNullableType(this Type type, bool throwOnError = true) {
            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }

            return IsGenericTypeOf(type, typeof(Nullable<>));
        }

        public static bool IsNullableType<T>(bool throwOnError = true) {
            return IsNullableType(typeof(T), throwOnError);
        }

        public static bool IsEnumerableType(this Type type, bool throwOnError = true) {

            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }

            if (type == typeof(string))
                return false;

            if (IsDictionaryType(type))
                return false;

            return type.GetInterfaces().Contains(typeof(IEnumerable));
        }

        public static bool IsEnumerableType<T>(bool throwOnError = true) {
            return IsEnumerableType(typeof(T), throwOnError);
        }

        public static bool IsDictionaryType(this Type type, bool throwOnError = true) {
            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }


            return IsGenericTypeOf(type, typeof(IDictionary)) || IsGenericTypeOf(type, typeof(IDictionary<,>)) || ImplementsInterface(type, typeof(IDictionary)) || ImplementsInterface(type, typeof(IDictionary<,>));
        }

        public static bool IsDictionaryType<T>(bool throwOnError = true) {
            return IsDictionaryType(typeof(T), throwOnError);
        }

        public static bool ImplementsInterface<T>(this Type type, bool throwOnError = true) {
            return ImplementsInterface(type, typeof(T), throwOnError);
        }
        
        public static bool ImplementsInterface(this Type type, Type interfaceType, bool throwOnError = true) {

            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }

            if (interfaceType == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(interfaceType));
                }
                return false;
            }


            if (!interfaceType.IsInterface) {
                if (throwOnError) {
                    throw new ArgumentException($"Paramter '{nameof(interfaceType)}' must be an Interface Type!");
                }
                return false;
            }

            return type.GetInterfaces().Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : i).Contains(interfaceType);
        }

        public static bool InheritFromClass<T>(this Type type, bool inherit = false, bool throwOnError = true) {
            return InheritFromClass(type, typeof(T), inherit, throwOnError);
        }
       
        public static bool InheritFromClass(this Type type, Type from, bool inherit = false, bool throwOnError = true) {

            int? maxLevel = null;
            if (!inherit)
                maxLevel = 0;

            return InheritFromClassLevel(type, from, maxLevel, throwOnError) > 0;

        }

        public static bool IsImplicitCastableTo<T>(this Type type, bool throwOnError = true) {
            return IsImplicitCastableTo(type, typeof(T), throwOnError);
        }
        
        public static bool IsImplicitCastableTo(this Type type, Type to, bool throwOnError = true) {

            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }

            if (to == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(to));
                }
                return false;
            }

            if (IsNumericType(type) && IsNumericType(to)) {
                if (ImplicitNumericConversionsTable[type].Contains(to))
                    return true;
            }

            if (ImplementsInterface(type, to, false) || InheritFromClass(type, to, true)) {
                return true;
            }

            if(GetImplicitCastMethodTo(type, to) != null)
                return true;

            if (IsNullableType(to)) {
                var underlingType = Nullable.GetUnderlyingType(to);
                if (IsImplicitCastableTo(type, underlingType, throwOnError)) {
                    return true;
                }
            }

            return false;

        }

        public static bool Equals<T>(this Type type) {
            return type == typeof(T);
        }
        
        public static bool NotEquals<T>(this Type type) {
            return !Equals<T>(typeof(T));
        }

        public static bool HasAttribute<T>(this Type type, bool inherit = false, bool throwOnError = true) where T : Attribute {

            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }


            return HasAttribute(type, typeof(T));
        }
        
        public static bool HasAttribute(this Type type, Type attributeType, bool inherit = false, bool throwOnError = true) {
            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }

            if (!InheritFromClass<Attribute>(attributeType)) {
                throw new ArgumentException($"Parameter '{nameof(attributeType)}' has to be an Attribute Type!");
            }

            return type.GetCustomAttribute(attributeType, inherit) != null;
        }

        public static bool IsStatic(this Type type, bool throwOnError = true) {
            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return false;
            }

            return type.IsAbstract && type.IsSealed;
        }


        #endregion



        #region Linq IEnumerable<Type>

        public static IEnumerable<Type> WithAttribute<T>(this IEnumerable<Type> types, bool inherit = false) where T : Attribute {
            return types.Where(m => HasAttribute<T>(m, inherit, false));
        }

        public static IEnumerable<Type> WithAttribute(this IEnumerable<Type> types, Type attributeType, bool inherit = false) {
            return types.Where(m => HasAttribute(m, attributeType, inherit, false));
        }

        public static IEnumerable<(Type Type, T Attribute)> WithAttributeExpanded<T>(this IEnumerable<Type> types, bool inherit = false) where T : Attribute {
            return WithAttribute<T>(types, inherit).Select(t => (t, t.GetCustomAttribute<T>()));
        }

        public static IEnumerable<(Type Type, Attribute Attribute)> WithAttributeExpanded(this IEnumerable<Type> types, Type attributeType, bool inherit = false) {

            if (!InheritFromClass<Attribute>(attributeType, true, false)) {
                throw new ArgumentException($"Parameter '{nameof(attributeType)}' be an Attribute Type!");
            }

            return WithAttribute(types, attributeType, inherit).Select(t => (t, t.GetCustomAttribute(attributeType)));
        }

        public static IEnumerable<Type> WhichIsGenericTypeOf(this IEnumerable<Type> types, Type of) {
            return types.Where(t => IsGenericTypeOf(t, of, false));
        }

        public static IEnumerable<Type> WhichIsGenericTypeOf<T>(this IEnumerable<Type> types) {
            return types.Where(t => IsGenericTypeOf<T>(t, false));
        }

        public static IEnumerable<Type> WhichInheritFromClass(this IEnumerable<Type> types, Type from, bool inherit = false) {
            return types.Where(t => InheritFromClass(t, from, inherit, false));
        }
        public static IEnumerable<Type> WhichInheritFromClass<T>(this IEnumerable<Type> types, bool inherit = false) {
            return types.Where(t => InheritFromClass<T>(t, inherit, false));
        }

        #endregion



        #region Get Operator Methods


        public static IEnumerable<MethodInfo> GetImplictOperatorMethods(this Type type, bool throwOnError = true) {

            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return new List<MethodInfo>();
            }
            
            return type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.HasName("op_Implicit"));
        }
        public static IEnumerable<MethodInfo> GetExplicitOperatorMethods(this Type type, bool throwOnError = true) {
            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return new List<MethodInfo>();
            }

            return type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.HasName("op_Explicit"));
        }

        public static IEnumerable<MethodInfo> GetImplictOperatorMethods<T>(bool throwOnError = true) {
            return GetImplictOperatorMethods(typeof(T), throwOnError);
        }

        public static IEnumerable<MethodInfo> GetExplicitOperatorMethods<T>(bool throwOnError = true) {
            return GetExplicitOperatorMethods(typeof(T), throwOnError);
        }

        public static MethodInfo GetImplicitCastMethodTo<T>(this Type fromType, bool throwOnError = true) {
            return GetImplicitCastMethodTo(fromType, typeof(T), throwOnError);
        }
        public static MethodInfo GetImplicitCastMethodTo(this Type fromType, Type toType, bool throwOnError = true) {
            if (fromType == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(fromType));
                }
                return null;
            }

            if (toType == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(toType));
                }
                return null;
            }

            return GetImplictOperatorMethods(fromType, false).WithReturnType(toType).WithParametersOfType(fromType).FirstOrDefault() ??
                     GetImplictOperatorMethods(toType, false).WithReturnType(toType).WithParametersOfType(fromType).FirstOrDefault();

        }

        public static MethodInfo GetExplicitCastMethodTo<T>(this Type fromType, bool throwOnError = true) {
            return GetExplicitCastMethodTo(fromType, typeof(T), throwOnError);
        }
        public static MethodInfo GetExplicitCastMethodTo(this Type fromType, Type toType, bool throwOnError = true) {
            if (fromType == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(fromType));
                }
                return null;
            }

            if (toType == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(toType));
                }
                return null;
            }

            return GetExplicitOperatorMethods(fromType, false).WithReturnType(toType).WithParametersOfType(fromType).FirstOrDefault() ??
                     GetExplicitOperatorMethods(toType, false).WithReturnType(toType).WithParametersOfType(fromType).FirstOrDefault();
        }
        
        #endregion

      

        public static object CreateInstance(this Type type, params object[] args) {
            if (type == null)
                return null;

            return Activator.CreateInstance(type, args);
        }

        public static Type FindType(string typeName) {

            if (string.IsNullOrWhiteSpace(typeName))
                return typeof(void);


            lock (TypeHelperCache.TypeFromStringLock) {
                if (TypeHelperCache.TypeFromString.ContainsKey(typeName))
                    return TypeHelperCache.TypeFromString[typeName];

                Type foundType = null;

                if (!typeName.Contains(".")) {
                    foundType = Type.GetType($"System.{typeName}", false, true);
                }

                if (foundType == null) {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                    foreach (var assembly in assemblies) {

                        foundType = assembly.GetType(typeName, false, false);
                        if (foundType != null) {
                            break;
                        }

                    }

                    if (foundType == null) {
                        foreach (var assembly in assemblies) {
                            foundType = assembly.GetType(typeName, false, true);
                            if (foundType != null) {
                                break;
                            }
                        }
                    }
                }

                TypeHelperCache.TypeFromString.Add(typeName, foundType);

                return foundType;
            }

        }

        public static int InheritFromClassLevel<T>(this Type type, int? maximumLevel = null, bool throwOnError = true) {
            return InheritFromClassLevel(type, typeof(T), maximumLevel, throwOnError);
        }

        public static int InheritFromClassLevel(this Type type, Type from, int? maximumLevel = null, bool throwOnError = true) {
            if (type == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(type));
                }
                return -1;
            }

            if (from == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(from));
                }
                return -1;
            }

            lock (TypeHelperCache.InheritanceListLock) {
                var exists = TypeHelperCache.InheritanceList.FirstOrDefault(item => item?.Type == type && item?.From == from);
                if (exists != null)
                    return exists.Value.Level;

                var level = 0;
                bool found = type == from;
                var loockupType = type;

                if (loockupType.IsGenericType) {
                    found = loockupType.GetGenericTypeDefinition() == from;
                }

                while (loockupType != null && !found && (!maximumLevel.HasValue || level >= maximumLevel.Value)) {
                    level++;

                    found = loockupType.BaseType == from;
                    if(found) {
                        break;
                    }
                    loockupType = loockupType.BaseType;
                    if (loockupType?.IsGenericType == true) {
                        found = loockupType.GetGenericTypeDefinition() == from;
                    }

                }

                if (!found)
                    level = -1;

                TypeHelperCache.InheritanceList.Add((type, from, level));
                return level;
            }

        }

        public static Type GetUnderlyingType(this Type type) {

            if (!IsNullableType(type, false))
                throw new ArgumentException($"'{type}' is not a Nullable Type...");


            return Nullable.GetUnderlyingType(type);
        }

        public static Type GetUnderlyingType<T>() {

            return GetUnderlyingType(typeof(T));
        }





    }
}
