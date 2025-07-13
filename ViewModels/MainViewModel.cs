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
            LoadSampleData();
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

        public ICommand AddElementCommand { get; private set; }
        public ICommand DeleteElementCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand SelectAllCommand { get; private set; }
        public ICommand LoadFromRfemCommand { get; private set; }

        private void InitializeCommands()
        {
            AddElementCommand = new RelayCommand(AddElement);
            DeleteElementCommand = new RelayCommand(DeleteElement, CanDeleteElement);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            SelectAllCommand = new RelayCommand(SelectAll);
            LoadFromRfemCommand = new AsyncRelayCommand(LoadFromRfemAsync, () => RfemConnectionViewModel.IsConnected);
        }

        #endregion

        #region Command Methods

        private void AddElement()
        {
            var newElement = new RfemElement
            {
                ID = Elements.Count + 1,
                ElementType = "Beam",
                Length = 5.0,
                CrossSection = "IPE 300",
                Material = "S235",
                StartNode = $"N{Elements.Count + 1}",
                EndNode = $"N{Elements.Count + 2}",
                LoadCase = "LC1"
            };

            Elements.Add(newElement);
            FilterElements();
        }

        private void DeleteElement()
        {
            if (SelectedElement != null)
            {
                Elements.Remove(SelectedElement);
                FilterElements();
                SelectedElement = null;
            }
        }

        private bool CanDeleteElement()
        {
            return SelectedElement != null;
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
            foreach (var element in Elements)
            {
                element.IsSelected = true;
            }
        }

        private async Task LoadFromRfemAsync()
        {
            if (!RfemConnectionViewModel.IsConnected)
            {
                return;
            }

            IsLoading = true;

            try
            {
                // Symulacja pobierania danych z RFEM
                await Task.Delay(3000);

                // TODO: Tutaj będzie prawdziwe pobieranie danych z RFEM WebService
                // var elements = await _rfemService.GetElementsAsync();

                LoadSampleRfemData();
            }
            catch (Exception ex)
            {
                // Obsługa błędów
                var dialogService = ServiceLocator.Instance.GetService<IDialogService>();
                dialogService.ShowErrorDialog($"Błąd pobierania danych z RFEM: {ex.Message}", "Błąd");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Private Methods

        private void LoadSampleData()
        {
            Elements.Clear();

            var sampleElements = new[]
            {
                new RfemElement
                {
                    ID = 1,
                    ElementType = "Beam",
                    Length = 6.0,
                    CrossSection = "IPE 300",
                    Material = "S235",
                    StartNode = "N1",
                    EndNode = "N2",
                    LoadCase = "LC1",
                    AxialForce = -150.5,
                    ShearForce = 45.2,
                    BendingMoment = 180.7
                },
                new RfemElement
                {
                    ID = 2,
                    ElementType = "Column",
                    Length = 3.5,
                    CrossSection = "HEB 200",
                    Material = "S355",
                    StartNode = "N2",
                    EndNode = "N3",
                    LoadCase = "LC1",
                    AxialForce = -425.8,
                    ShearForce = 12.3,
                    BendingMoment = 65.4
                },
                new RfemElement
                {
                    ID = 3,
                    ElementType = "Truss",
                    Length = 8.2,
                    CrossSection = "RHS 120x80x5",
                    Material = "S235",
                    StartNode = "N3",
                    EndNode = "N4",
                    LoadCase = "LC1",
                    AxialForce = 89.2,
                    ShearForce = 0.0,
                    BendingMoment = 0.0
                }
            };

            foreach (var element in sampleElements)
            {
                Elements.Add(element);
            }

            FilterElements();
        }

        private void LoadSampleRfemData()
        {
            Elements.Clear();

            // Symulacja danych z RFEM - w przyszłości będzie to pobrane z API
            var rfemElements = new[]
            {
                new RfemElement
                {
                    ID = 101,
                    ElementType = "Beam",
                    Length = 12.0,
                    CrossSection = "IPE 400",
                    Material = "S355",
                    StartNode = "N101",
                    EndNode = "N102",
                    LoadCase = "1.35*G + 1.5*Q",
                    AxialForce = -89.3,
                    ShearForce = 156.7,
                    BendingMoment = 567.8
                },
                new RfemElement
                {
                    ID = 102,
                    ElementType = "Column",
                    Length = 4.2,
                    CrossSection = "HEB 300",
                    Material = "S355",
                    StartNode = "N102",
                    EndNode = "N103",
                    LoadCase = "1.35*G + 1.5*Q",
                    AxialForce = -1245.6,
                    ShearForce = 34.2,
                    BendingMoment = 189.5
                },
                new RfemElement
                {
                    ID = 103,
                    ElementType = "Beam",
                    Length = 7.5,
                    CrossSection = "IPE 300",
                    Material = "S235",
                    StartNode = "N103",
                    EndNode = "N104",
                    LoadCase = "1.35*G + 1.5*Q",
                    AxialForce = -67.8,
                    ShearForce = 89.4,
                    BendingMoment = 234.1
                },
                new RfemElement
                {
                    ID = 104,
                    ElementType = "Truss",
                    Length = 9.8,
                    CrossSection = "CHS 168.3x8",
                    Material = "S355",
                    StartNode = "N104",
                    EndNode = "N105",
                    LoadCase = "1.35*G + 1.5*Q",
                    AxialForce = 345.7,
                    ShearForce = 0.0,
                    BendingMoment = 0.0
                }
            };

            foreach (var element in rfemElements)
            {
                Elements.Add(element);
            }

            FilterElements();
        }

        private void FilterElements()
        {
            FilteredElements.Clear();

            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? Elements
                : Elements.Where(e => e.Description.ToLower().Contains(SearchText.ToLower()) ||
                                     e.Material.ToLower().Contains(SearchText.ToLower()) ||
                                     e.CrossSection.ToLower().Contains(SearchText.ToLower()));

            foreach (var element in filtered)
            {
                FilteredElements.Add(element);
            }
        }

        #endregion
    }
}