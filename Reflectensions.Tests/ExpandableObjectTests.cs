using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;
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
            exp2.Dates = new List<DateTime> {
                DateTime.Now,
                DateTime.Now.AddDays(1)
            };

            var exp3 = new Expandable2();
            exp3.Age = 3;
            exp3.Dates = new List<DateTime> {
                DateTime.Now.AddMonths(1),
                DateTime.Now.AddDays(7)
            };
            var exp4 = new Expandable1();
            exp4.Age = 4;
            var autobot = new Autobot("Bruce");
            autobot.ChangeNickName("Brucy");

            exp4["Autobot"] = autobot;



            exp2["nested"] = exp3;
            exp2["list1"] = new List<object> {
                exp3,
                exp4
            };

            exp2["list2"] = new List<object> {
               123123,
                "TestValue",
                exp4
            };

            var json = Json.Converter.ToJson(exp2);

            //var dict = Json.Converter.ToDictionary(json);
            
            dynamic ex = Json.Converter.ToObject<Expandable2>(json);

            //var dates = ex.GetValue<Expandable2>("nested").GetValuesOrDefault<DateTime>("Dates");

            //var atb = ex.GetValuesOrDefault<object>("list2")[2].As<ExpandableObject>();
            ////Assert.Equal(true, dict["Ok"]);
            ////Assert.Equal("Bernhard", dict["Name"]);
            ////Assert.Equal("99", dict["Age"].ToString());

            //var z = atb["Autobot"].To<Autobot>().Name;


            var name = ex.list2[2].Autobot.Name;
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
