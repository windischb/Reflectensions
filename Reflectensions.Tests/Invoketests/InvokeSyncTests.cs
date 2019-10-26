using System;
using System.Diagnostics;
using System.Linq;
using Reflectensions.ExtensionMethods;
using Reflectensions.Helper;
using Reflectensions.Tests.Classes;
using Xunit;
using Xunit.Abstractions;


namespace Reflectensions.Tests.Invoketests {
    public class InvokeSyncTests {
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);


        private readonly ITestOutputHelper _output;

        public InvokeSyncTests(ITestOutputHelper output) {
            this._output = output;
        }

        [Fact]
        public void InvokeSync_int() {
            var building = new Building(7);
            var method = building.GetType().GetMethod("CountFloors");

            var count = InvokeHelper.InvokeMethod<int>(building, method);

            Assert.Equal(7, count);
        }

        [Fact]
        public void InvokeSync_int_TO_long() {
            var building = new Building(7);
            var method = building.GetType().GetMethod("CountFloors");
            var count = InvokeHelper.InvokeMethod<long>(building, method);
            Assert.Equal(7, count);
        }

        [Fact]
        public void InvokeSync_Task() {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            var method = building.GetType().GetMethod("OpenMainDoorAsync");
            InvokeHelper.InvokeVoidMethod(building, method, _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
        }

        [Fact]
        public void InvokeSync_Task_OF_int() {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            var method = building.GetType().GetMethod("CountFloorsAsync");
            var floorCount = InvokeHelper.InvokeMethod<int>(building, method, _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
            Assert.Equal(7, floorCount);
        }

        [Fact]
        public void InvokeSync_Task_OF_int_TO_decimal() {

            var building = new Building(7);
            var sw = new Stopwatch();
            sw.Start();
            var method = building.GetType().GetMethod("CountFloorsAsync");
            var floorCount = InvokeHelper.InvokeMethod<decimal>(building, method, _delay);
            sw.Stop();

            Assert.True(sw.Elapsed >= _delay);
            Assert.Equal(7, floorCount);
        }

        [Fact]
        public void InvokeSync_Static_ToJson() {

            var building = new Building(7);
            building.WindowCount = 78;

            var type = TypeExtensions.FindType("Newtonsoft.Json.JsonConvert");
            
            var method = type.GetMethods().WithName("SerializeObject").FirstOrDefault();
            var json = InvokeHelper.InvokeMethod<string>(null, method, building);
            _output.WriteLine(json);

            var type2 = TypeExtensions.FindType("Reflectensions.Json");
            var inst2 = Activator.CreateInstance(type2);
            var method2 = type.GetMethod("ToJson");
            var json2 = InvokeHelper.InvokeMethod<string>(inst2, method, building, true);
            _output.WriteLine(json2);


        }

    }
}
