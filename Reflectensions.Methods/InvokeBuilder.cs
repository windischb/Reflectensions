﻿using System;
using System.Collections.Generic;
using System.Linq;
using Reflectensions.ExtensionMethods;
using Reflectensions.Helper;
using Reflectensions.HelperClasses;

namespace Reflectensions
{
    public class InvokeBuilder
    {
        public InvokeContext Context = new InvokeContext();


        public InvokeBuilder WithMethodName(string methodName) {
            Context.SetMethodName(methodName);
            return this;
        }

        public InvokeBuilder WithMethodType(MethodType methodType) {
            Context.SetMethodType(methodType);
            return this;
        }

        public InvokeBuilder WithAccessModifier(MethodAccessModifier accessModifier) {
            Context.SetAccessModifier(accessModifier);
            return this;
        }

        public InvokeBuilder WithParameters(params object[] parameters) {

            Context.Parameters = parameters?.ToList();
            Context.SetParameterTypes(TypeHelpers.ToParameterTypes(parameters));
            return this;
        }

        internal InvokeBuilder WithGenericArguments(params Type[] arguments) {
            Context.SetGenericArguments(arguments);
            return this;
        }

        internal InvokeBuilder WithGenericArguments<T>() {
            Context.SetGenericArguments(typeof(T));
            return this;
        }

        internal InvokeBuilder WithGenericArguments<T1, T2>() {
            Context.SetGenericArguments(typeof(T1), typeof(T2));
            return this;
        }

        internal InvokeBuilder WithGenericArguments<T1, T2, T3>() {
            Context.SetGenericArguments(typeof(T1), typeof(T2), typeof(T3));
            return this;
        }

        internal InvokeBuilder WithGenericArguments<T1, T2, T3, T4>() {
            Context.SetGenericArguments(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
            return this;
        }
    }

    public class InvokeContext : MethodSearch {

        public List<object> Parameters { get; set; } = new List<object>();

    }
}
