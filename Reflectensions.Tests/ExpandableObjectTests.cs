using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Reflectensions.ExtensionMethods;
using Reflectensions.JsonConverters;
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

            //var dict = Json.Converter.ToDictionary(json);
            
            var ex = Json.Converter.ToObject<Expandable2>(json);

            //Assert.Equal(true, dict["Ok"]);
            //Assert.Equal("Bernhard", dict["Name"]);
            //Assert.Equal("99", dict["Age"].ToString());

        }

        [Fact]
        public void AsDictionary() {

            var exp2 = new Expandable2();
            exp2.Name = "Bernhard";
            exp2.Age = 99;
            exp2["Ok"] = true;


            var idict = exp2 as IDictionary<string, object>;

            var n = idict["Name"];

            var json = Json.Converter.ToJson(idict);

            var dict = Json.Converter.ToDictionary(json);

            var ndict = (IDictionary<string, object>)Activator.CreateInstance(typeof(Expandable2));
            ndict["Name"] = "BernhardDict";
            ndict["Age"] = 10;

            var lengt = ndict.Count;

            var _ex = ndict as Expandable2;

            var a = _ex.Age;

            Assert.Equal(true, dict["Ok"]);
            Assert.Equal("Bernhard", dict["Name"]);
            Assert.Equal("99", dict["Age"].ToString());

        }
    }
}
