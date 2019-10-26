using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Reflectensions.Helper;
using Reflectensions.Tests.Classes;
using Xunit;

namespace Reflectensions.Tests.Invoketests {
    public class InvokeAsyncTests {

        private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);

        [Fact]
        public async Task InvokeAsync_int() {
            var building = new Building(7);

            var method = building.GetType().GetMethod("CountFloors");

            var count = await InvokeHelper.InvokeMethodAsync<int>(building, method);
            
            Assert.Equal(7, count);
        }

        [Fact]
        public async Task InvokeAsync_int_TO_long() {
            var building = new Building(7);
            var method = building.GetType().GetMethod("CountFloors");
            var count = await InvokeHelper.InvokeMethodAsync<long>(building, method);
            Assert.Equal(7, count);
        }

        [Fact]
        public async Task InvokeAsync_Task() {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            var method = building.GetType().GetMethod("OpenMainDoorAsync");
            await InvokeHelper.InvokeVoidMethodAsync(building, method, _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
        }

        [Fact]
        public async Task InvokeAsync_Task_OF_int() {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            var method = building.GetType().GetMethod("CountFloorsAsync");
            var floorCount = await InvokeHelper.InvokeMethodAsync<int>(building, method, _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
            Assert.Equal(7, floorCount);
        }

        [Fact]
        public async Task InvokeAsync_Task_OF_int_TO_decimal() {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            var method = building.GetType().GetMethod("CountFloorsAsync");
            var floorCount = await InvokeHelper.InvokeMethodAsync<decimal>(building, method, _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
            Assert.Equal(7, floorCount);
        }

    }
}
