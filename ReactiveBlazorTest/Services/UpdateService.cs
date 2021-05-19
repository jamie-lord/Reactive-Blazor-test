using System;
using System.Collections.Generic;

namespace ReactiveBlazorTest.Services
{
    public class UpdateObservable : IObservable<UpdateEvent>
    {
        private readonly Dictionary<int, List<IObserver<UpdateEvent>>> _observers = new();

        public void Updated(UpdateEvent updateEvent)
        {
            Console.WriteLine($"{DateTime.Now}\t{updateEvent.SessionId}\tUpdateObservable.Updated()");
            if (_observers.ContainsKey(updateEvent.Person.Id))
            {
                foreach (var observer in _observers[updateEvent.Person.Id])
                {
                    observer.OnNext(updateEvent);
                }
            }
        }

        public IDisposable Subscribe(IObserver<UpdateEvent> observer)
        {
            var o = (UpdateObserver)observer;

            if (!_observers.ContainsKey(o.ObjId))
            {
                _observers[o.ObjId] = new();
            }

            if (!_observers[o.ObjId].Contains(observer))
            {
                _observers[o.ObjId].Add(observer);
            }

            return new Unsubscriber(_observers[o.ObjId], observer);
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

    public class UpdateObserver : IObserver<UpdateEvent>, IDisposable
    {
        private IDisposable _unsubscriber;
        private readonly Action<UpdateEvent> _func;

        public readonly int ObjId;

        public UpdateObserver(IObservable<UpdateEvent> provider, int objId, Action<UpdateEvent> func)
        {
            ObjId = objId;
            _func = func;
            _unsubscriber = provider.Subscribe(this);
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

        public void Dispose()
        {
            _unsubscriber.Dispose();
        }
    }

    public class UpdateEvent
    {
        public Guid SessionId { get; set; }
        public Person Person { get; set; }
    }
}
