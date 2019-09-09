using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reflectensions.ExtensionMethods;
using Xunit;
using Xunit.Abstractions;

namespace Reflectensions.Tests
{
    public class ConvertStringTests
    {

        private readonly ITestOutputHelper _output;

        public ConvertStringTests(ITestOutputHelper output) {
            this._output = output;
        }


        [Theory]
        [InlineData("")]
        [InlineData(@"\/Date(1198908717056)\/")]
        [InlineData("2012-03-19T07:22Z")]
        [InlineData("915148798.75")]
        [InlineData("December 17, 1995 03:24:00")]

        public async Task ConvertToDateTime(string value) {

            var dt = value.ToNullableDateTime();

            _output.WriteLine(dt.ToString());

            var t = await Task.Run(() => "December 17, 1995 03:24:00").ConvertToTaskOf<DateTime>();

            var z = await Task.Run(() => Console.WriteLine("Test")).ConvertToTaskOf<string>();
        }

        [Fact]
        public async Task ConvertAsTaskOfTest() {



            IEnumerable<string> ienum = new List<string>(){"eins", "zwei", "drei"};

            List<string> t = await Task.Run(() => ienum).ConvertToTaskOf<List<string>>();

            _output.WriteLine(new Json().ToJson(t, true));
        }


    }
}
