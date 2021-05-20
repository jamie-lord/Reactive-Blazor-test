using System;
using System.Collections.Generic;
using System.Linq;

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

            return new Unsubscriber(o.ObjId, observer, Unsubscribe);
        }

        private void Unsubscribe(Unsubscriber unsubscriber)
        {
            _observers[unsubscriber.ObjId].Remove(unsubscriber.Observer);
            if (_observers[unsubscriber.ObjId].Count == 0)
            {
                _observers.Remove(unsubscriber.ObjId);
            }
        }

        public int NumberOfSubscriptions()
        {
            return _observers.SelectMany(x => x.Value).Count();
        }

        public class Unsubscriber : IDisposable
        {
            public readonly int ObjId;
            public readonly IObserver<UpdateEvent> Observer;
            private readonly Action<Unsubscriber> _onDispose;

            public Unsubscriber(int objId, IObserver<UpdateEvent> observer, Action<Unsubscriber> onDispose)
            {
                ObjId = objId;
                Observer = observer;
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                _onDispose.Invoke(this);
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
