using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;

namespace Reflectensions.Internal {
    /// <summary>
    /// !!! do not use directly !!! -> use ExpandableObject
    /// </summary>
    public abstract class ExpandableBaseObject : DynamicObject {

        private readonly object _instance;
        private readonly Type _instanceType;

        private PropertyInfo[] _instancePropertyInfo;
        private PropertyInfo[] InstancePropertyInfo {
            get {
                if (_instancePropertyInfo == null && _instance != null) {
                    _instancePropertyInfo = _instance
                        .GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase)
                        .ToArray();
                }
                return _instancePropertyInfo;
            }
        }


        // ReSharper disable once InconsistentNaming
        public Dictionary<string, object> __Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);


        protected ExpandableBaseObject() {
            _instance = this;
            _instanceType = _instance.GetType();

        }


        protected ExpandableBaseObject(object instance) {
            _instance = instance;
            if (instance != null) {
                _instanceType = _instance.GetType();
            }

        }


        public override IEnumerable<string> GetDynamicMemberNames() {
            foreach (var prop in GetProperties()) {
                yield return prop.Key;
            }
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result) {

            if (__Properties.Keys.Contains(binder.Name)) {
                result = __Properties[binder.Name];
                return true;
            }

            if (_instance != null) {
                try {
                    if (TryGetProperty(_instance, binder.Name, out result)) {
                        return true;
                    }
                } catch {
                    // ignored
                }
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {

            if (_instance != null) {
                try {
                    if (TrySetProperty(_instance, binder.Name, value)) {
                        return true;
                    }
                } catch {
                    // ignored
                }
            }

            __Properties[binder.Name] = value;
            return true;
        }


        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
            if (_instance != null) {
                try {
                    if (TryInvokeMethod(_instance, binder.Name, args, out result)) {
                        return true;
                    }
                } catch {
                    // ignored
                }
            }

            result = null;
            return false;
        }


        protected bool TryGetProperty(object instance, string name, out object result) {

            instance = instance ?? this;

            var memberInfo = _instanceType
                .GetMember(name, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.IgnoreCase).FirstOrDefault();

            if (memberInfo?.MemberType == MemberTypes.Property) {
                result = ((PropertyInfo)memberInfo).GetValue(instance, null);
                return true;
            }

            result = null;
            return false;
        }


        protected bool TrySetProperty(object instance, string name, object value) {

            instance = instance ?? this;

            var memberInfo = _instanceType
                .GetMember(name, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.IgnoreCase).FirstOrDefault();

            if (memberInfo?.MemberType == MemberTypes.Property) {
                ((PropertyInfo)memberInfo).SetValue(instance, value, null);
                return true;
            }

            return false;
        }


        protected bool TryInvokeMethod(object instance, string name, object[] args, out object result) {

            instance = instance ?? this;

            var memberInfo = _instanceType
                .GetMember(name, BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.IgnoreCase).FirstOrDefault();

            if (memberInfo?.MemberType == MemberTypes.Method) {
                result = ((MethodInfo)memberInfo).Invoke(instance, args);
                return true;
            }

            result = null;
            return false;
        }


        public object this[string key] {
            get {

                if (TryGetProperty(_instance, key, out var result)) {
                    return result;
                }

                if (__Properties.TryGetValue(key, out var prop)) {
                    return prop;
                }

                throw new KeyNotFoundException(key);
            }
            set {

                if (!TrySetProperty(_instance, key, value)) {
                    __Properties[key] = value;
                }

            }
        }


        public IEnumerable<KeyValuePair<string, object>> GetProperties(bool includeInstanceProperties = true) {

            if (includeInstanceProperties && _instance != null) {
                foreach (var prop in InstancePropertyInfo) {
                    yield return new KeyValuePair<string, object>(prop.Name, prop.GetValue(_instance, null));
                }
            }

            foreach (var key in __Properties.Keys) {
                yield return new KeyValuePair<string, object>(key, __Properties[key]);
            }

        }


        public bool Contains(KeyValuePair<string, object> item, bool includeInstanceProperties = false) {
            var res = __Properties.ContainsKey(item.Key);
            if (res) {
                return true;
            }


            if (includeInstanceProperties && _instance != null) {
                foreach (var prop in InstancePropertyInfo) {
                    if (prop.Name == item.Key) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ContainsKey(string key, bool includeInstanceProperties = false) {

            var res = __Properties.ContainsKey(key);
            if (res) {
                return true;
            }

            if (includeInstanceProperties && _instance != null) {
                foreach (var prop in InstancePropertyInfo) {
                    if (prop.Name == key) {
                        return true;
                    }
                }
            }

            return false;

        }

        public bool IsInstanceProperty(string key) {

            foreach (var prop in InstancePropertyInfo) {
                if (prop.Name == key)
                    return true;
            }

            return false;
        }


        public IEnumerable<object> GetValues() {
            return GetProperties().Select(p => p.Value);
        }

        public IEnumerable<string> GetKeys() {
            return GetProperties().Select(p => p.Key);
        }

        public T GetValue<T>(string key) {
            if (TryGetProperty(_instance, key, out var result)) {
                return (T)result;
            }

            if (__Properties.TryGetValue(key, out var prop)) {
                return (T)prop;
            }

            throw new KeyNotFoundException(key);
        }

        public object GetValue(string key) {
            return GetValue<object>(key);
        }

        public object GetValueOrDefault(string key, object defaultValue = default) {
            return GetValueOrDefault<object>(key, defaultValue);
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default) {
            if (TryGetProperty(_instance, key, out var result)) {
                return (T)result;
            }

            if (__Properties.TryGetValue(key, out var prop)) {
                return (T)prop;
            }

            return defaultValue;
        }

        public List<T> GetValuesOrDefault<T>(string key) {
            var list = new List<T>();

            if (!TryGetProperty(_instance, key, out var result)) {
                if (!__Properties.TryGetValue(key, out result)) {
                    return list;
                }
            }

            if (result is List<T> resultList) {
                return resultList;
            }

            if (result is IEnumerable ienum) {
                foreach (var o in ienum) {
                    if (o.TryTo<T>(out var t)) {
                        list.Add(t);
                    } else {
                        if (o.TryTo<T>(out t)) {
                            list.Add(t);
                        }
                    }

                }
            }

            return list;
        }

        //public T GetValueOrDefault<T>(string key, T defaultValue = default)
        //{
        //    var found = false;
        //    object _result = null;
        //    if (TryGetProperty(_instance, key, out var result))
        //    {
        //        found = true;
        //        _result = result;
        //    }

        //    if (!found)
        //    {
        //        if (__Properties.TryGetValue(key, out var prop))
        //        {
        //            found = true;
        //            _result = prop;
        //        }
        //    }

        //    if (!found)
        //    {
        //        return default;
        //    }

        //    var typeOfT = typeof(T);

        //    if (_result.GetType() == typeOfT)
        //    {
        //        return (T)_result;
        //    }

        //    if (typeOfT == typeof(ExpandableObject) || typeOfT.IsSubclassOf(typeof(ExpandableObject)))
        //    {
        //        return (T)ToExpandableObject(typeOfT, _result);
        //    }

        //    return defaultValue;
        //}

        //public T GetValuesOrDefault<T>(string key, T defaultValue = default) where T: ICollection
        //{
        //    var found = false;
        //    object _result = null;
        //    if (TryGetProperty(_instance, key, out var result))
        //    {
        //        found = true;
        //        _result = result;
        //    }

        //    if (!found)
        //    {
        //        if (__Properties.TryGetValue(key, out var prop))
        //        {
        //            found = true;
        //            _result = prop;
        //        }
        //    }

        //    if (!found)
        //    {
        //        return default;
        //    }

        //    var typeOfT = typeof(T);

        //    if (_result.GetType() == typeof(T))
        //    {
        //        return (T)_result;
        //    }

        //    if(typeOfT.gener)
        //    if (_result is IEnumerable)
        //    {

        //    }

        //    return defaultValue;
        //}


        private object ToExpandableObject(Type expandableObjectType, object @object) {



            if (@object is IDictionary<string, object> dict) {
                var ne = (ExpandableObject)Activator.CreateInstance(expandableObjectType);
                foreach (var keyValuePair in dict) {
                    ne[keyValuePair.Key] = keyValuePair.Value;
                }

                return ne;
            } else if (@object is ExpandableObject exp) {
                var ne = (ExpandableObject)Activator.CreateInstance(expandableObjectType);
                foreach (var keyValuePair in exp.AsDictionary()) {
                    ne[keyValuePair.Key] = keyValuePair.Value;
                }

                return ne;
            } else {
                var ne = (ExpandableObject)Activator.CreateInstance(expandableObjectType, @object);
                return ne;
            }
        }

        //public TExp GetValueOrDefault<TExp>(string key) where TExp : ExpandableObject
        //{
        //    var found = false;
        //    object _result = null;
        //    if (TryGetProperty(_instance, key, out var result))
        //    {
        //        found = true;
        //        _result = result;
        //    }

        //    if (!found)
        //    {
        //        if (__Properties.TryGetValue(key, out var prop))
        //        {
        //            found = true;
        //            _result = prop;
        //        }
        //    }


        //    if (typeof(TExp) == typeof(ExpandableObject) || typeof(TExp).IsSubclassOf(typeof(ExpandableObject)))
        //    {
        //        if (_result is IDictionary<string, object> dict)
        //        {
        //            var ne = (ExpandableObject)Activator.CreateInstance(typeof(TExp));
        //            foreach (var keyValuePair in dict)
        //            {
        //                ne[keyValuePair.Key] = keyValuePair.Value;
        //            }

        //            return (TExp)ne;
        //        }
        //        else if(_result is ExpandableObject exp)
        //        {
        //            var ne = (ExpandableObject)Activator.CreateInstance(typeof(TExp));
        //            foreach (var keyValuePair in exp.AsDictionary())
        //            {
        //                ne[keyValuePair.Key] = keyValuePair.Value;
        //            }

        //            return (TExp)ne;
        //        }


        //    }

        //    return null;
        //}

    }
}
