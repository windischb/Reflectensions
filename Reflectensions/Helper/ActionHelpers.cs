using System;

namespace Reflectensions.Helper
{
    public static class ActionHelpers
    {
        public static T InvokeAction<T>(Action<T> action, T instance = default) {
            if (instance == null) {
                instance = Activator.CreateInstance<T>();
            }

            action(instance);
            return instance;
        }

        public static (T1, T2) InvokeAction<T1, T2>(Action<T1, T2> action, T1 firstInstance = default, T2 secondInstance = default) {

            if (firstInstance == null) {
                firstInstance = Activator.CreateInstance<T1>();
            }

            if (secondInstance == null) {
                secondInstance = Activator.CreateInstance<T2>();
            }

            action(firstInstance, secondInstance);
            return (firstInstance, secondInstance);
        }

        public static (T1, T2, T3) InvokeAction<T1, T2, T3>(Action<T1, T2, T3> action, T1 firstInstance = default, T2 secondInstance = default, T3 thirdInstance = default) {

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
