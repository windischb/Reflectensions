using System;
using Reflectensions.Logging;

namespace Reflectensions.Helpers
{
    public static class ExceptionHelper
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
        public static bool IgnoreErrors(Action operation)
        {
            if (operation == null)
                return false;
            try
            {
                operation.Invoke();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }

            return true;
        }

        public static T IgnoreErrors<T>(Func<T> operation, T defaultValue = default)
        {
            if (operation == null)
                return defaultValue;

            T result;
            try
            {
                result = operation.Invoke();
            }
            catch (Exception ex) {
                Logger.Error(ex, ex.Message);
                result = defaultValue;
            }

            return result;
        }
    }
}
