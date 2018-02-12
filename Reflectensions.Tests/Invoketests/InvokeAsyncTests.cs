using System;
using System.Diagnostics;
using System.Threading.Tasks;
using doob.Reflectensions.Helpers;
using doob.Reflectensions.Tests.Classes;
using Xunit;

namespace doob.Reflectensions.Tests.Invoketests
{
    public class InvokeAsyncTests {

        private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
        private readonly MethodManager _mm = new MethodManager(options => options.EnableCache());

        [Fact]
        public async Task InvokeAsync_int() {
            var building = new Building(7);
            
            
            var count = await _mm.InvokeMethodAsync<int>(building, builder => builder.WithMethodName("CountFloors"));
            Assert.Equal(7, count);
        }

        [Fact]
        public async Task InvokeAsync_int_TO_long()
        {
            var building = new Building(7);
            var count = await _mm.InvokeMethodAsync<long>(building, "CountFloors");
            Assert.Equal(7, count);
        }

        [Fact]
        public async Task InvokeAsync_Task() {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            await _mm.InvokeMethodAsync(building, "OpenMainDoorAsync", _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
        }

        [Fact]
        public async Task InvokeAsync_Task_OF_int()
        {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            var floorCount = await _mm.InvokeMethodAsync<int>(building, "CountFloorsAsync", _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
            Assert.Equal(7, floorCount);
        }

        [Fact]
        public async Task InvokeAsync_Task_OF_int_TO_decimal()
        {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            var floorCount = await _mm.InvokeMethodAsync<decimal>(building, "CountFloorsAsync", _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
            Assert.Equal(7, floorCount);
        }

    }
}
