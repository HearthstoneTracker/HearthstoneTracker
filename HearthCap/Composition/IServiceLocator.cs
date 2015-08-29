using System;

namespace HearthCap.Composition
{
    public interface IServiceLocator
    {
        T GetInstance<T>(string key = null) where T : class;

        object GetInstance(Type type, string key = null);
    }
}
