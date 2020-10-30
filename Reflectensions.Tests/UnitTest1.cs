using System;
using Reflectensions.ExtensionMethods;
using Xunit;

namespace Reflectensions.Tests {
    public class UnitTest1 {
        [Fact]
        public void Test1() {

            var dt = "\"2020-09-27T11:02:11.8862206+02:00\"";

            var dt1 = dt.ToDateTime();

            var datetime = Reflectensions.Json.Converter.ToObject(dt, typeof(DateTime));

        }
    }
}
