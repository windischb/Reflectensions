using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Reflectensions.ExtensionMethods;
using Reflectensions.Helpers;

namespace Reflectensions {
    public class MethodManager<TBox> where TBox : IMethodBox {

        private readonly MethodBoxCache<TBox> _methodInfoBoxCache = new MethodBoxCache<TBox>();

        public MethodManagerOptions Options = new MethodManagerOptions();

        public MethodManager() {

        }

        public MethodManager(MethodManagerOptions options) {
            Options = options;
        }

        public MethodManager(Action<MethodManagerOptionsBuilder> builder) {
            var optionsBuilder = new MethodManagerOptionsBuilder();

            builder?.Invoke(optionsBuilder);
            Options = optionsBuilder;
        }

        public void SetMethodBoxBuilderAction(Action<MethodBoxBuilderContext, MethodBoxBuilderResult<TBox>> builderfunction) {
            _methodBoxBuilderAction = builderfunction;
        }

        public TBox RegisterMethod(MethodInfo methodInfo) {
            var box = BuildMethod(MethodBoxBuilderContext.Build(methodInfo, Options));
            return _methodInfoBoxCache.Add(box);
        }

        public TBox RegisterMethod(TBox methodBox, MethodSearch customSearch = null) {

            return _methodInfoBoxCache.Add(customSearch ?? MethodSignature.FromMethodInfo(methodBox.MethodInfo), methodBox);
        }

        private Action<MethodBoxBuilderContext, MethodBoxBuilderResult<TBox>> _methodBoxBuilderAction;

        private static object[] BuildParametersArray(MethodInfo methodInfo, IEnumerable<object> parameters) {
            var enumerable = parameters as object[] ?? parameters.ToArray();

            var pa = methodInfo.GetParameters().Select((p, i) => {

                if (enumerable.Length > i) {

                    return enumerable[i].ConvertTo(p.ParameterType, false);
                }

                return p.DefaultValue;
            }).ToArray();

            return pa;
        }


        
        private TBox _FindMethod(MethodSearch search) {
            
            var bindingFlags = search.Context.AccessModifier.ToBindingFlags().Add(search.Context.MethodType.ToBindingFlags());
            var methodInfo = search.Context.OwnerType.FoundMatchingType.GetMethods(bindingFlags).FindBestMatchingMethodInfo(search);
            return BuildMethod(MethodBoxBuilderContext.Build(methodInfo, Options)).MethodInfoBox;
        }

        public TBox FindMethod(MethodSearch search) {

            if (Options.EnableCache) {
                return _methodInfoBoxCache.GetOrAdd(search, () => _FindMethod(search));
            } else {
                return _FindMethod(search);
            }

        }

        private MethodBoxBuilderResult<TBox> BuildMethod(MethodBoxBuilderContext options) {
            if (_methodBoxBuilderAction == null)
                throw new Exception("MethodBoxBuilderAction is not defined!");
            var boxkit = new MethodBoxBuilderResult<TBox>();
            _methodBoxBuilderAction(options, boxkit);
            if (boxkit.Ignore)
                return null;

            return boxkit;
        }

        

        private (MethodInfo MethodInfo, object[] Parameters) GetMethodInfo(InvokeBuilder builder) {
            var methodInfoBox = FindMethod(builder.Context);
            MethodInfo methodInfo = methodInfoBox.MethodInfo;
            if (builder.Context.Context.GenericArguments.Any()) {
                methodInfo = methodInfo.MakeGenericMethod(builder.Context.Context.GenericArguments.Select(a => a.FoundMatchingType).ToArray());
            }

            return (methodInfo, BuildParametersArray(methodInfo, builder.Context.Parameters));
        }

        #region Non-Generic Instance Methods

