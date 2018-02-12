using System;
using System.Collections.Generic;
using System.Text;

namespace doob.Reflectensions.Tests.Classes
{
    public class Truck : Car
    {


        public static implicit operator Truck(Camaro camaro) {
            return new Truck();
        }
    }
}
