﻿using System;
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
        public void FromEMptyStringToNullableDateTime() {

            var str = "2018-03-21T15:50:17+00:00";

            DateTime? nullDate = str.ConvertTo<DateTime?>();
            DateTime date = str.ConvertTo<DateTime>();

            var str2 = "";

            DateTime? nullDate2 = str2.ConvertTo<DateTime?>(false);
            DateTime date2 = str2.ConvertTo<DateTime>(false, DateTime.Now);

        }
    }
}