using System;
using System.Collections.Generic;

namespace Core.Common.DI
{
    public interface IModule
    {
        void RegisterSingleton<T>(Func<IDependencyProvider, T> instanceCreator);
        
        void RegisterNamedSingleton<T>(string name, Func<IDependencyProvider, T> instanceCreator);
        
        void RegisterFactory<T>(Func<IDependencyProvider, IArgumentsProvider, T> factory);
        
        void RegisterNamedFactory<T>(string name, Func<IDependencyProvider, IArgumentsProvider, T> factory);
    }
    
    public interface IDependencyProvider
    {
        T Resolve<T>();

        T ResolveNamed<T>(string name);
    }

    public interface IFactory
    {
        object Create();
        
        object Create(Action<IArgumentsScope> argumentsConfigurator);
    }

    public interface IFactory<T> : IFactory
    {
        object IFactory.Create()
        {
            return Create();
        }
        
        object IFactory.Create(Action<IArgumentsScope> argumentsConfigurator)
        {
            return Create(argumentsConfigurator);
        }

        new T Create();

        new T Create(Action<IArgumentsScope> argumentsConfigurator);
    }

    public interface IArgumentsScope
    {
        void SetArgument<T>(T argument);

        void SetNamedArgument<T>(string name, T argument);
    }

    public interface IArgumentsProvider
    {
        T GetArgument<T>();

        T GetNamedArgument<T>(string name);
    }

    public class DependencyContainer : IDependencyProvider
    {
        private readonly Dictionary<Type, SingletonHolder> _singletons = new();
        private readonly Dictionary<(string, Type), SingletonHolder> _namedSingletons = new();
        private readonly Dictionary<Type, IFactory> _factories = new();
        private readonly Dictionary<(string, Type), IFactory> _namedFactories = new();

        public void AddModule(Module module)
        {
            foreach(var (type, instanceCreator) in module.Singletons)
            {
                var holder = new SingletonHolder(() => instanceCreator(this));
                _singletons.Add(type, holder);
            }
            foreach(var (nameAndType, instanceCreator) in module.NamedSingletons)
            {
                var holder = new SingletonHolder(() => instanceCreator(this));
                _namedSingletons.Add(nameAndType, holder);
            }
            foreach(var (factoryTypes, factoryCreator) in module.Factories)
            {
                var factory = factoryCreator(this);
                var factoryHolder = new SingletonHolder(() => factory);
                _factories.Add(factoryTypes.FactoryCreatedType, factory);
                _singletons.Add(factoryTypes.FactoryType, factoryHolder);
            }
            foreach(var ((name, factoryTypes), factoryCreator) in module.NamedFactories)
            {
                var factory = factoryCreator(this);
                var factoryHolder = new SingletonHolder(() => factory);
                var factoryKey = (name, factoryTypes.FactoryCreatedType);
                var singletonKey = (name, factoryTypes.FactoryType);
                _namedFactories.Add(factoryKey, factory);
                _namedSingletons.Add(singletonKey, factoryHolder);
            }
        }

        public T Resolve<T>()
        {
            if (_singletons.TryGetValue(typeof(T), out var singleton))
            {
                return (T)singleton.Instance;
            }

            if (_factories.TryGetValue(typeof(T), out var factory))
            {
                return (T)factory.Create();
            }

            throw new KeyNotFoundException($"Dependency not found for type {typeof(T)}");
        }

        public T ResolveNamed<T>(string name)
        {
            var lookupKey = (name, typeof(T));
            if (_namedSingletons.TryGetValue(lookupKey, out var singleton))
            {
                return (T)singleton.Instance;
            }

            if (_namedFactories.TryGetValue(lookupKey, out var factory))
            {
                return (T)factory.Create();
            }

            throw new KeyNotFoundException($"Dependency not found for type {typeof(T)} with name {name}");
        }
    }

    public class SingletonHolder
    {
        private readonly Func<object> _instanceCreator;
        private readonly object _lock = new();
        private object _instance;
        
        public SingletonHolder(Func<object> instanceCreator)
        {
            _instanceCreator = instanceCreator;
        }

