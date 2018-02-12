using System.Reflection;

namespace doob.Reflectensions {

    public class MethodBoxBuilderContext {

        public MethodInfo MethodInfo { get; internal set; }
        public MethodManagerOptions MethodManagerOptions { get; internal set; }

        public MethodBoxBuilderContext(MethodInfo methodInfo, MethodManagerOptions options) {
            MethodManagerOptions = options;
            MethodInfo = methodInfo;
        }

        public static MethodBoxBuilderContext Build(MethodInfo methodInfo, MethodManagerOptions options) {
            return new MethodBoxBuilderContext(methodInfo, options);
        }
    }

}
