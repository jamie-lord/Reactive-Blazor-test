using Mapster;
using ReactiveBlazorTest.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ReactiveBlazorTest.Services
{
    public class PersonService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly UpdateService _updateService;

        public PersonService(DatabaseContext databaseContext, UpdateService updateService)
        {
            _databaseContext = databaseContext;
            _updateService = updateService;
        }

        public async Task<Person> Get(Guid sessionId, int id)
        {
            Console.WriteLine($"{DateTime.Now}\t{sessionId}\tPersonService.Get()");
            PersonPto pto = await _databaseContext.Persons.FindAsync(id);
            var person = new Person(sessionId, pto.Id);
            return pto.Adapt(person);
        }

        public async Task Update(Guid sessionId, Person person)
        {
            Console.WriteLine($"{DateTime.Now}\t{sessionId}\tPersonService.Update()");
            PersonPto pto = await _databaseContext.Persons.FindAsync(person.Id);
            person.Adapt(pto);
            _databaseContext.Persons.Update(pto);
            await _databaseContext.SaveChangesAsync();

            // Send update to all listening views
            _updateService.Updated(new UpdateEvent
            {
                SessionId = sessionId,
                Person = person
            });
        }
    }

    public class Person : INotifyPropertyChanged
    {
        private readonly Guid _sessionId;
        public readonly int Id;

        public Person(Guid sessionId, int id)
        {
            _sessionId = sessionId;
            Id = id;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            Console.WriteLine($"{DateTime.Now}\t{_sessionId}\tPerson.SetField({value})");
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private string _firstName;

        public string FirstName
        {
            get => _firstName;
            set => SetField(ref _firstName, value);
        }

        private string _lastName;

        public string LastName
        {
            get => _lastName;
            set => SetField(ref _lastName, value);
        }

        private DateTime _created;

        public DateTime Created
        {
            get => _created;
            set => SetField(ref _created, value);
        }
    }
}
