using System;
using System.Collections.Generic;
using System.Linq;

namespace RfemApplication.Services
{
    /// <summary>
    /// Interfejs do komunikacji między komponentami aplikacji
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Subskrybuje odbiór eventów określonego typu
        /// </summary>
        /// <typeparam name="T">Typ eventu</typeparam>
        /// <param name="handler">Handler obsługujący event</param>
        void Subscribe<T>(Action<T> handler);

        /// <summary>
        /// Usuwa subskrypcję eventów określonego typu
        /// </summary>
        /// <typeparam name="T">Typ eventu</typeparam>
        /// <param name="handler">Handler do usunięcia</param>
        void Unsubscribe<T>(Action<T> handler);

        /// <summary>
        /// Publikuje event do wszystkich subskrybentów
        /// </summary>
        /// <typeparam name="T">Typ eventu</typeparam>
        /// <param name="eventData">Dane eventu</param>
        void Publish<T>(T eventData);

        /// <summary>
        /// Czyści wszystkie subskrypcje
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Implementacja EventAggregatora do komunikacji między komponentami
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<object>> _subscriptions = new Dictionary<Type, List<object>>();
        private readonly object _lock = new object();

        public void Subscribe<T>(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var eventType = typeof(T);
                if (!_subscriptions.ContainsKey(eventType))
                {
                    _subscriptions[eventType] = new List<object>();
                }

                _subscriptions[eventType].Add(handler);
            }
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var eventType = typeof(T);
                if (_subscriptions.ContainsKey(eventType))
                {
                    _subscriptions[eventType].Remove(handler);

                    // Usuń typ jeśli nie ma już subskrybentów
                    if (_subscriptions[eventType].Count == 0)
                    {
                        _subscriptions.Remove(eventType);
                    }
                }
            }
        }

        public void Publish<T>(T eventData)
        {
            List<object> handlers;

            lock (_lock)
            {
                var eventType = typeof(T);
                if (!_subscriptions.ContainsKey(eventType))
                    return;

                // Skopiuj listę handlerów aby uniknąć problemów z modyfikacją podczas iteracji
                handlers = new List<object>(_subscriptions[eventType]);
            }

            // Wywołaj handlery poza lockiem aby uniknąć deadlocków
            foreach (var handler in handlers.OfType<Action<T>>())
            {
                try
                {
                    handler(eventData);
                }
                catch (Exception ex)
                {
                    // Loguj błąd ale nie przerywaj publikowania do innych handlerów
                    System.Diagnostics.Debug.WriteLine($"Błąd w handlerze eventu {typeof(T).Name}: {ex.Message}");
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _subscriptions.Clear();
            }
        }

        /// <summary>
        /// Zwraca liczbę subskrybentów dla określonego typu eventu
        /// </summary>
        /// <typeparam name="T">Typ eventu</typeparam>
        /// <returns>Liczba subskrybentów</returns>
        public int GetSubscriberCount<T>()
        {
            lock (_lock)
            {
                var eventType = typeof(T);
                return _subscriptions.ContainsKey(eventType) ? _subscriptions[eventType].Count : 0;
            }
        }

        /// <summary>
        /// Sprawdza czy są jacyś subskrybenci dla określonego typu eventu
        /// </summary>
        /// <typeparam name="T">Typ eventu</typeparam>
        /// <returns>True jeśli są subskrybenci</returns>
        public bool HasSubscribers<T>()
        {
            return GetSubscriberCount<T>() > 0;
        }
    }

    #region Example Events

    /// <summary>
    /// Przykładowe eventy do użycia w aplikacji
    /// </summary>
    public class RfemConnectionStatusChangedEvent
    {
        public bool IsConnected { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class ApplicationErrorEvent
    {
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class ModelDataChangedEvent
    {
        public string ModelName { get; set; }
        public string ChangeType { get; set; }
        public object ChangedData { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    #endregion
}