using System;

namespace Reflectensions.Exceptions {

    public class MethodNotFoundException : Exception {
        public override string Message { get; }
        
        public MethodNotFoundException(string methodName) {

            this.Message = $"No Method with matching Name '{methodName}' found...";
        }

       
    }

}
