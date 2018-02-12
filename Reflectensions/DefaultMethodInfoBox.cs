using System.Reflection;

namespace doob.Reflectensions
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
