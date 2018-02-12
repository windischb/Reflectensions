using System;
using System.Collections.Generic;
using System.Text;

namespace doob.Reflectensions.Tests.Classes
{
    public interface ITransformer {
        Transformer<T> TransformTo<T>() where T : CamouflageMode;
    }

    
}
