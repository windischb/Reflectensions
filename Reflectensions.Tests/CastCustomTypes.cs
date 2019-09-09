using System;
using Reflectensions.ExtensionMethods;
using Reflectensions.Tests.Classes;
using Xunit;

namespace Reflectensions.Tests
{
    public class CastCustomTypes
    {


        [Theory]
        [InlineData(typeof(Camaro), typeof(Truck))]
        public void From_IsCastableTo(Type from, Type to) {
            var isCastable = from.IsImplicitCastableTo(to);
            Assert.True(isCastable);
        }


        [Theory]
        [InlineData(typeof(Truck), typeof(Camaro))]

        public void From_IsNOTCastableTo(Type from, Type to) {
            var isCastable = from.IsImplicitCastableTo(to);
            Assert.False(isCastable);
        }

        [Fact]
        public void FromEmptyStringToNullableDateTime() {

            var str = "2018-03-21T15:50:17+00:00";

            DateTime? nullDate = str.To<DateTime?>();
            DateTime date = str.To<DateTime>();

            var str2 = "";

            DateTime? nullDate2 = str2.To<DateTime?>(false);
            DateTime date2 = str2.To<DateTime>(false, DateTime.Now);

        }


        [Theory]
        [InlineData("1")]
        [InlineData("12345")]
        public void FromStringToInt(string value) {

           

            int? _nullInt = value.To<int?>();
            int _int = value.To<int>();

           Assert.Equal(_nullInt, int.Parse(value));
           Assert.Equal(_int, int.Parse(value));
        }

        [Theory]
        [InlineData("1,123")]
        [InlineData("12345,123")]
        public void FromStringToDouble(string value) {



            double? _nullInt = value.To<double?>();
            double _int = value.To<double>();

            Assert.Equal(_nullInt, double.Parse(value));
            Assert.Equal(_int, double.Parse(value));
        }
    }
}
