using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using doob.Reflectensions.ExtensionMethods;
using doob.Reflectensions.HelperClasses;

namespace doob.Reflectensions
{
    public class MethodSummary
    {
        public SignatureType ReturnType { get; internal set; }
        public MethodAccessModifier AccessModifier { get; internal set; }
        public MethodType MethodType { get; internal set; }
        public string OwnerTypeAlias { get; internal set; }
        public SignatureType OwnerType { get; internal set; }
        public string MethodNameAlias { get; internal set; }
        public string MethodName { get; internal set; }
        public List<SignatureType> ParameterTypes { get; internal set; } = new List<SignatureType>();
        public List<SignatureType> GenericArguments { get; internal set; } = new List<SignatureType>();


        public MethodInfoRating RateAgainst(MethodSummary search, MethodSearchFlags searchFlags) {

            var result = new MethodInfoRating();

            if (searchFlags.HasFlag(MethodSearchFlags.OwnerType)) {
                if (this.OwnerType.FoundMatchingType != search.OwnerType.FoundMatchingType) {
                    return result.SetFailed();
                }
            }

            if (searchFlags.HasFlag(MethodSearchFlags.AccessModifier)) {
                if (!search.AccessModifier.HasFlag(AccessModifier)) {
                    return result.SetFailed();
                }
            }

            if (searchFlags.HasFlag(MethodSearchFlags.ReturnType)) {
                if (this.ReturnType != search.ReturnType) {
                    return result.SetFailed();
                }
            }

            if (searchFlags.HasFlag(MethodSearchFlags.MethodName)) {
                result.SetMethodNameRating(GetMethodNameRating(search.MethodName));
            }

            if (searchFlags.HasFlag(MethodSearchFlags.GenericArguments)) {


                if (search.GenericArguments.Count == GenericArguments.Count) {

                } else {
                    return result.SetFailed();
                }

            }

            if (searchFlags.HasFlag(MethodSearchFlags.ParameterTypes)) {
                result.SetParameterTypesRating(GetParameterTypesRating(search.ParameterTypes));
            }

            return result;
        }

        private MethodNameRating GetMethodNameRating(string searchMethodName) {
            var methodNameRating = new MethodNameRating();
            methodNameRating.SetNamingDiff(StringExtensions.DifferencesCount(MethodName, searchMethodName));
            return methodNameRating;
        }

        private ParameterTypesRating GetParameterTypesRating(List<SignatureType> searchParameterTypes) {


            var parameterRating = new ParameterTypesRating();

            if (ParameterTypes.Count == 0 && searchParameterTypes.Count == 0)
                parameterRating.AddExactMatch();

            if (ParameterTypes.Count < searchParameterTypes.Count) {
                return parameterRating.SetFailed();
            }

            parameterRating.SetGenericParameterCount(ParameterTypes.Count(p => p.IsGeneric));

            for (var i = 0; i < ParameterTypes.Count; i++) {

                var thisType = this.ParameterTypes[i];

                if (searchParameterTypes.Count > i) {
                    var otherType = searchParameterTypes[i];

                    if (otherType.FoundMatchingType == thisType.FoundMatchingType || otherType.FoundMatchingType == typeof(NullObject)) {
                        parameterRating.AddExactMatch();
                        continue;
                    }

                    if (thisType.FoundMatchingType.IsInterface && TypeExtensions.ImplementsInterface(otherType.FoundMatchingType, thisType.FoundMatchingType)) {
                        parameterRating.AddCastableMatch();
                        continue;
                    }

                    if (TypeExtensions.IsImplicitCastableTo(otherType.FoundMatchingType, thisType.FoundMatchingType)) {
                        parameterRating.AddCastableMatch();
                        continue;
                    }

                    if (TypeExtensions.InheritFromClass(otherType.FoundMatchingType, thisType.FoundMatchingType, true, false)) {
                        parameterRating.AddInheritanceLevel(TypeExtensions.InheritFromClassLevel(otherType.FoundMatchingType, thisType.FoundMatchingType));
                        continue;
                    }

                    return parameterRating.SetFailed();
                }

                if (!thisType.IsOptional) {
                    return parameterRating.SetFailed();
                }
            }

            return parameterRating;


        }

