using System.Reflection;
using System.Threading.Tasks;

namespace Reflectensions.ExtensionMethods {
    public static class TaskExtensions {

        public static async Task<TResult> ConvertToTaskOf<TResult>(this Task task, bool throwOnError = true, TResult returnOnError = default(TResult)) {

            if (task == null)
                return returnOnError;

            return await task.ContinueWith(t => {
                return t.GetPropertyValue<TResult>("Result");
            });
            
        }

    }
}
