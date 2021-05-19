using System;
using System.Collections.Generic;

namespace ReactiveBlazorTest.Services
{
    public class UpdateObservable : IObservable<UpdateEvent>
    {
        private readonly List<IObserver<UpdateEvent>> _observers = new();

        public void Updated(UpdateEvent updateEvent)
        {
            Console.WriteLine($"{DateTime.Now}\t{updateEvent.SessionId}\tUpdateObservable.Updated()");
            foreach (var observer in _observers)
            {
                observer.OnNext(updateEvent);
            }
        }

        public IDisposable Subscribe(IObserver<UpdateEvent> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            return new Unsubscriber(_observers, observer);
        }

        public class Unsubscriber : IDisposable
        {
            private List<IObserver<UpdateEvent>> _observers;
            private IObserver<UpdateEvent> _observer;

            public Unsubscriber(List<IObserver<UpdateEvent>> observers, IObserver<UpdateEvent> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null)
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }

    public class UpdateObserver : IObserver<UpdateEvent>
    {
        private IDisposable unsubscriber;
        private readonly Action<UpdateEvent> _func;

        public UpdateObserver(Action<UpdateEvent> func)
        {
            _func = func;
        }

        public void Subscribe(IObservable<UpdateEvent> provider)
        {
            unsubscriber = provider.Subscribe(this);
        }

        public void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

        public void OnNext(UpdateEvent value)
        {
            _func.Invoke(value);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }

    public class UpdateEvent
    {
        public Guid SessionId { get; set; }
        public Person Person { get; set; }
    }
}
