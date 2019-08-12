using System;
using System.Collections.Generic;

namespace Reflectensions.HelperClasses
{
    internal class ListOfTypes<T1> : List<Type>
    {
        public ListOfTypes() {
            this.Add(typeof(T1));
        }
    }

    internal class ListOfTypes<T1, T2> : List<Type>
    {
        public ListOfTypes()
        {
            this.Add(typeof(T1));
            this.Add(typeof(T2));
        }
    }

    internal class ListOfTypes<T1, T2, T3> : List<Type>
    {
        public ListOfTypes()
        {
            this.Add(typeof(T1));
            this.Add(typeof(T2));
            this.Add(typeof(T3));
        }
    }

    internal class ListOfTypes<T1, T2, T3, T4> : List<Type>
    {
        public ListOfTypes()
        {
            this.Add(typeof(T1));
            this.Add(typeof(T2));
            this.Add(typeof(T3));
            this.Add(typeof(T4));
        }
    }
}
