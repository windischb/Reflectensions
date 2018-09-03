using System;

namespace Reflectensions.ExtensionMethods
{
    public static class ActionExtensions
    {
        public static T InvokeAction<T>(this Action<T> action, T instance = default(T)) {
            if (instance == null) {
                instance = Activator.CreateInstance<T>();
            }

            action(instance);
            return instance;
        }

        public static (T1, T2) InvokeAction<T1, T2>(this Action<T1, T2> action, T1 firstInstance = default(T1), T2 secondInstance = default(T2)) {

            if (firstInstance == null) {
                firstInstance = Activator.CreateInstance<T1>();
            }

            if (secondInstance == null) {
                secondInstance = Activator.CreateInstance<T2>();
            }

            action(firstInstance, secondInstance);
            return (firstInstance, secondInstance);
        }

        public static (T1, T2, T3) InvokeAction<T1, T2, T3>(this Action<T1, T2, T3> action, T1 firstInstance = default(T1), T2 secondInstance = default(T2), T3 thirdInstance = default(T3)) {

            if (firstInstance == null) {
                firstInstance = Activator.CreateInstance<T1>();
            }

            if (secondInstance == null) {
                secondInstance = Activator.CreateInstance<T2>();
            }

            if (thirdInstance == null) {
                thirdInstance = Activator.CreateInstance<T3>();
            }

            action(firstInstance, secondInstance, thirdInstance);
            return (firstInstance, secondInstance, thirdInstance);
        }

    }
}
