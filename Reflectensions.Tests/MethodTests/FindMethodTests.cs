using System;
using System.Reflection;
using doob.Reflectensions.Exceptions;
using doob.Reflectensions.Tests.Classes;
using Xunit;
using Xunit.Abstractions;

namespace doob.Reflectensions.Tests.MethodTests
{
    public class FindMethodTests
    {
        private readonly ITestOutputHelper _output;

        public FindMethodTests(ITestOutputHelper output) {
            _output = output;
        }

        [Theory]
        [InlineData(typeof(Autobot), "TransformTo")]
        public void FindMultiple_ByName(Type type, string name) {
            

            var search = MethodSearch.Create();
            search.SetMethodName(name);
            _output.WriteLine(search.ToSignature().ToString());
            MethodInfo method = null;
            try {
                method = type.GetMethods().FindBestMatchingMethodInfo(search);
                
            } catch (MultipleMethodsFoundException e) {
                Assert.NotNull(e);
            }

            Assert.Null(method);

        }

        [Theory]
        [InlineData(typeof(Autobot), typeof(string))]
        public void FindMultiple_ByParameter(Type type, params Type[] parameterTypes) {


            var search = MethodSearch.Create();
            search.SetParameterTypes(parameterTypes);
            _output.WriteLine(search.ToSignature().ToString());
            MethodInfo method = null;
            try {
                method = type.GetMethods().FindBestMatchingMethodInfo(search);

            } catch (MultipleMethodsFoundException e) {
                Assert.NotNull(e);
            }

            Assert.Null(method);
           
        }

        [Theory]
        [InlineData(typeof(Autobot), "@ch?nge+na*", typeof(string))]
        public void FindOne_ByNameANDParameter_IgnoreCaseANDWildcard(Type type, string methodName, params Type[] parameterTypes) {


            var search = MethodSearch.Create();
            search.SetMethodName(methodName);
            search.SetParameterTypes(parameterTypes);
            _output.WriteLine(search.ToSignature().ToString());

            var method = type.GetMethods().FindBestMatchingMethodInfo(search);


            Assert.NotNull(method);
            _output.WriteLine(method.ToString());
        }


        [Theory]
        [InlineData(typeof(Autobot), "ChangeName", typeof(string))]
        public void FindOne_ByNameANDParameter(Type type, string methodName, params Type[] parameterTypes) {


            var search = MethodSearch.Create();
            search.SetMethodName(methodName);
            search.SetParameterTypes(parameterTypes);
            _output.WriteLine(search.ToSignature().ToString());

            var method = type.GetMethods().FindBestMatchingMethodInfo(search);

           
            Assert.NotNull(method);
            _output.WriteLine(method.ToString());
            

        }


    }
}
