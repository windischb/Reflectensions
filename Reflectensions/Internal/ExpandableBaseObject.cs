using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Reflectensions.Annotations;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;

namespace Reflectensions.Internal {
    /// <summary>
    /// !!! do not use directly !!! -> use ExpandableObject
    /// </summary>
    public abstract partial class ExpandableBaseObject : DynamicObject, INotifyPropertyChanged {

        private readonly object _instance;
        private readonly Type _instanceType;

        private PropertyInfo[] _instancePropertyInfo;
        private PropertyInfo[] InstancePropertyInfo {
            get {
                if (_instancePropertyInfo == null && _instance != null) {
                    _instancePropertyInfo = _instance
                        .GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                        .Where(p => p.DeclaringType != typeof(ExpandableBaseObject))
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
                        RaisePropertyChanged(binder.Name);
                        return true;
                    }
                } catch {
                    // ignored
                }
            }

            __Properties[binder.Name] = value;
            RaisePropertyChanged(binder.Name);
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
                var propInfo = (PropertyInfo)memberInfo;
                if (value != null) {
                    
                    if (propInfo.PropertyType != value.GetType()) {
                        value.TryTo(propInfo.PropertyType, out value);
                    }
                }
                propInfo.SetValue(instance, value, null);
                RaisePropertyChanged(name);
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
                RaisePropertyChanged(key);
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
                return result.To<T>();
            }

            if (__Properties.TryGetValue(key, out var prop)) {
                return prop.To<T>();
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


       

        protected virtual bool SetPropertyChanged<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        protected virtual bool SetPropertyChanged<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);

            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            //TODO: when we remove the old OnPropertyChanged method we need to uncomment the below line
            //OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
#pragma warning disable CS0618 // Type or member is obsolete
            OnPropertyChanged(propertyName);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
