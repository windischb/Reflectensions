using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflectensions.ExtensionMethods {
    public static class TaskExtensions {

        public static async Task<IEnumerable<TSource>> WhereAsync<TSource>(this Task<IEnumerable<TSource>> source, Func<TSource, bool> predicate) {
            var ie = await source;
            return ie.Where(predicate);
        }

        public static async Task<IEnumerable<TSource>> WhereAsync<TSource>(this Task<List<TSource>> source, Func<TSource, bool> predicate) {
            var ie = await source;
            return ie.Where(predicate);
        }

        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this Task<IEnumerable<TSource>> source, Func<TSource, TResult> selector) {
            var ie = await source;
            return ie.Select(selector);
        }

        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this Task<List<TSource>> source, Func<TSource, TResult> selector) {
            var ie = await source;
            return ie.Select(selector);
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this Task<IEnumerable<T>> source, Func<T, bool> predicate) {
            var ie = await source;
            return ie.FirstOrDefault(predicate);
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this Task<List<T>> source, Func<T, bool> predicate) {
            var ie = await source;
            return ie.FirstOrDefault(predicate);
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this Task<IEnumerable<T>> source) {
            var ie = await source;
            return ie.FirstOrDefault();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this Task<List<T>> source) {
            var ie = await source;
            return ie.FirstOrDefault();
        }

        public static async Task<bool> AnyAsync<T>(this Task<IEnumerable<T>> source, Func<T, bool> predicate) {
            var ie = await source;
            return ie.Any(predicate);
        }

        public static async Task<bool> AnyAsync<T>(this Task<List<T>> source, Func<T, bool> predicate) {
            var ie = await source;
            return ie.Any(predicate);
        }

        public static async Task<bool> AnyAsync<T>(this Task<IEnumerable<T>> source) {
            var ie = await source;
            return ie.Any();
        }

        public static async Task<bool> AnyAsync<T>(this Task<List<T>> source) {
            var ie = await source;
            return ie.Any();
        }

        public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> source) {
            var ie = await source;
            return ie.ToList();
        }

        public static async Task<TResult> ConvertToTaskOf<TResult>(this Task task) {

            if (task == null)
                return default;

            task.GetAwaiter().GetResult();
            var obj = task.GetType()?.GetProperty("Result")?.GetValue(task);

            if (obj == null)
                return default;

            return await Task.FromResult(obj.ConvertTo<TResult>());
        }

    }
}
