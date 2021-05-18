using System;

namespace ReactiveBlazorTest.Services
{
    public class UpdateService
    {
        public delegate void UpdatedDelegate(UpdateEvent updateEvent);

        public event UpdatedDelegate OnPersonUpdated;

        public void Updated(UpdateEvent updateEvent)
        {
            Console.WriteLine($"{DateTime.Now}\t{updateEvent.SessionId}\tUpdateService.Updated()");
            OnPersonUpdated?.Invoke(updateEvent);
        }
    }

    public class UpdateEvent
    {
        public Guid SessionId { get; set; }
        public Person Person { get; set; }
    }
}
