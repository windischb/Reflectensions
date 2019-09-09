using System;
using System.Diagnostics;
using Reflectensions.ExtensionMethods;
using Reflectensions.Helper;
using Reflectensions.Tests.Classes;
using Xunit;
using Xunit.Abstractions;


namespace Reflectensions.Tests.Invoketests
{
    public class InvokeSyncTests
    {
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
        private readonly MethodManager _mm = new MethodManager(options => options.EnableCache());

        private readonly ITestOutputHelper _output;

        public InvokeSyncTests(ITestOutputHelper output) {
            this._output = output;
        }

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

        [Fact]
        public void InvokeSync_Static_ToJson() {

            var building = new Building(7);
            building.WindowCount = 78;

            var type = TypeHelpers.FindType("Newtonsoft.Json.JsonConvert");
            var json = _mm.InvokeMethod<string>(type, "SerializeObject", building);
            _output.WriteLine(json);

            var jType = TypeHelpers.FindType("Reflectensions.Json").CreateInstance();
            var json2 = _mm.InvokeMethod<string>(jType, "ToJson", building, true);
            _output.WriteLine(json2);

            var json3 = _mm.InvokeMethod<string>(null, "Newtonsoft.Json.JsonConvert.SerializeObject", building);
            _output.WriteLine(json3);

            var json4 = _mm.InvokeMethod<string>(null, "Reflectensions.Json.ToJson", building, true);
            _output.WriteLine(json4);

            var json5 = _mm.InvokeMethod<string>(TypeHelpers.FindType("Reflectensions.Json"), "ToJson", building, true);
            _output.WriteLine(json5);

            
            var obj = _mm.InvokeGenericMethod<Building, Building>(TypeHelpers.FindType("Reflectensions.Json"), "ToObject", json5);


        }

    }
}
