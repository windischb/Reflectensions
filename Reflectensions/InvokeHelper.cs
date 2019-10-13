﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Reflectensions.ExtensionMethods;
using Reflectensions.Helper;

namespace Reflectensions {
    
    
    public static class InvokeHelper {

        public static void InvokeVoidMethod(object instance, MethodInfo methodInfo, params object[] parameters) {

            if (methodInfo.IsStatic) {
                instance = null;
            } else if (instance == null || instance is Type) {
                instance = TypeHelpers.CreateInstance(methodInfo?.ReflectedType);
            }

            var enumerable = BuildParametersArray(methodInfo, parameters);
            var isTaskVoid = methodInfo.ReturnType == typeof(Task);
            var isTaskReturn = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

            if (isTaskVoid || isTaskReturn) {
                AsyncHelper.RunSync(() => ((Task)methodInfo.Invoke(instance, enumerable)));
                return;
            }

            methodInfo.Invoke(instance, enumerable);
        }
        public static T InvokeMethod<T>(object instance, MethodInfo methodInfo, params object[] parameters) {

            if (methodInfo.IsStatic) {
                instance = null;
            } else if (instance == null || instance is Type) {
                instance = TypeHelpers.CreateInstance(methodInfo?.ReflectedType);
            }


            var enumerable = BuildParametersArray(methodInfo, parameters);
            var isTaskReturn = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

            var returnType = methodInfo.ReturnType;
            if (isTaskReturn) {
                returnType = methodInfo.ReturnType.GenericTypeArguments[0];
            }

            if (TypeHelpers.NotEquals<T>(returnType) && !TypeHelpers.InheritFromClass<T>(returnType) && !TypeHelpers.ImplementsInterface<T>(returnType, false) && !TypeHelpers.IsImplicitCastableTo<T>(returnType))
                throw new Exception($"Method returns a Type of '{methodInfo.ReturnType}' which is not implicitly castable to {typeof(T)}");

            T returnObject;

            if (isTaskReturn) {

                var task = ((Task)methodInfo.Invoke(instance, enumerable));
                returnObject = AsyncHelper.RunSync(() => task.ConvertToTaskOf<T>(false));
            } else {
                returnObject = methodInfo.Invoke(instance, enumerable).To<T>(false);
            }

            return returnObject;
        }
        public static async Task InvokeVoidMethodAsync(object instance, MethodInfo methodInfo, params object[] parameters) {

            if (methodInfo.IsStatic) {
                instance = null;
            } else if (instance == null || instance is Type) {
                instance = TypeHelpers.CreateInstance(methodInfo?.ReflectedType);
            }

            var enumerable = BuildParametersArray(methodInfo, parameters);
            var isTaskVoid = methodInfo.ReturnType == typeof(Task);
            var isTaskReturn = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

            if (isTaskVoid || isTaskReturn) {
                await (Task)methodInfo.Invoke(instance, enumerable);
                return;
            }

            await Task.Run(() => methodInfo.Invoke(instance, enumerable));

        }
        public static async Task<T> InvokeMethodAsync<T>(object instance, MethodInfo methodInfo, params object[] parameters) {

            if (methodInfo.IsStatic) {
                instance = null;
            } else if (instance == null || instance is Type) {
                instance = TypeHelpers.CreateInstance(methodInfo?.ReflectedType);
            }

            var enumerable = BuildParametersArray(methodInfo, parameters);
            var isTaskVoid = methodInfo.ReturnType == typeof(Task);
            if (isTaskVoid) {
                await (Task)methodInfo.Invoke(instance, enumerable);
                return default;
            }

            var isTaskReturn = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

            var returnType = methodInfo.ReturnType;
            if (isTaskReturn) {
                returnType = methodInfo.ReturnType.GenericTypeArguments[0];
            }

            if (TypeHelpers.NotEquals<T>(returnType) && !TypeHelpers.InheritFromClass<T>(returnType) && !TypeHelpers.ImplementsInterface<T>(returnType, false) && !TypeHelpers.IsImplicitCastableTo<T>(returnType))
                throw new Exception($"Method returns a Type of '{methodInfo.ReturnType}' which is not implicitly castable to {typeof(T)}");

            object returnObject = null;

            if (isTaskReturn) {

                var task = (Task)methodInfo.Invoke(instance, enumerable);
                await task;
                var resultProperty = typeof(Task<>).MakeGenericType(methodInfo.ReturnType.GetGenericArguments().FirstOrDefault()).GetProperty("Result");
                returnObject = resultProperty.GetValue(task);
            } else {
                returnObject = await Task.Run(() => methodInfo.Invoke(instance, enumerable));
            }

            return returnObject.To<T>(false);

        }

