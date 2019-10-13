using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Reflectensions.ExtensionMethods;

namespace Reflectensions
{
    public static class InvokeDIHelper {

        public static void InvokeVoidMethod(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            var parameters = BuildParametersArrayFromServiceProvider(methodInfo, serviceProvider);
            InvokeHelper.InvokeVoidMethod(instance, methodInfo, parameters);
        }

        public static T InvokeMethod<T>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            var parameters = BuildParametersArrayFromServiceProvider(methodInfo, serviceProvider);
            return InvokeHelper.InvokeMethod<T>(instance, methodInfo, parameters);
        }

        public static async Task InvokeVoidMethodAsync(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {

            var parameters = BuildParametersArrayFromServiceProvider(methodInfo, serviceProvider);
            await  InvokeHelper.InvokeVoidMethodAsync(instance, methodInfo, parameters);

        }
        public static async Task<T> InvokeMethodAsync<T>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            var parameters = BuildParametersArrayFromServiceProvider(methodInfo, serviceProvider);
            return await InvokeHelper.InvokeMethodAsync<T>(instance, methodInfo, parameters);
        }

        #region Generic Methods

        #region Void

        public static void InvokeGenericVoidMethod(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, IServiceProvider serviceProvider) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            InvokeVoidMethod(instance, methodInfo, serviceProvider);
        }

        public static void InvokeGenericVoidMethod<TArg>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            InvokeGenericVoidMethod(instance, methodInfo, new[] { typeof(TArg) }, serviceProvider);
        }

        public static void InvokeGenericVoidMethod<TArg1, TArg2>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            InvokeGenericVoidMethod(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, serviceProvider);
        }

        public static void InvokeGenericVoidMethod<TArg1, TArg2, TArg3>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            InvokeGenericVoidMethod(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, serviceProvider);
        }

        #endregion

        #region WithReturn

        public static TResult InvokeGenericMethod<TResult>(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, IServiceProvider serviceProvider) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            return InvokeMethod<TResult>(instance, methodInfo, serviceProvider);
        }

        public static TResult InvokeGenericMethod<TArg, TResult>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            return InvokeGenericMethod<TResult>(instance, methodInfo, new[] { typeof(TArg) }, serviceProvider);
        }

        public static TResult InvokeGenericMethod<TArg1, TArg2, TResult>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            return InvokeGenericMethod<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, serviceProvider);
        }

        public static TResult InvokeGenericMethod<TArg1, TArg2, TArg3, TResult>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            return InvokeGenericMethod<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, serviceProvider);
        }

        #endregion

        #region Task

        public static Task InvokeGenericVoidMethodAsync(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, IServiceProvider serviceProvider) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            return InvokeVoidMethodAsync(instance, methodInfo, serviceProvider);
        }

        public static Task InvokeGenericVoidMethodAsync<TArg>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            return InvokeGenericVoidMethodAsync(instance, methodInfo, new[] { typeof(TArg) }, serviceProvider);
        }

        public static Task InvokeGenericVoidMethodAsync<TArg1, TArg2>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            return InvokeGenericVoidMethodAsync(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, serviceProvider);
        }

        public static Task InvokeGenericVoidMethodAsync<TArg1, TArg2, TArg3>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            return InvokeGenericVoidMethodAsync(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, serviceProvider);
        }

        #endregion

        #region WithTaskOfT

        public static Task<TResult> InvokeGenericMethodAsync<TResult>(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, IServiceProvider serviceProvider) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            return InvokeMethodAsync<TResult>(instance, methodInfo, serviceProvider);
        }

        public static Task<TResult> InvokeGenericMethodAsync<TArg, TResult>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            return InvokeGenericMethodAsync<TResult>(instance, methodInfo, new[] { typeof(TArg) }, serviceProvider);
        }

        public static Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TResult>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            return InvokeGenericMethodAsync<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, serviceProvider);
        }

        public static Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TArg3, TResult>(object instance, MethodInfo methodInfo, IServiceProvider serviceProvider) {
            return InvokeGenericMethodAsync<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, serviceProvider);
        }

        #endregion

        #endregion


        private static object[] BuildParametersArrayFromServiceProvider(MethodInfo methodInfo, IServiceProvider serviceProvider) {
            var pa = methodInfo.GetParameters().Select((p, i) => serviceProvider.GetService(p.ParameterType) ?? p.DefaultValue).ToArray();
            return pa;
        }
    }
}