        public static MethodSummary FromString(string signature) {

            var regex = new Regex(@"^(((\[(?<OwnerTypeAlias>\w*)\])?(?<OwnerType>\S+)?)?(\s)?(?<AccessModifier>.*)?\s)?(?<ReturnType>\S+)\s(\[(?<MethodNameAlias>\w*)\])?(?<MethodName>\w*)([\[\<](?<GenericArguments>.*)[\]\>])?\((?<ParameterTypes>.*)\)");

            var match = regex.Match(signature);

            if (!match.Success) return null;

            var msig = new MethodSummary();
            msig.MethodName = match.Groups["MethodName"]?.Value;
            msig.GenericArguments = match.Groups["GenericArguments"]?.Value.Split(',')
                .Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).Select(SignatureType.FromTypeString).ToList();

            msig.ParameterTypes = match.Groups["ParameterTypes"]?.Value.Split(',').Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(p => {

                    var regMatch = new Regex(@"(\[(?<genericPart>.+)\])?(?<typePart>.*)").Match(p);

                    var gen = regMatch.Groups["genericPart"]?.Value;
                    var typ = regMatch.Groups["typePart"]?.Value;

                    var sigType = SignatureType.FromTypeString(typ);

                    if (!String.IsNullOrWhiteSpace(gen)) {
                        sigType = sigType.SetGenericName(gen);
                    }

                    return sigType;
                }).ToList();

            msig.ReturnType = SignatureType.FromTypeString(match.Groups["ReturnType"]?.Value);
            var accessModifier = match.Groups["AccessModifier"]?.Value;

            if (accessModifier != null) {
                msig.AccessModifier = Enum<MethodAccessModifier>.Find(accessModifier);
                if (accessModifier.Contains(" ") && msig.AccessModifier == MethodAccessModifier.Unknown) {
                    accessModifier = string.Join(" ", accessModifier.Split(' ').Reverse());
                    msig.AccessModifier = Enum<MethodAccessModifier>.Find(accessModifier);
                }
            }

            msig.OwnerType = SignatureType.FromTypeString(match.Groups["OwnerType"]?.Value);

            msig.MethodNameAlias = match.Groups["MethodNameAlias"]?.Value;
            msig.OwnerTypeAlias = match.Groups["OwnerTypeAlias"]?.Value;
            return msig;

        }

        public T ConvertTo<T>() where T : MethodSummary, new() {

            var result = new T() {
                OwnerType = OwnerType,
                MethodName = MethodName,
                ParameterTypes = ParameterTypes,
                GenericArguments = GenericArguments,
                ReturnType = ReturnType,
                MethodNameAlias = MethodNameAlias,
                AccessModifier = AccessModifier,
                OwnerTypeAlias = OwnerTypeAlias
            };

            return result;
        }

        public override string ToString() {

            var owner = (!String.IsNullOrWhiteSpace(OwnerTypeAlias) ? $"[{OwnerTypeAlias}]" : String.Empty) + OwnerType?.FoundMatchingType;
            var accessModifier = AccessModifier != MethodAccessModifier.Unknown ? $"{AccessModifier.GetName()} " : String.Empty;
            var returnType = ReturnType?.FoundMatchingType?.ToString() ?? ReturnType?.Name;
            var methodName = (!String.IsNullOrWhiteSpace(MethodNameAlias) ? $"[{MethodNameAlias}]" : String.Empty) + MethodName;
            var genericArguments = GenericArguments.Any() ? $"<{string.Join(", ", GenericArguments.Select(a => a.FoundMatchingType?.ToString() ?? a.Name))}>" : null;


            var parameters = ParameterTypes.Select(p => {
                var str = $"{p.Name}";
                if (!String.IsNullOrWhiteSpace(p.GenericName) && p.GenericName != p.Name) {
                    str = $"[{p.GenericName}]{str}";
                }
                return p.IsOptional ? $"{str}?" : str;
            }).ToArray();

            var parametersStr = parameters.Any() ? $"({string.Join(", ", parameters)})" : null;

            return Regex.Replace($"{owner} {accessModifier} {returnType} {methodName}{genericArguments}{parametersStr}".Trim(), @"\s+", " ");
        }
    }
}
