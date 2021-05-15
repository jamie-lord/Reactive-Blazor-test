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
        private List<Person> _people = new List<Person>();

        public PersonService()
        {
            _people.Add(new Person
            {
                Id = 1,
                FirstName = "Jamie",
                LastName = "Lord",
                Created = DateTime.Now
            });
        }

        public async Task<Person> Get(int id)
        {
            return _people.FirstOrDefault(p => p.Id == id);
        }

        public async Task Update()
        {
            _people.First(p => p.Id == 1).Created = DateTime.Now;
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
