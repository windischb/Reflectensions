namespace doob.Reflectensions {


    public class MethodManagerOptions {

        public bool EnableCache { get; set; }
        //public bool EnableRuntimeDiscovery { get; set; }

        internal MethodManagerOptions() { }
    }

    public class MethodManagerOptionsBuilder {
        private readonly MethodManagerOptions _options = new MethodManagerOptions();

        internal MethodManagerOptionsBuilder() { }

        public MethodManagerOptionsBuilder EnableCache(bool value) {
            _options.EnableCache = value;
            return this;
        }

        public MethodManagerOptionsBuilder EnableCache() {
            return EnableCache(true);
        }

        //public MethodManagerOptionsBuilder EnableRuntimeDiscovery(bool value) {
        //    _options.EnableCache = value;
        //    return this;
        //}

        //public MethodManagerOptionsBuilder EnableRuntimeDiscovery() {
        //    return EnableRuntimeDiscovery(true);

        //}

        public static implicit operator MethodManagerOptions(MethodManagerOptionsBuilder builder) {
            return builder._options;
        }
    }


}
