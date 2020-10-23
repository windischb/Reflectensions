using System;
using Newtonsoft.Json;
using Reflectensions.ExtensionMethods;
using Reflectensions.Tests.Classes;
using Xunit;

namespace Reflectensions.Tests {
    public class ExpandableObjectTests {
        [Fact]
        public void Serialize() {

            var exp1 = new Expandable1();
            exp1.Name = "Bernhard";
            exp1.Age = 99;
            exp1["Ok"] = true;

            var json = Json.Converter.ToJson(exp1);

            var dict = Json.Converter.ToDictionary(json);


            Assert.Equal(true, dict["Ok"]);
            Assert.Equal("Bernhard", dict["Name"]);
            Assert.Equal("99", dict["Age"].ToString());

        }

        [Fact]
        public void SerializeInherit() {

            var exp2 = new Expandable2();
            exp2.Name = "Bernhard";
            exp2.Age = 99;
            exp2["Ok"] = true;

            var json = Json.Converter.ToJson(exp2);

            var dict = Json.Converter.ToDictionary(json);


            Assert.Equal(true, dict["Ok"]);
            Assert.Equal("Bernhard", dict["Name"]);
            Assert.Equal("99", dict["Age"].ToString());

        }
    }
}
