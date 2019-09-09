using System;
using Reflectensions.Helper;

namespace Reflectensions.ExtensionMethods
{
    public static class ActionExtensions
    {
        public static T InvokeAction<T>(this Action<T> action, T instance = default) => ActionHelpers.InvokeAction(action, instance);
        public static (T1, T2) InvokeAction<T1, T2>(this Action<T1, T2> action, T1 firstInstance = default, T2 secondInstance = default) => ActionHelpers.InvokeAction(action, firstInstance, secondInstance);
        public static (T1, T2, T3) InvokeAction<T1, T2, T3>(this Action<T1, T2, T3> action, T1 firstInstance = default, T2 secondInstance = default, T3 thirdInstance = default) => ActionHelpers.InvokeAction(action, firstInstance, secondInstance, thirdInstance);

    }
}