        #region Generic Methods

        #region Void

        public static void InvokeGenericVoidMethod(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, params object[] parameters) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            InvokeVoidMethod(instance, methodInfo, parameters);
        }

        public static void InvokeGenericVoidMethod<TArg>(object instance, MethodInfo methodInfo, params object[] parameters) {
            InvokeGenericVoidMethod(instance, methodInfo, new[] { typeof(TArg) }, parameters);
        }

        public static void InvokeGenericVoidMethod<TArg1, TArg2>(object instance, MethodInfo methodInfo, params object[] parameters) {
            InvokeGenericVoidMethod(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, parameters);
        }

        public static void InvokeGenericVoidMethod<TArg1, TArg2, TArg3>(object instance, MethodInfo methodInfo, params object[] parameters) {
            InvokeGenericVoidMethod(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, parameters);
        }
        
        #endregion

        #region WithReturn

        public static TResult InvokeGenericMethod<TResult>(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, params object[] parameters) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            return InvokeMethod<TResult>(instance, methodInfo, parameters);
        }

        public static TResult InvokeGenericMethod<TArg, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethod<TResult>(instance, methodInfo, new[] { typeof(TArg) }, parameters);
        }

        public static TResult InvokeGenericMethod<TArg1, TArg2, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethod<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, parameters);
        }

        public static TResult InvokeGenericMethod<TArg1, TArg2, TArg3, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethod<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, parameters);
        }

        #endregion

        #region Task

        public static Task InvokeGenericVoidMethodAsync(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, params object[] parameters) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            return InvokeVoidMethodAsync(instance, methodInfo, parameters);
        }

        public static Task InvokeGenericVoidMethodAsync<TArg>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericVoidMethodAsync(instance, methodInfo, new[] { typeof(TArg) }, parameters);
        }

        public static Task InvokeGenericVoidMethodAsync<TArg1, TArg2>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericVoidMethodAsync(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, parameters);
        }
        
        public static Task InvokeGenericVoidMethodAsync<TArg1, TArg2, TArg3>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericVoidMethodAsync(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, parameters);
        }

        #endregion

        #region WithTaskOfT

        public static Task<TResult> InvokeGenericMethodAsync<TResult>(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, params object[] parameters) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            return InvokeMethodAsync<TResult>(instance, methodInfo, parameters);
        }

        public static Task<TResult> InvokeGenericMethodAsync<TArg, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethodAsync<TResult>(instance, methodInfo, new[] { typeof(TArg) }, parameters);
        }
        
        public static Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethodAsync<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, parameters);
        }
        
        public static Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TArg3, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethodAsync<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, parameters);
        }

        #endregion

        #endregion



        private static object[] BuildParametersArray(MethodInfo methodInfo, IEnumerable<object> parameters) {
            var enumerable = parameters as object[] ?? parameters.ToArray();

            var pa = methodInfo.GetParameters().Select((p, i) => {

                if (enumerable.Length > i) {

                    return enumerable[i].To(p.ParameterType, false);
                }

                return p.DefaultValue;
            }).ToArray();

            return pa;
        }

      
    }

}
