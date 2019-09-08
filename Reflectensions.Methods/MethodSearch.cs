using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;
using Reflectensions.Helpers;

namespace Reflectensions
{
    public class MethodSearch
    {
        public MethodSearchContext Context { get; internal set; } = new MethodSearchContext();

        internal MethodSearch() {

            Context.AccessModifier = MethodAccessModifier.Any;
        }

        public static MethodSearch Create() {
            return new MethodSearch();
        }

        public MethodSearch SetOwnerType(string typeName) {
            Context.SearchFor = Context.SearchFor | MethodSearchFlags.OwnerType;
            Context.OwnerType = SignatureType.FromTypeString(typeName);
            if (Context.OwnerType.IsStatic) {
                SetMethodType(MethodType.Static);
            }
            return this;
        }
        public MethodSearch SetOwnerType(object instance) {
            if (instance == null)
                return this;

            if (instance is Type type) {
                return SetOwnerType(type);
            }
            return SetOwnerType(instance.GetType());
        }
        public MethodSearch SetOwnerType(Type type) {
            if (type == null)
                return this;

            Context.SearchFor = Context.SearchFor | MethodSearchFlags.OwnerType;
            Context.OwnerType = SignatureType.FromType(type);
            if (Context.OwnerType.IsStatic) {
                SetMethodType(MethodType.Static);
            }
            return this;
        }

        public MethodSearch SetOwnerAlias(string alias) {
            Context.SearchFor = Context.SearchFor | MethodSearchFlags.OwnerAlias;
            Context.OwnerTypeAlias = alias;
            return this;
        }

        public MethodSearch SetMethodType(string methodType) {
            return SetMethodType(Enum<MethodType>.Find(methodType));
        }
        public MethodSearch SetMethodType(MethodType type) {
            Context.SearchFor = Context.SearchFor | MethodSearchFlags.MethodType;
            Context.MethodType = type;
            return this;
        }
        
        public MethodSearch SetAccessModifier(string accessModifier) {
            MethodAccessModifier _enum = MethodAccessModifier.Unknown;

            if (accessModifier != null) {
                _enum = Enum<MethodAccessModifier>.Find(accessModifier);
                if (accessModifier.Contains(" ") && _enum == MethodAccessModifier.Unknown) {
                    accessModifier = string.Join(" ", accessModifier.Split(' ').Reverse());
                    _enum = Enum<MethodAccessModifier>.Find(accessModifier);
                }
            }

            return SetAccessModifier(_enum);
        }
        public MethodSearch SetAccessModifier(MethodAccessModifier accessModifier) {
            Context.SearchFor = Context.SearchFor | MethodSearchFlags.AccessModifier;
            Context.AccessModifier = accessModifier;
            return this;
        }

        public MethodSearch SetReturnType(string typeName) {
            Context.SearchFor = Context.SearchFor | MethodSearchFlags.ReturnType;
            Context.ReturnType = SignatureType.FromTypeString(typeName);
            return this;
        }
        public MethodSearch SetReturnType(Type type) {
            Context.SearchFor = Context.SearchFor | MethodSearchFlags.ReturnType;
            Context.ReturnType = SignatureType.FromType(type);
            return this;
        }

        public MethodSearch SetMethodName(string methodName) {
            Context.SearchFor = Context.SearchFor | MethodSearchFlags.MethodName;
            if (methodName.Contains(".")) {
                var lastDot = methodName.LastIndexOf(".");
                var typeName = methodName.Substring(0, lastDot);
                SetOwnerType(typeName);
                methodName = methodName.Substring(lastDot).Trim('.');
            }
            Context.MethodName = methodName;
            return this;
        }

        public MethodSearch SetGenericArguments(params Type[] arguments) {
            Context.SearchFor = Context.SearchFor | MethodSearchFlags.GenericArguments;
            Context.GenericArguments = arguments?.Select(SignatureType.FromType).ToList() ?? new List<SignatureType>();
            return null;
        }

        public MethodSearch SetParameterTypes(params object[] parameters) {
            
            Context.SearchFor = Context.SearchFor | MethodSearchFlags.ParameterTypes;
            Context.ParameterTypes = TypeHelpers.ToParameterTypes(parameters)?.Select(SignatureType.FromType).ToList() ?? new List<SignatureType>();
            return this;
        }
        public MethodSearch SetParameterTypes(params Type[] parameterTypes) {
            Context.SearchFor = Context.SearchFor | MethodSearchFlags.ParameterTypes;
            Context.ParameterTypes = parameterTypes?.Select(SignatureType.FromType).ToList() ?? new List<SignatureType>();
            return this;
        }


        public MethodSignature ToSignature() {

            return Context.ConvertTo<MethodSignature>();
        }

        public static MethodSearch FromSignature(MethodSignature signature) {
            var search = new MethodSearch();
            search.Context = signature.ConvertTo<MethodSearchContext>();
            
            return search;
        }

        public static MethodSearch FromMethodInfo(MethodInfo methodInfo) {
            return MethodSearch.FromSignature(MethodSignature.FromMethodInfo(methodInfo));
        }

        public static implicit operator MethodSignature(MethodSearch search) {
            return search.ToSignature();
        }

        public static implicit operator MethodSearch(MethodSignature signature) {
            return MethodSearch.FromSignature(signature);
        }

        public override string ToString() {
            return Context.ToString();
        }
    }

    public class MethodSearchContext : MethodSummary {

        public MethodSearchFlags SearchFor { get; internal set; }
    }

    [Flags]
    public enum MethodSearchFlags {
        None = 0,
        OwnerType = 1,
        AccessModifier = 2,
        MethodType = 4,
        ReturnType = 8,
        MethodName = 16,
        GenericArguments = 32,
        ParameterTypes = 64,
        OwnerAlias = 128
    }
}
