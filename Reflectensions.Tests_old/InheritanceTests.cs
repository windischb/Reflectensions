﻿using System;
using Reflectensions.ExtensionMethods;
using Reflectensions.Tests.Classes;
using Xunit;

namespace Reflectensions.Tests
{
    public class InheritanceTests
    {

        [Theory]
        [InlineData(typeof(Autobot), typeof(Transformer))]
        [InlineData(typeof(Decepticon), typeof(Transformer))]
        
        public void From_InheritFromClassTo(Type from, Type to) {

           
            var inherit = from.InheritFromClass(to);
            Assert.True(inherit);
        }


        [Theory]
        [InlineData(typeof(Human), typeof(Transformer))]
        [InlineData(typeof(Transformer<Camaro>), typeof(Transformer<>))]
        [InlineData(typeof(Autobot), typeof(Autobot))]
        public void From_InheritNOTFromClassTo(Type from, Type to) {
            var inherit = from.InheritFromClass(to);
            Assert.False(inherit);
        }
    }
}
