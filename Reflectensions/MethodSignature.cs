using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace doob.Reflectensions {
    public class MethodSignature : MethodSummary {
       
        public static MethodSignature FromMethodInfo(MethodInfo methodInfo) {
            var ms = new MethodSignature();
            ms.OwnerType = SignatureType.FromType(methodInfo.ReflectedType);
            ms.MethodName = methodInfo.Name;
            ms.ParameterTypes = methodInfo.GetParameters().Select(SignatureType.FromParameterInfo).ToList();
            ms.GenericArguments = methodInfo.GetGenericArguments().Select(SignatureType.FromType).ToList();
            ms.AccessModifier = methodInfo.GetAccessModifier();
            ms.ReturnType = SignatureType.FromType(methodInfo.ReturnType);
            ms.MethodType = methodInfo.IsStatic ? MethodType.Static : MethodType.Instance;
            return ms;
        }

        
        
        
    }
}
