using System;
using System.Collections.Generic;

namespace RfemApplication.Services
{
    /// <summary>
    /// Prosty Service Locator do zarządzania zależnościami
    /// </summary>
    public class ServiceLocator
    {
        private static readonly Lazy<ServiceLocator> _instance = new Lazy<ServiceLocator>(() => new ServiceLocator());
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Func<object>> _factories = new Dictionary<Type, Func<object>>();

        private ServiceLocator() { }

        /// <summary>
        /// Instancja Singleton ServiceLocatora
        /// </summary>
        public static ServiceLocator Instance => _instance.Value;

        /// <summary>
        /// Rejestruje singleton serwis
        /// </summary>
        /// <typeparam name="T">Typ interfejsu</typeparam>
        /// <param name="instance">Instancja serwisu</param>
        public void RegisterSingleton<T>(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _services[typeof(T)] = instance;
        }

        /// <summary>
        /// Rejestruje fabrykę dla serwisu
        /// </summary>
        /// <typeparam name="T">Typ interfejsu</typeparam>
        /// <param name="factory">Fabryka tworzenia instancji</param>
        public void RegisterFactory<T>(Func<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factories[typeof(T)] = () => factory();
        }

        /// <summary>
        /// Pobiera serwis z kontenera
        /// </summary>
        /// <typeparam name="T">Typ serwisu</typeparam>
        /// <returns>Instancja serwisu</returns>
        public T GetService<T>()
        {
            var type = typeof(T);

            // Sprawdź czy jest zarejestrowany singleton
            if (_services.TryGetValue(type, out object service))
            {
                return (T)service;
            }

            // Sprawdź czy jest zarejestrowana fabryka
            if (_factories.TryGetValue(type, out Func<object> factory))
            {
                return (T)factory();
            }

            throw new InvalidOperationException($"Serwis typu {type.Name} nie jest zarejestrowany.");
        }

        /// <summary>
        /// Pobiera serwis z kontenera lub tworzy domyślną instancję
        /// </summary>
        /// <typeparam name="T">Typ serwisu</typeparam>
        /// <param name="defaultFactory">Fabryka domyślnej instancji</param>
        /// <returns>Instancja serwisu</returns>
        public T GetService<T>(Func<T> defaultFactory)
        {
            try
            {
                return GetService<T>();
            }
            catch (InvalidOperationException)
            {
                // Jeśli serwis nie jest zarejestrowany, użyj fabryki domyślnej
                var instance = defaultFactory();
                RegisterSingleton(instance);
                return instance;
            }
        }

        /// <summary>
        /// Sprawdza czy serwis jest zarejestrowany
        /// </summary>
        /// <typeparam name="T">Typ serwisu</typeparam>
        /// <returns>True jeśli serwis jest zarejestrowany</returns>
        public bool IsRegistered<T>()
        {
            var type = typeof(T);
            return _services.ContainsKey(type) || _factories.ContainsKey(type);
        }

        /// <summary>
        /// Wyrejestrowuje serwis
        /// </summary>
        /// <typeparam name="T">Typ serwisu</typeparam>
        public void Unregister<T>()
        {
            var type = typeof(T);
            _services.Remove(type);
            _factories.Remove(type);
        }

        /// <summary>
        /// Czyści wszystkie zarejestrowane serwisy
        /// </summary>
        public void Clear()
        {
            _services.Clear();
            _factories.Clear();
        }
    }
}