﻿using System;
using System.Collections.Generic;
using System.Text;

namespace doob.Reflectensions.Tests.Classes
{
    public class Transformer : ITransformer  {

        public string Name { get; internal set; }
        public string NickName { get; private set; }

        protected int _transformCount = 0;


        public Transformer(string name) {
            Name = name;
        }

        public Transformer<T> TransformTo<T>() where T : CamouflageMode {


            var t =  new Transformer<T>(Name);
            t._transformCount = _transformCount + 1;
            return t;
        }

        public Transformer TransformTo(CamouflageMode type) {

            var genType = typeof(Transformer<>).MakeGenericType(type.GetType());
            var t = (Transformer) Activator.CreateInstance(genType, new object[] {Name});
            t._transformCount = _transformCount + 1;
            return t;
        }

        public int GetTransformations() {
            return _transformCount;
        }

        public void ChangeName(string name) {
            Name = name;
        }

        public void ChangeNickName(string nickName) {
            NickName = nickName;
        }
    }

    public class Transformer<T> : Transformer where T : CamouflageMode {

        public T CamouflageMode { get; }

        public Transformer(string name) : base(name)
        {
        }

    }
}
