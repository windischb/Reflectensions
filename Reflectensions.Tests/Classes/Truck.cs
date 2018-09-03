namespace Reflectensions.Tests.Classes
{
    public class Truck : Car
    {


        public static implicit operator Truck(Camaro camaro) {
            return new Truck();
        }
    }
}
