using System.Runtime.CompilerServices;
using RfemApplication.ViewModels.Base;

namespace RfemApplication.Models
{
    public class Person : ViewModelBase
    {
        private string _firstName;
        private string _lastName;
        private int _age;
        private bool _isSelected;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string FullName => $"{FirstName} {LastName}";

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Powiadom o zmianie FullName gdy zmieni się FirstName lub LastName
            if (propertyName == nameof(FirstName) || propertyName == nameof(LastName))
            {
                OnPropertyChanged(nameof(FullName));
            }
        }
    }
}