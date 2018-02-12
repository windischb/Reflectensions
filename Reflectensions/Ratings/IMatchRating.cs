using System;

namespace doob.Reflectensions
{
    public interface IRating : IComparable<IRating> {

        bool Failed { get; }
        string Rating { get; }
    }
}
