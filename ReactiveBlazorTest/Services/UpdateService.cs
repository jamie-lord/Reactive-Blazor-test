namespace ReactiveBlazorTest.Services
{
    public class UpdateService
    {
        public delegate void UpdatedDelegate(Person person);

        public event UpdatedDelegate OnPersonUpdated;

        public void Updated(Person person)
        {
            OnPersonUpdated?.Invoke(person);
        }
    }
}
