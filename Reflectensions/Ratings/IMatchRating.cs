using System;

namespace Reflectensions.Ratings
{
    public interface IRating : IComparable<IRating> {

        bool Failed { get; }
        string Rating { get; }
    }
}
