using System;
using System.Diagnostics;
using doob.Reflectensions.Tests.Classes;
using Xunit;

namespace doob.Reflectensions.Tests.Invoketests
{
    public class InvokeSyncTests
    {
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
        private readonly MethodManager _mm = new MethodManager(options => options.EnableCache());

        [Fact]
        public void InvokeSync_int() {
            var building = new Building(7);
            var count = _mm.InvokeMethod<int>(building, "CountFloors");
            Assert.Equal(7, count);


            _mm.InvokeMethod(building, builder => builder.WithMethodName("CountFloors"));
        }

        [Fact]
        public void InvokeSync_int_TO_long()
        {
            var building = new Building(7);
            var count = _mm.InvokeMethod<long>(building, "CountFloors");
            Assert.Equal(7, count);
        }

        [Fact]
        public void InvokeSync_Task() {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            _mm.InvokeMethod(building, builder => builder.WithMethodName("OpenMainDoorAsync").WithParameters(_delay));
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
        }

        [Fact]
        public void InvokeSync_Task_OF_int()
        {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            var floorCount = _mm.InvokeMethod<int>(building, "CountFloorsAsync", _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
            Assert.Equal(7, floorCount);
        }

        [Fact]
        public void InvokeSync_Task_OF_int_TO_decimal()
        {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            var floorCount = _mm.InvokeMethod<decimal>(building, "CountFloorsAsync", _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
            Assert.Equal(7, floorCount);
        }

    }
}
