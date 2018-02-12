using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using doob.Reflectensions.ExtensionMethods;

namespace doob.Reflectensions {
    public class MethodManager<TBox> where TBox : IMethodBox, new() {

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

        private Action<MethodBoxBuilderContext, MethodBoxBuilderResult<TBox>> _methodBoxBuilderAction;

        private static object[] BuildParametersArray(MethodInfo methodInfo, IEnumerable<object> parameters) {
            var enumerable = parameters as object[] ?? parameters.ToArray();

            var pa = methodInfo.GetParameters().Select((p, i) => {

                if (enumerable.Length > i) {
                    return enumerable[i] == null ? null : enumerable[i].CastTo(p.ParameterType);
                }

                return p.DefaultValue;
            }).ToArray();

            return pa;
        }

        private TBox FindMethod(MethodSearch search) {
            var bindingFlags = search.Context.AccessModifier.ToBindingFlags().Add(MethodType.Instance.ToBindingFlags());
            var methodInfo = search.Context.OwnerType.FoundMatchingType.GetMethods(bindingFlags).FindBestMatchingMethodInfo(search);
            return BuildMethod(MethodBoxBuilderContext.Build(methodInfo, Options)).MethodInfoBox;
        }

        private TBox FindCachedMethod(MethodSearch search) {

            if (Options.EnableCache) {
                return _methodInfoBoxCache.GetOrAdd(search, () => FindMethod(search));
            } else {
                return FindMethod(search);
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

        public void SetMethodBoxBuilderAction(Action<MethodBoxBuilderContext, MethodBoxBuilderResult<TBox>> builderfunction) {
            _methodBoxBuilderAction = builderfunction;
        }


        private (MethodInfo MethodInfo, object[] Parameters) GetMethodInfo(InvokeBuilder builder) {
            var methodInfoBox = FindCachedMethod(builder.Context);
            MethodInfo methodInfo = methodInfoBox.MethodInfo;
            if (builder.Context.Context.GenericArguments.Any()) {
                methodInfo = methodInfo.MakeGenericMethod(builder.Context.Context.GenericArguments.Select(a => a.FoundMatchingType).ToArray());
            }

            return (methodInfo, BuildParametersArray(methodInfo, builder.Context.Parameters));
        }

        #region Non-Generic Methods


        public void InvokeMethod(object @object, MethodInfo methodInfo, IEnumerable<object> parameters) {

            var enumerable = BuildParametersArray(methodInfo, parameters);
            var isTaskVoid = methodInfo.ReturnType == typeof(Task);
            var isTaskReturn = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

            if (isTaskVoid || isTaskReturn) {
                ((Task)methodInfo.Invoke(@object, enumerable)).Wait();
                return;
            }

            methodInfo.Invoke(@object, enumerable);
        }
        public T InvokeMethod<T>(object @object, MethodInfo methodInfo, IEnumerable<object> parameters) {
            var enumerable = BuildParametersArray(methodInfo, parameters);
            var isTaskReturn = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

            var returnType = methodInfo.ReturnType;
            if (isTaskReturn) {
                returnType = methodInfo.ReturnType.GenericTypeArguments[0];
            }

            if (returnType.NotEquals<T>() && !returnType.InheritFromClass<T>() && !returnType.ImplementsInterface<T>(false) && !returnType.IsImplicitCastableTo<T>())
                throw new Exception($"Method returns a Type of '{methodInfo.ReturnType}' which is not implicitly castable to {typeof(T)}");

            object returnObject;

            if (isTaskReturn) {
                var task = ((Task)methodInfo.Invoke(@object, enumerable));
                task.Wait();

                var resultProperty = typeof(Task<>).MakeGenericType(methodInfo.ReturnType.GetGenericArguments().FirstOrDefault()).GetProperty("Result");
                returnObject = resultProperty.GetValue(task);
            } else {
                returnObject = methodInfo.Invoke(@object, enumerable);
            }

            return returnObject.CastTo<T>();
        }
        public async Task InvokeMethodAsync(object @object, MethodInfo methodInfo, IEnumerable<object> parameters) {
            var enumerable = BuildParametersArray(methodInfo, parameters);
            var isTaskVoid = methodInfo.ReturnType == typeof(Task);
            var isTaskReturn = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

            if (isTaskVoid || isTaskReturn) {
                await (Task)methodInfo.Invoke(@object, enumerable);
                return;
            }

            await Task.Run(() => methodInfo.Invoke(@object, enumerable));

        }
        public async Task<T> InvokeMethodAsync<T>(object @object, MethodInfo methodInfo, IEnumerable<object> parameters) {
            var enumerable = BuildParametersArray(methodInfo, parameters);

            var isTaskReturn = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

            var returnType = methodInfo.ReturnType;
            if (isTaskReturn) {
                returnType = methodInfo.ReturnType.GenericTypeArguments[0];
            }

            if (returnType.NotEquals<T>() && !returnType.InheritFromClass<T>() && !returnType.ImplementsInterface<T>(false) && !returnType.IsImplicitCastableTo<T>())
                throw new Exception($"Method returns a Type of '{methodInfo.ReturnType}' which is not implicitly castable to {typeof(T)}");

            object returnObject;

            if (isTaskReturn) {

                var task = (Task)methodInfo.Invoke(@object, enumerable);
                await task;
                var resultProperty = typeof(Task<>).MakeGenericType(methodInfo.ReturnType.GetGenericArguments().FirstOrDefault()).GetProperty("Result");
                returnObject = resultProperty.GetValue(task);
            } else {
                returnObject = await Task.Run(() => methodInfo.Invoke(@object, enumerable));
            }

            return returnObject.CastTo<T>();

        }


        public void InvokeMethod(object @object, InvokeBuilder builder) {
            builder.Context.SetOwnerType(@object.GetType());
            var (methodInfo, parameters) = GetMethodInfo(builder);
            InvokeMethod(@object, methodInfo, parameters);
        }
        public void InvokeMethod(object @object, Action<InvokeBuilder> builder) {
            var invokeBuilder = new InvokeBuilder();
            builder(invokeBuilder);
            InvokeMethod(@object, invokeBuilder);
        }
        public void InvokeMethod(object @object, string name, params object[] parameters) {
            InvokeMethod(@object, builder => builder.WithMethodName(name).WithParameters(parameters));
        }

        public T InvokeMethod<T>(object @object, InvokeBuilder builder) {
            builder.Context.SetOwnerType(@object.GetType());
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return InvokeMethod<T>(@object, methodInfo, parameters);
        }
        public T InvokeMethod<T>(object @object, Action<InvokeBuilder> builder) {
            var invokeBuilder = new InvokeBuilder();
            builder(invokeBuilder);
            return InvokeMethod<T>(@object, invokeBuilder);
        }
        public T InvokeMethod<T>(object @object, string name, params object[] parameters) {
            return InvokeMethod<T>(@object, builder => builder.WithMethodName(name).WithParameters(parameters));
        }

        public async Task InvokeMethodAsync(object @object, InvokeBuilder builder) {
            builder.Context.SetOwnerType(@object.GetType());
            var (methodInfo, parameters) = GetMethodInfo(builder);
            await InvokeMethodAsync(@object, methodInfo, parameters);
        }
        public async Task InvokeMethodAsync(object @object, Action<InvokeBuilder> builder) {
            var invokeBuilder = new InvokeBuilder();
            builder(invokeBuilder);
            await InvokeMethodAsync(@object, invokeBuilder);
        }
        public async Task InvokeMethodAsync(object @object, string name, params object[] parameters) {
            await InvokeMethodAsync(@object, builder => builder.WithMethodName(name).WithParameters(parameters));
        }

        public async Task<T> InvokeMethodAsync<T>(object @object, InvokeBuilder builder) {
            builder.Context.SetOwnerType(@object.GetType());
            var (methodInfo, parameters) = GetMethodInfo(builder);
            return await InvokeMethodAsync<T>(@object, methodInfo, parameters);
        }
        public async Task<T> InvokeMethodAsync<T>(object @object, Action<InvokeBuilder> builder) {
            var invokeBuilder = new InvokeBuilder();
            builder(invokeBuilder);
            return await InvokeMethodAsync<T>(@object, invokeBuilder);
        }
        public async Task<T> InvokeMethodAsync<T>(object @object, string name, params object[] parameters) {
            return await InvokeMethodAsync<T>(@object, builder => builder.WithMethodName(name).WithParameters(parameters));
        }

        #endregion

        //#region Generic Methods

        //public void InvokeGenericMethod(object @object, MethodInfo methodInfo, IEnumerable<Type> genericArguments, IEnumerable<object> parameters) {
        //    methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
        //    InvokeMethod(@object, methodInfo, parameters);
        //}
        //public void InvokeGenericMethod(object @object, string name, IEnumerable<Type> genericArguments, IEnumerable<object> parameters, MethodAccessModifier bindingFlags) {
        //    var enumerable = parameters as object[] ?? parameters.ToArray();
        //    var search = MethodSearch.Create().SetOwnerType(@object.GetType()).SetMethodName(name).SetParameterTypes(enumerable).SetAccessModifier(bindingFlags);
        //    var methodInfo = FindInstanceMethod(search);
        //    InvokeGenericMethod(@object, methodInfo.MethodInfo, genericArguments, enumerable);
        //}
        //public void InvokeGenericMethod(object @object, string name, IEnumerable<Type> genericArguments, params object[] parameters) {
        //    InvokeGenericMethod(@object, name, genericArguments, parameters, DefaultAccessModifier);
        //}




        //public void InvokeGenericMethod<TArg>(object @object, MethodInfo methodInfo, IEnumerable<object> parameters) {
        //    InvokeGenericMethod(@object, methodInfo, new ListOfTypes<TArg>(), parameters);
        //}
        //public void InvokeGenericMethod<TArg>(object @object, string name, IEnumerable<object> parameters, MethodAccessModifier bindingFlags) {
        //    var enumerable = parameters as object[] ?? parameters.ToArray();
        //    var search = MethodSearch.Create().SetOwnerType(@object.GetType()).SetMethodName(name).SetParameterTypes(enumerable).SetAccessModifier(bindingFlags);
        //    var methodInfo = FindInstanceMethod(search);
        //    InvokeGenericMethod<TArg>(@object, methodInfo.MethodInfo, enumerable);
        //}
        //public void InvokeGenericMethod<TArg>(object @object, string name, params object[] parameters) {
        //    InvokeGenericMethod<TArg>(@object, name, parameters, DefaultAccessModifier);
        //}



        //public TResult InvokeGenericMethod<TResult>(object @object, MethodInfo methodInfo, IEnumerable<Type> genericArguments, IEnumerable<object> parameters) {
        //    methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
        //    return InvokeMethod<TResult>(@object, methodInfo, parameters);
        //}
        //public TResult InvokeGenericMethod<TResult>(object @object, string name, IEnumerable<Type> genericArguments, IEnumerable<object> parameters, MethodAccessModifier bindingFlags) {
        //    var enumerable = parameters as object[] ?? parameters.ToArray();
        //    var search = MethodSearch.Create().SetOwnerType(@object.GetType()).SetMethodName(name).SetParameterTypes(enumerable).SetAccessModifier(bindingFlags);
        //    var methodInfo = FindInstanceMethod(search);
        //    return InvokeGenericMethod<TResult>(@object, methodInfo.MethodInfo, genericArguments, enumerable);
        //}
        //public TResult InvokeGenericMethod<TResult>(object @object, string name, IEnumerable<Type> genericArguments, params object[] parameters) {
        //    return InvokeGenericMethod<TResult>(@object, name, genericArguments, parameters, DefaultAccessModifier);
        //}

        //public TResult InvokeGenericMethod<TResult, TArg>(object @object, MethodInfo methodInfoBox, IEnumerable<object> parameters) {
        //    return InvokeGenericMethod<TResult>(@object, methodInfoBox, new ListOfTypes<TArg>(), parameters);
        //}
        //public TResult InvokeGenericMethod<TResult, TArg>(object @object, string name, IEnumerable<object> parameters, MethodAccessModifier bindingFlags) {
        //    var enumerable = parameters as object[] ?? parameters.ToArray();
        //    var search = MethodSearch.Create().SetOwnerType(@object.GetType()).SetMethodName(name).SetParameterTypes(enumerable).SetAccessModifier(bindingFlags);
        //    var methodInfo = FindInstanceMethod(search);
        //    return InvokeGenericMethod<TResult, TArg>(@object, methodInfo.MethodInfo, enumerable);
        //}
        //public TResult InvokeGenericMethod<TResult, TArg>(object @object, string name, params object[] parameters) {
        //    return InvokeGenericMethod<TResult, TArg>(@object, name, parameters, DefaultAccessModifier);
        //}



        //public async Task InvokeGenericMethodAsync(object @object, MethodInfo methodInfo, IEnumerable<Type> genericArguments, IEnumerable<object> parameters) {
        //    methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
        //    await InvokeMethodAsync(@object, methodInfo, parameters);
        //}
        //public async Task InvokeGenericMethodAsync(object @object, string name, IEnumerable<Type> genericArguments, IEnumerable<object> parameters, MethodAccessModifier bindingFlags) {
        //    var enumerable = parameters as object[] ?? parameters.ToArray();
        //    var search = MethodSearch.Create().SetOwnerType(@object.GetType()).SetMethodName(name).SetParameterTypes(enumerable).SetAccessModifier(bindingFlags);
        //    var methodInfo = FindInstanceMethod(search);
        //    await InvokeGenericMethodAsync(@object, methodInfo.MethodInfo, genericArguments, enumerable);
        //}
        //public async Task InvokeGenericMethodAsync(object @object, string name, IEnumerable<Type> genericArguments, params object[] parameters) {
        //    await InvokeGenericMethodAsync(@object, name, genericArguments, parameters, DefaultAccessModifier);
        //}

        //public async Task InvokeGenericMethodAsync<TArg>(object @object, MethodInfo methodInfoBox, IEnumerable<object> parameters) {
        //    await InvokeGenericMethodAsync(@object, methodInfoBox, new ListOfTypes<TArg>(), parameters);
        //}
        //public async Task InvokeGenericMethodAsync<TArg>(object @object, string name, IEnumerable<object> parameters, MethodAccessModifier bindingFlags) {
        //    var enumerable = parameters as object[] ?? parameters.ToArray();
        //    var search = MethodSearch.Create().SetOwnerType(@object.GetType()).SetMethodName(name).SetParameterTypes(enumerable).SetAccessModifier(bindingFlags);
        //    var methodInfo = FindInstanceMethod(search);
        //    await InvokeGenericMethodAsync<TArg>(@object, methodInfo.MethodInfo, enumerable);
        //}
        //public async Task InvokeGenericMethodAsync<TArg>(object @object, string name, params object[] parameters) {
        //    await InvokeGenericMethodAsync<TArg>(@object, name, parameters, DefaultAccessModifier);
        //}

        //public async Task<TResult> InvokeGenericMethodAsync<TResult>(object @object, MethodInfo methodInfo, IEnumerable<Type> genericArguments, IEnumerable<object> parameters) {
        //    methodInfo = methodInfo.MakeGenericMethod(genericArguments.ToArray());
        //    return await InvokeMethodAsync<TResult>(@object, methodInfo, parameters);
        //}
        //public async Task<TResult> InvokeGenericMethodAsync<TResult>(object @object, string name, IEnumerable<Type> genericArguments, IEnumerable<object> parameters, MethodAccessModifier bindingFlags) {
        //    var enumerable = parameters as object[] ?? parameters.ToArray();
        //    var search = MethodSearch.Create().SetOwnerType(@object.GetType()).SetMethodName(name).SetParameterTypes(enumerable).SetAccessModifier(bindingFlags);
        //    var methodInfo = FindInstanceMethod(search);
        //    return await InvokeGenericMethodAsync<TResult>(@object, methodInfo.MethodInfo, genericArguments, enumerable);
        //}
        //public async Task<TResult> InvokeGenericMethodAsync<TResult>(object @object, string name, IEnumerable<Type> genericArguments, params object[] parameters) {
        //    return await InvokeGenericMethodAsync<TResult>(@object, name, genericArguments, parameters, DefaultAccessModifier);
        //}

        //public async Task<TResult> InvokeGenericMethodAsync<TResult, TArg>(object @object, MethodInfo methodInfoBox, IEnumerable<object> parameters) {
        //    return await InvokeGenericMethodAsync<TResult>(@object, methodInfoBox, new ListOfTypes<TArg>(), parameters);
        //}
        //public async Task<TResult> InvokeGenericMethodAsync<TResult, TArg>(object @object, string name, IEnumerable<object> parameters, MethodAccessModifier bindingFlags) {
        //    var enumerable = parameters as object[] ?? parameters.ToArray();
        //    var search = MethodSearch.Create().SetOwnerType(@object.GetType()).SetMethodName(name).SetParameterTypes(enumerable).SetAccessModifier(bindingFlags);
        //    var methodInfo = FindInstanceMethod(search);
        //    return await InvokeGenericMethodAsync<TResult, TArg>(@object, methodInfo.MethodInfo, enumerable);
        //}
        //public async Task<TResult> InvokeGenericMethodAsync<TResult, TArg>(object @object, string name, params object[] parameters) {
        //    return await InvokeGenericMethodAsync<TResult, TArg>(@object, name, parameters, DefaultAccessModifier);
        //}


        //#endregion
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