        public void InvokeVoidMethod(object instance, MethodInfo methodInfo, params object[] parameters) {

            if (methodInfo.IsStatic) {
                instance = null;
            } else if (instance == null || instance is Type) {
                instance = methodInfo.ReflectedType.CreateInstance();
            }

            var enumerable = BuildParametersArray(methodInfo, parameters);
            var isTaskVoid = methodInfo.ReturnType == typeof(Task);
            var isTaskReturn = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
            
            if (isTaskVoid || isTaskReturn) {
                ((Task)methodInfo.Invoke(instance, enumerable)).Wait();
                return;
            }

            methodInfo.Invoke(instance, enumerable);
        }
        public T InvokeMethod<T>(object instance, MethodInfo methodInfo, params object[] parameters) {

            if (methodInfo.IsStatic) {
                instance = null;
            } else if (instance == null || instance is Type) {
                instance = methodInfo.ReflectedType.CreateInstance();
            }


            var enumerable = BuildParametersArray(methodInfo, parameters);
            var isTaskReturn = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

            var returnType = methodInfo.ReturnType;
            if (isTaskReturn) {
                returnType = methodInfo.ReturnType.GenericTypeArguments[0];
            }

            if (returnType.NotEquals<T>() && !returnType.InheritFromClass<T>() && !returnType.ImplementsInterface<T>(false) && !returnType.IsImplicitCastableTo<T>())
                throw new Exception($"Method returns a Type of '{methodInfo.ReturnType}' which is not implicitly castable to {typeof(T)}");

            T returnObject;

            if (isTaskReturn) {
                
                var task = ((Task)methodInfo.Invoke(instance, enumerable));
                returnObject = AsyncHelper.RunSync(() => task.ConvertToTaskOf<T>(false));
                //task.GetAwaiter().GetResult();

                //var resultProperty = typeof(Task<>).MakeGenericType(methodInfo.ReturnType.GetGenericArguments().FirstOrDefault()).GetProperty("Result");
                //returnObject = resultProperty?.GetValue(task);
            } else {
                returnObject = methodInfo.Invoke(instance, enumerable).ConvertTo<T>(false);
            }

            return returnObject; // != null ? returnObject.ConvertTo<T>() : default(T);
        }
        public async Task InvokeVoidMethodAsync(object instance, MethodInfo methodInfo, params object[] parameters) {

            if (methodInfo.IsStatic) {
                instance = null;
            } else if (instance == null || instance is Type) {
                instance = methodInfo.ReflectedType.CreateInstance();
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
        public async Task<T> InvokeMethodAsync<T>(object instance, MethodInfo methodInfo, params object[] parameters) {

            if (methodInfo.IsStatic) {
                instance = null;
            } else if (instance == null || instance is Type) {
                instance = methodInfo.ReflectedType.CreateInstance();
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

            if (returnType.NotEquals<T>() && !returnType.InheritFromClass<T>() && !returnType.ImplementsInterface<T>(false) && !returnType.IsImplicitCastableTo<T>())
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

            return returnObject.ConvertTo<T>(false);

        }

        #region Simplified versions
        public void InvokeMethod(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            InvokeVoidMethod(instance, methodInfo, parameters);
        }
        public void InvokeMethod(object instance, Action<InvokeBuilder> builder) {
            InvokeMethod(instance, builder.InvokeAction());
        }
        public void InvokeMethod(object instance, string name, params object[] parameters) {
            InvokeMethod(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }

        public T InvokeMethod<T>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeMethod<T>(instance, methodInfo, parameters);
        }
        public T InvokeMethod<T>(object instance, Action<InvokeBuilder> builder) {
            return InvokeMethod<T>(instance, builder.InvokeAction());
        }
        public T InvokeMethod<T>(object instance, string name, params object[] parameters) {
            return InvokeMethod<T>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }

        public async Task InvokeMethodAsync(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            await InvokeVoidMethodAsync(instance, methodInfo, parameters);
        }
        public async Task InvokeMethodAsync(object instance, Action<InvokeBuilder> builder) {
            await InvokeMethodAsync(instance, builder.InvokeAction());
        }
        public async Task InvokeMethodAsync(object instance, string name, params object[] parameters) {
            await InvokeMethodAsync(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }

        public async Task<T> InvokeMethodAsync<T>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return await InvokeMethodAsync<T>(instance, methodInfo, parameters);
        }
        public async Task<T> InvokeMethodAsync<T>(object instance, Action<InvokeBuilder> builder) {
            return await InvokeMethodAsync<T>(instance, builder.InvokeAction());
        }
        public async Task<T> InvokeMethodAsync<T>(object instance, string name, params object[] parameters) {
            return await InvokeMethodAsync<T>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }
        #endregion

        #endregion


        #region Generic Methods


        #region Void
        public void InvokeGenericVoidMethod(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, params object[] parameters) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            InvokeVoidMethod(instance, methodInfo, parameters);
        }

        public void InvokeGenericVoidMethod(object instance, InvokeBuilder builder, IEnumerable<Type> genericArguments) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            InvokeGenericVoidMethod(instance, methodInfo, genericArguments, parameters);
        }
        public void InvokeGenericVoidMethod(object instance, Action<InvokeBuilder> builder, IEnumerable<Type> genericArguments) {
            InvokeGenericVoidMethod(instance, builder.InvokeAction(), genericArguments);
        }
        public void InvokeGenericVoidMethod(object instance, string name, IEnumerable<Type> genericArguments, params object[] parameters) {
            InvokeGenericVoidMethod(instance, builder => builder.WithMethodName(name).WithParameters(parameters), genericArguments.ToArray());
        }



        public void InvokeGenericVoidMethod<TArg>(object instance, MethodInfo methodInfo, params object[] parameters) {
            InvokeGenericVoidMethod(instance, methodInfo, new []{typeof(TArg)}, parameters);
        }

        public void InvokeGenericVoidMethod<TArg>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            InvokeGenericVoidMethod<TArg>(instance, methodInfo, parameters);
        }
        public void InvokeGenericVoidMethod<TArg>(object instance, Action<InvokeBuilder> builder) {
            InvokeGenericVoidMethod<TArg>(instance, builder.InvokeAction());
        }
        public void InvokeGenericVoidMethod<TArg>(object instance, string name, params object[] parameters) {
            InvokeGenericVoidMethod<TArg>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }




