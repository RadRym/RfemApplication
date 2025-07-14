using System;
using System.Collections.Generic;
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
        private RfemElement _selectedElement;
        private bool _isLoading;
        private RfemConnectionViewModel _rfemConnectionViewModel;
        private RfemServerViewModel _rfemServerViewModel;

        public MainViewModel()
        {
            Elements = new ObservableCollection<RfemElement>();
            FilteredElements = new ObservableCollection<RfemElement>();

            // Inicjalizacja serwisów
            var dialogService = ServiceLocator.Instance.GetService<IDialogService>(() => new DialogService());
            var rfemService = ServiceLocator.Instance.GetService<IRfemConnectionService>(() => new RfemConnectionService());
            var serverManager = ServiceLocator.Instance.GetService<IRfemServerManager>(() => new RfemServerManager());

            // Inicjalizacja ViewModeli
            RfemConnectionViewModel = new RfemConnectionViewModel(rfemService, dialogService);
            RfemServerViewModel = new RfemServerViewModel(serverManager, dialogService);

            InitializeCommands();
            // Nie ładujemy przykładowych danych - aplikacja startuje z pustą listą
        }

        #region Properties

        public ObservableCollection<RfemElement> Elements { get; }
        public ObservableCollection<RfemElement> FilteredElements { get; }

        public RfemConnectionViewModel RfemConnectionViewModel
        {
            get => _rfemConnectionViewModel;
            set => SetProperty(ref _rfemConnectionViewModel, value);
        }

        public RfemServerViewModel RfemServerViewModel
        {
            get => _rfemServerViewModel;
            set => SetProperty(ref _rfemServerViewModel, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value, FilterElements);
        }

        public RfemElement SelectedElement
        {
            get => _selectedElement;
            set => SetProperty(ref _selectedElement, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; private set; }
        public ICommand LoadFromRfemCommand { get; private set; }

        private void InitializeCommands()
        {
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            LoadFromRfemCommand = new AsyncRelayCommand(LoadFromRfemAsync, () => RfemConnectionViewModel.IsConnected);
        }

        #endregion

        #region Command Methods

        private async Task RefreshAsync()
        {
            IsLoading = true;

            try
            {
                // Symulacja odświeżania danych
                await Task.Delay(1000);

                // Odśwież filtry
                FilterElements();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadFromRfemAsync()
        {
            if (!RfemConnectionViewModel.IsConnected)
            {
                var dialogService = ServiceLocator.Instance.GetService<IDialogService>();
                dialogService.ShowWarningDialog("Brak połączenia z RFEM Server.", "Ostrzeżenie");
                return;
            }

            IsLoading = true;
            var dialogSvc = ServiceLocator.Instance.GetService<IDialogService>();

            try
            {
                var rfemService = ServiceLocator.Instance.GetService<IRfemConnectionService>();

                // Sprawdź czy jest aktywny model
                var activeModelUrl = rfemService.GetActiveModelUrl();
                if (string.IsNullOrEmpty(activeModelUrl))
                {
                    dialogSvc.ShowWarningDialog(
                        "Brak aktywnego modelu w RFEM.\n\nOtwórz model w programie RFEM i spróbuj ponownie.",
                        "Brak aktywnego modelu");
                    return;
                }

                // Pobierz elementy z RFEM
                var elements = await GetElementsFromRfemAsync();

                // Wyczyść obecne elementy i dodaj nowe
                Elements.Clear();
                foreach (var element in elements)
                {
                    Elements.Add(element);
                }

                FilterElements();

                dialogSvc.ShowInformationDialog(
                    $"Pobrano {elements.Count} elementów z modelu RFEM.",
                    "Pobieranie zakończone");
            }
            catch (Exception ex)
            {
                dialogSvc.ShowErrorDialog(
                    $"Błąd pobierania danych z RFEM:\n\n{ex.Message}",
                    "Błąd");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Private Methods

        private async Task<List<RfemElement>> GetElementsFromRfemAsync()
        {
            var elements = new List<RfemElement>();

            try
            {
                var rfemService = ServiceLocator.Instance.GetService<IRfemConnectionService>();
                var modelClient = rfemService.GetModelClient();

                if (modelClient == null)
                {
                    throw new Exception("Brak połączenia z modelem RFEM. Sprawdź czy model jest otwarty.");
                }

                // Użyj serwisu do pobierania elementów
                var elementsService = ServiceLocator.Instance.GetService<IRfemElementsService>(
                    () => new RfemElementsService());

                elements = await elementsService.GetElementsAsync(modelClient);

                System.Diagnostics.Debug.WriteLine($"Pobrano {elements.Count} elementów z RFEM");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd pobierania z RFEM: {ex.Message}");
                throw;
            }

            return elements;
        }

        private void FilterElements()
        {
            FilteredElements.Clear();

            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? Elements
                : Elements.Where(e =>
                    e.ID.ToString().Contains(SearchText) ||
                    (!string.IsNullOrEmpty(e.ElementType) && e.ElementType.ToLower().Contains(SearchText.ToLower())) ||
                    (!string.IsNullOrEmpty(e.Material) && e.Material.ToLower().Contains(SearchText.ToLower())) ||
                    (!string.IsNullOrEmpty(e.CrossSection) && e.CrossSection.ToLower().Contains(SearchText.ToLower())));

            foreach (var element in filtered)
            {
                FilteredElements.Add(element);
            }
        }

        #endregion
    }
}