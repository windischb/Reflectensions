namespace Reflectensions.Tests.Classes
{
    public interface ITransformer {
        Transformer<T> TransformTo<T>() where T : CamouflageMode;
    }

    
}