        public void InvokeGenericVoidMethod<TArg1, TArg2>(object instance, MethodInfo methodInfo, params object[] parameters) {
            InvokeGenericVoidMethod(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, parameters);
        }

        public void InvokeGenericVoidMethod<TArg1, TArg2>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            InvokeGenericVoidMethod<TArg1, TArg2>(instance, methodInfo, parameters);
        }
        public void InvokeGenericVoidMethod<TArg1, TArg2>(object instance, Action<InvokeBuilder> builder) {
            InvokeGenericVoidMethod<TArg1, TArg2>(instance, builder.InvokeAction());
        }
        public void InvokeGenericVoidMethod<TArg1, TArg2>(object instance, string name, params object[] parameters) {
            InvokeGenericVoidMethod<TArg1, TArg2>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }


        public void InvokeGenericVoidMethod<TArg1, TArg2, TArg3>(object instance, MethodInfo methodInfo, params object[] parameters) {
            InvokeGenericVoidMethod(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, parameters);
        }
        public void InvokeGenericVoidMethod<TArg1, TArg2, TArg3>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            InvokeGenericVoidMethod<TArg1, TArg2, TArg3>(instance, methodInfo, parameters);
        }
        public void InvokeGenericVoidMethod<TArg1, TArg2, TArg3>(object instance, Action<InvokeBuilder> builder) {
            InvokeGenericVoidMethod<TArg1, TArg2, TArg3>(instance, builder.InvokeAction());
        }
        public void InvokeGenericVoidMethod<TArg1, TArg2, TArg3>(object instance, string name, params object[] parameters) {
            InvokeGenericVoidMethod<TArg1, TArg2, TArg3>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }

        #endregion

        #region WithReturn

        public TResult InvokeGenericMethod<TResult>(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, params object[] parameters) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            return InvokeMethod<TResult>(instance, methodInfo, parameters);
        }


        public TResult InvokeGenericMethod<TResult>(object instance, InvokeBuilder builder, IEnumerable<Type> genericArguments) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericMethod<TResult>(instance, methodInfo, genericArguments, parameters);
        }
        public TResult InvokeGenericMethod<TResult>(object instance, Action<InvokeBuilder> builder, IEnumerable<Type> genericArguments) {
            return InvokeGenericMethod<TResult>(instance, builder.InvokeAction(), genericArguments);
        }
        public TResult InvokeGenericMethod<TResult>(object instance, string name, IEnumerable<Type> genericArguments, params object[] parameters) {
            return InvokeGenericMethod<TResult>(instance, builder => builder.WithMethodName(name).WithParameters(parameters), genericArguments.ToArray());
        }



        public TResult InvokeGenericMethod<TArg, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethod<TResult>(instance, methodInfo, new[] { typeof(TArg) }, parameters);
        }

        public TResult InvokeGenericMethod<TArg, TResult>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericMethod<TArg, TResult>(instance, methodInfo, parameters);
        }
        public TResult InvokeGenericMethod<TArg, TResult>(object instance, Action<InvokeBuilder> builder) {
            return InvokeGenericMethod<TArg, TResult>(instance, builder.InvokeAction());
        }
        public TResult InvokeGenericMethod<TArg, TResult>(object instance, string name, params object[] parameters) {
            return InvokeGenericMethod<TArg, TResult>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }



        public TResult InvokeGenericMethod<TArg1, TArg2, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethod<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, parameters);
        }

        public TResult InvokeGenericMethod<TArg1, TArg2, TResult>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericMethod<TArg1, TArg2, TResult>(instance, methodInfo, parameters);
        }
        public TResult InvokeGenericMethod<TArg1, TArg2, TResult>(object instance, Action<InvokeBuilder> builder) {
            return InvokeGenericMethod<TArg1, TArg2, TResult>(instance, builder.InvokeAction());
        }
        public TResult InvokeGenericMethod<TArg1, TArg2, TResult>(object instance, string name, params object[] parameters) {
            return InvokeGenericMethod<TArg1, TArg2, TResult>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }






        public TResult InvokeGenericMethod<TArg1, TArg2, TArg3, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethod<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, parameters);
        }
        public TResult InvokeGenericMethod<TArg1, TArg2, TArg3, TResult>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericMethod<TArg1, TArg2, TArg3, TResult>(instance, methodInfo, parameters);
        }
        public TResult InvokeGenericMethod<TArg1, TArg2, TArg3, TResult>(object instance, Action<InvokeBuilder> builder) {
            return InvokeGenericMethod<TArg1, TArg2, TArg3, TResult>(instance, builder.InvokeAction());
        }
        public TResult InvokeGenericMethod<TArg1, TArg2, TArg3, TResult>(object instance, string name, params object[] parameters) {
            return InvokeGenericMethod<TArg1, TArg2, TArg3, TResult>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }

        #endregion

        #region Task

        public Task InvokeGenericVoidMethodAsync(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, params object[] parameters) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            return InvokeVoidMethodAsync(instance, methodInfo, parameters);
        }

        public Task InvokeGenericVoidMethodAsync(object instance, InvokeBuilder builder, IEnumerable<Type> genericArguments) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericVoidMethodAsync(instance, methodInfo, genericArguments, parameters);
        }
        public Task InvokeGenericVoidMethodAsync(object instance, Action<InvokeBuilder> builder, IEnumerable<Type> genericArguments) {
            return InvokeGenericVoidMethodAsync(instance, builder.InvokeAction(), genericArguments);
        }
        public Task InvokeGenericVoidMethodAsync(object instance, string name, IEnumerable<Type> genericArguments, params object[] parameters) {
            return InvokeGenericVoidMethodAsync(instance, builder => builder.WithMethodName(name).WithParameters(parameters), genericArguments.ToArray());
        }




        public Task InvokeGenericVoidMethodAsync<TArg>(object instance, MethodInfo methodInfo, params object[] parameters) {
           return InvokeGenericVoidMethodAsync(instance, methodInfo, new[] { typeof(TArg) }, parameters);
        }
        public Task InvokeGenericVoidMethodAsync<TArg>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericVoidMethodAsync<TArg>(instance, methodInfo, parameters);
        }
        public Task InvokeGenericVoidMethodAsync<TArg>(object instance, Action<InvokeBuilder> builder) {
            return InvokeGenericVoidMethodAsync<TArg>(instance, builder.InvokeAction());
        }
        public Task InvokeGenericVoidMethodAsync<TArg>(object instance, string name, params object[] parameters) {
            return InvokeGenericVoidMethodAsync<TArg>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }





        public Task InvokeGenericVoidMethodAsync<TArg1, TArg2>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericVoidMethodAsync(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, parameters);
        }
        public Task InvokeGenericVoidMethodAsync<TArg1, TArg2>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericVoidMethodAsync<TArg1, TArg2>(instance, methodInfo, parameters);
        }
        public Task InvokeGenericVoidMethodAsync<TArg1, TArg2>(object instance, Action<InvokeBuilder> builder) {
            return InvokeGenericVoidMethodAsync<TArg1, TArg2>(instance, builder.InvokeAction());
        }
        public Task InvokeGenericVoidMethodAsync<TArg1, TArg2>(object instance, string name, params object[] parameters) {
            return InvokeGenericVoidMethodAsync<TArg1, TArg2>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }




        public Task InvokeGenericVoidMethodAsync<TArg1, TArg2, TArg3>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericVoidMethodAsync(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, parameters);
        }
        public Task InvokeGenericVoidMethodAsync<TArg1, TArg2, TArg3>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericVoidMethodAsync<TArg1, TArg2, TArg3>(instance, methodInfo, parameters);
        }
        public Task InvokeGenericVoidMethodAsync<TArg1, TArg2, TArg3>(object instance, Action<InvokeBuilder> builder) {
            return InvokeGenericVoidMethodAsync<TArg1, TArg2, TArg3>(instance, builder.InvokeAction());
        }
        public Task InvokeGenericVoidMethodAsync<TArg1, TArg2, TArg3>(object instance, string name, params object[] parameters) {
            return InvokeGenericVoidMethodAsync<TArg1, TArg2, TArg3>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }


        #endregion

        #region WithTaskOfT

        public Task<TResult> InvokeGenericMethodAsync<TResult>(object instance, MethodInfo methodInfo, IEnumerable<Type> genericArguments, params object[] parameters) {
            methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
            return InvokeMethodAsync<TResult>(instance, methodInfo, parameters);
        }

        public Task<TResult> InvokeGenericMethodAsync<TResult>(object instance, InvokeBuilder builder, IEnumerable<Type> genericArguments) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericMethodAsync<TResult>(instance, methodInfo, genericArguments, parameters);
        }
        public Task<TResult> InvokeGenericMethodAsync<TResult>(object instance, Action<InvokeBuilder> builder, IEnumerable<Type> genericArguments) {
            return InvokeGenericMethodAsync<TResult>(instance, builder.InvokeAction(), genericArguments);
        }
        public Task<TResult> InvokeGenericMethodAsync<TResult>(object instance, string name, IEnumerable<Type> genericArguments, params object[] parameters) {
            return InvokeGenericMethodAsync<TResult>(instance, builder => builder.WithMethodName(name).WithParameters(parameters), genericArguments.ToArray());
        }



        public Task<TResult> InvokeGenericMethodAsync<TArg, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethodAsync<TResult>(instance, methodInfo, new[] { typeof(TArg) }, parameters);
        }
        public Task<TResult> InvokeGenericMethodAsync<TArg, TResult>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericMethodAsync<TArg, TResult>(instance, methodInfo, parameters);
        }
        public Task<TResult> InvokeGenericMethodAsync<TArg, TResult>(object instance, Action<InvokeBuilder> builder) {
            return InvokeGenericMethodAsync<TArg, TResult>(instance, builder.InvokeAction());
        }
        public Task<TResult> InvokeGenericMethodAsync<TArg, TResult>(object instance, string name, params object[] parameters) {
            return InvokeGenericMethodAsync<TArg, TResult>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }




        public Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethodAsync<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2) }, parameters);
        }
        public Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TResult>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericMethodAsync<TArg1, TArg2, TResult>(instance, methodInfo, parameters);
        }
        public Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TResult>(object instance, Action<InvokeBuilder> builder) {
            return InvokeGenericMethodAsync<TArg1, TArg2, TResult>(instance, builder.InvokeAction());
        }
        public Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TResult>(object instance, string name, params object[] parameters) {
            return InvokeGenericMethodAsync<TArg1, TArg2, TResult>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }




        public Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TArg3, TResult>(object instance, MethodInfo methodInfo, params object[] parameters) {
            return InvokeGenericMethodAsync<TResult>(instance, methodInfo, new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }, parameters);
        }
        public Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TArg3, TResult>(object instance, InvokeBuilder builder) {
            builder.Context.SetOwnerType(instance);
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeGenericMethodAsync<TArg1, TArg2, TArg3, TResult>(instance, methodInfo, parameters);
        }
        public Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TArg3, TResult>(object instance, Action<InvokeBuilder> builder) {
            return InvokeGenericMethodAsync<TArg1, TArg2, TArg3, TResult>(instance, builder.InvokeAction());
        }
        public Task<TResult> InvokeGenericMethodAsync<TArg1, TArg2, TArg3, TResult>(object instance, string name, params object[] parameters) {
            return InvokeGenericMethodAsync<TArg1, TArg2, TArg3, TResult>(instance, builder => builder.WithMethodName(name).WithParameters(parameters));
        }

        #endregion

        #endregion

    }

    public class MethodManager : MethodManager<DefaultMethodBox> {

        public MethodManager() : base() {
            SetMethodBoxBuilderAction(Builderfunction);
        }

        public MethodManager(MethodManagerOptions options) : base(options) {
            SetMethodBoxBuilderAction(Builderfunction);
        }

        public MethodManager(Action<MethodManagerOptionsBuilder> builder = null) : base(builder) {
            SetMethodBoxBuilderAction(Builderfunction);
        }

        private void Builderfunction(MethodBoxBuilderContext options, MethodBoxBuilderResult<DefaultMethodBox> result) {
            result.MethodInfoBox = options.MethodInfo;
        }

    }
}
