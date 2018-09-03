using System.Reflection;

namespace Reflectensions
{


    public class DefaultMethodBox : IMethodBox
    {
        public MethodInfo MethodInfo { get; protected set; }

        public static implicit operator DefaultMethodBox(MethodInfo methodInfo) {
            return new DefaultMethodBox {
                MethodInfo = methodInfo
            };
        }

    }

    
}
