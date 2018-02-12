using System;
using System.Reflection;
using doob.Reflectensions.Helpers;

namespace doob.Reflectensions {

    public class SignatureType {

        public string Name { get; }
        public bool IsOptional { get; private set; }
        public bool IsGeneric { get; private set; }
        public Type FoundMatchingType { get; private set; }
        public string GenericName { get; private set; }


        private SignatureType(string name) {

            FoundMatchingType = TypeHelper.FindType(name);

            if (FoundMatchingType == null) {
                if (!name.Contains(".")) {
                    IsGeneric = true;
                }
                Name = name;
            } else {
                Name = FoundMatchingType.ToString();
            }

        }

        private SignatureType(Type type)
        {
            Name = type.ToString();
            IsGeneric = type.IsGenericParameter;
            FoundMatchingType = type;
        }

        private SignatureType(ParameterInfo parameterInfo) {
            Name = parameterInfo.ParameterType.ToString();
            IsGeneric = parameterInfo.ParameterType.IsGenericParameter;
            IsOptional = parameterInfo.IsOptional;
            FoundMatchingType = parameterInfo.ParameterType;
        }

        internal SignatureType SetOptional(bool value) {
            IsOptional = value;
            return this;
        }

        internal SignatureType SetGenericName(string value)
        {
            GenericName = value;
            return this;
        }

        internal static SignatureType FromType(Type type) {
            return new SignatureType(type);
        }

        internal static SignatureType FromParameterInfo(ParameterInfo parameterInfo) {
            return new SignatureType(parameterInfo);
        }

        internal static SignatureType FromTypeString(string value) {
            return new SignatureType(value);
        }
    }

}
