using System;

namespace Core.Common.DI
{
    public interface IProvider<T>
    {
        T Provide();
        
        T Provide(Action<IArgumentsScope> argumentsConfigurator);
    }

    public interface IArgumentsScope
    {
        void SetArgument<T>(T argument);
    }
    
    public class Configurator
    {
        
    }
}