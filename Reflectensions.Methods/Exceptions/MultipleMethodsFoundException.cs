﻿using System;

namespace Reflectensions.Exceptions {

    public class MultipleMethodsFoundException : Exception {
        public override string Message { get; }
        
        public MultipleMethodsFoundException(string methodName) {

            this.Message = $"Mutliple possible Methods with matching Name '{methodName}' found, please try to specify more details...";
        }

       
    }

}
