using Mapster;
using Microsoft.EntityFrameworkCore;
using ReactiveBlazorTest.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ReactiveBlazorTest.Services
{
    public class PersonService
    {
        private readonly DatabaseContext _databaseContext;
        //private List<Person> _people = new List<Person>();

        public PersonService(DatabaseContext databaseContext)
        {
            //_people.Add(new Person
            //{
            //    Id = 1,
            //    FirstName = "Jamie",
            //    LastName = "Lord",
            //    Created = DateTime.Now
            //});
            _databaseContext = databaseContext;
        }

        public async Task<Person> Get(int id)
        {
            PersonPto pto = await _databaseContext.Persons.FirstOrDefaultAsync(p => p.Id == id);
            return pto.Adapt<Person>();
        }

        public async Task Update()
        {
            PersonPto pto = await _databaseContext.Persons.FirstOrDefaultAsync(p => p.Id == 1);
            pto.Created = DateTime.Now;
            _databaseContext.Persons.Update(pto);
            await _databaseContext.SaveChangesAsync();
        }
    }

    public class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private int _id;

        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
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