        public object Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = _instanceCreator();
                        }
                    }
                }
                
                return _instance;
            }
        }
    }

    public class FactoryTypes : IEquatable<FactoryTypes>
    {
        public readonly Type FactoryCreatedType;
        public readonly Type FactoryType;
        
        public FactoryTypes(Type factoryCreatedType, Type factoryType)
        {
            FactoryCreatedType = factoryCreatedType;
            FactoryType = factoryType;
        }

        public bool Equals(FactoryTypes other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(FactoryCreatedType, other.FactoryCreatedType) && Equals(FactoryType, other.FactoryType);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FactoryTypes)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FactoryCreatedType, FactoryType);
        }
    }

    public class Module : IModule
    {
        public IReadOnlyDictionary<Type, Func<IDependencyProvider, object>> Singletons => _singletons;
        private readonly Dictionary<Type, Func<IDependencyProvider, object>> _singletons = new();
        public IReadOnlyDictionary<(string, Type), Func<IDependencyProvider, object>> NamedSingletons => _namedSingletons;
        private readonly Dictionary<(string, Type), Func<IDependencyProvider, object>> _namedSingletons = new();
        public IReadOnlyDictionary<FactoryTypes, Func<IDependencyProvider, IFactory>> Factories => _factories;
        private readonly Dictionary<FactoryTypes, Func<IDependencyProvider, IFactory>> _factories = new();
        public IReadOnlyDictionary<(string, FactoryTypes), Func<IDependencyProvider, IFactory>> NamedFactories => _namedFactories;
        private readonly Dictionary<(string, FactoryTypes), Func<IDependencyProvider, IFactory>> _namedFactories = new();
        
        private Module() {}
        
        public static Module Create(Action<IModule> configurator)
        {
            var module = new Module();
            configurator(module);
            return module;
        }
        
        public void RegisterSingleton<T>(Func<IDependencyProvider, T> instanceCreator)
        {
            _singletons[typeof(T)] = container => instanceCreator(container);
        }

        public void RegisterNamedSingleton<T>(string name, Func<IDependencyProvider, T> instanceCreator)
        {
            _namedSingletons[(name, typeof(T))] = container => instanceCreator(container);
        }

        public void RegisterFactory<T>(Func<IDependencyProvider, IArgumentsProvider, T> factory)
        {
            var factoryTypes = new FactoryTypes(typeof(T), typeof(IFactory<T>));
            _factories[factoryTypes] = container => new Factory<T>(factory, container);
        }

        public void RegisterNamedFactory<T>(string name, Func<IDependencyProvider, IArgumentsProvider, T> factory)
        {
            var factoryTypes = new FactoryTypes(typeof(T), typeof(IFactory<T>));
            _namedFactories[(name, factoryTypes)] = container => new Factory<T>(factory, container);
        }
    }

    public class Factory<T> : IFactory<T>
    {
        private readonly Func<IDependencyProvider, IArgumentsProvider, T> _factory;
        private readonly IDependencyProvider _dependencyProvider;
        private readonly ArgumentsHolder _argumentsHolder = new();

        public Factory(Func<IDependencyProvider, IArgumentsProvider, T> factory,
            IDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
            _factory = factory;
        }

        public T Create()
        {
            return _factory(_dependencyProvider, _argumentsHolder);
        }

        public T Create(Action<IArgumentsScope> argumentsConfigurator)
        {
            argumentsConfigurator(_argumentsHolder);
            return _factory(_dependencyProvider, _argumentsHolder);
        }
    }

    public class ArgumentsHolder : IArgumentsScope, IArgumentsProvider
    {
        private readonly Dictionary<Type, object> _arguments = new();
        private readonly Dictionary<string, object> _namedArguments = new();

        public void SetArgument<T>(T argument)
        {
            _arguments[typeof(T)] = argument;
        }

        public void SetNamedArgument<T>(string name, T argument)
        {
            _namedArguments[name] = argument;
        }

        public T GetArgument<T>()
        {
            return (T)_arguments[typeof(T)];
        }

        public T GetNamedArgument<T>(string name)
        {
            return (T)_namedArguments[name];
        }
    }
}