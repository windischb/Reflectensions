namespace doob.Reflectensions
{
    public class MethodBoxBuilderResult<TBox> where TBox : IMethodBox
    {
        public TBox MethodInfoBox { get; set; }

        public string CustomCachingKey { get; set; }

        private bool _ignore;
        public bool Ignore {
            get => MethodInfoBox == null || _ignore;
            set => _ignore = value;
        }

        internal MethodBoxBuilderResult() { }

    }

    
}
