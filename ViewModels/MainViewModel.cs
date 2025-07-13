using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RfemApplication.Models;
using RfemApplication.Services;
using RfemApplication.ViewModels;
using RfemApplication.ViewModels.Base;
using RfemApplication.ViewModels.Commands;

namespace RfemApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _searchText;
        private Person _selectedPerson;
        private bool _isLoading;
        private RfemConnectionViewModel _rfemConnectionViewModel;

        public MainViewModel()
        {
            People = new ObservableCollection<Person>();
            FilteredPeople = new ObservableCollection<Person>();

            // Inicjalizacja serwisów
            var dialogService = ServiceLocator.Instance.GetService<IDialogService>(() => new DialogService());
            var rfemService = ServiceLocator.Instance.GetService<IRfemConnectionService>(() => new RfemConnectionService());

            // Inicjalizacja ViewModelu dla połączenia RFEM
            RfemConnectionViewModel = new RfemConnectionViewModel(rfemService, dialogService);

            InitializeCommands();
            LoadSampleData();
        }

        #region Properties

        public ObservableCollection<Person> People { get; }
        public ObservableCollection<Person> FilteredPeople { get; }

        public RfemConnectionViewModel RfemConnectionViewModel
        {
            get => _rfemConnectionViewModel;
            set => SetProperty(ref _rfemConnectionViewModel, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value, FilterPeople);
        }

        public Person SelectedPerson
        {
            get => _selectedPerson;
            set => SetProperty(ref _selectedPerson, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        #endregion

        #region Commands

        public ICommand AddPersonCommand { get; private set; }
        public ICommand DeletePersonCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand SelectAllCommand { get; private set; }

        private void InitializeCommands()
        {
            AddPersonCommand = new RelayCommand(AddPerson);
            DeletePersonCommand = new RelayCommand(DeletePerson, CanDeletePerson);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            SelectAllCommand = new RelayCommand(SelectAll);
        }

        #endregion

        #region Command Methods

        private void AddPerson()
        {
            var newPerson = new Person
            {
                FirstName = "Nowa",
                LastName = "Osoba",
                Age = 25
            };

            People.Add(newPerson);
            FilterPeople();
        }

        private void DeletePerson()
        {
            if (SelectedPerson != null)
            {
                People.Remove(SelectedPerson);
                FilterPeople();
                SelectedPerson = null;
            }
        }

        private bool CanDeletePerson()
        {
            return SelectedPerson != null;
        }

        private async Task RefreshAsync()
        {
            IsLoading = true;

            // Symulacja asynchronicznego ładowania danych
            await Task.Delay(2000);

            LoadSampleData();
            IsLoading = false;
        }

        private void SelectAll()
        {
            foreach (var person in People)
            {
                person.IsSelected = true;
            }
        }

        #endregion

        #region Private Methods

        private void LoadSampleData()
        {
            People.Clear();

            var samplePeople = new[]
            {
                new Person { FirstName = "Jan", LastName = "Kowalski", Age = 30 },
                new Person { FirstName = "Anna", LastName = "Nowak", Age = 25 },
                new Person { FirstName = "Piotr", LastName = "Wiśniewski", Age = 35 }
            };

            foreach (var person in samplePeople)
            {
                People.Add(person);
            }

            FilterPeople();
        }

        private void FilterPeople()
        {
            FilteredPeople.Clear();

            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? People
                : People.Where(p => p.FullName.ToLower().Contains(SearchText.ToLower()));

            foreach (var person in filtered)
            {
                FilteredPeople.Add(person);
            }
        }

        #endregion
    }
}