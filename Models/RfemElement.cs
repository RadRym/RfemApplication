using System.Runtime.CompilerServices;
using RfemApplication.ViewModels.Base;

namespace RfemApplication.Models
{
    public class RfemElement : ViewModelBase
    {
        private int _id;
        private string _elementType;
        private double _length;
        private string _crossSection;
        private string _material;
        private string _startNode;
        private string _endNode;
        private bool _isSelected;
        private string _loadCase;
        private double _axialForce;
        private double _shearForce;
        private double _bendingMoment;

        public int ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string ElementType
        {
            get => _elementType;
            set => SetProperty(ref _elementType, value);
        }

        public double Length
        {
            get => _length;
            set => SetProperty(ref _length, value);
        }

        public string CrossSection
        {
            get => _crossSection;
            set => SetProperty(ref _crossSection, value);
        }

        public string Material
        {
            get => _material;
            set => SetProperty(ref _material, value);
        }

        public string StartNode
        {
            get => _startNode;
            set => SetProperty(ref _startNode, value);
        }

        public string EndNode
        {
            get => _endNode;
            set => SetProperty(ref _endNode, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string LoadCase
        {
            get => _loadCase;
            set => SetProperty(ref _loadCase, value);
        }

        public double AxialForce
        {
            get => _axialForce;
            set => SetProperty(ref _axialForce, value);
        }

        public double ShearForce
        {
            get => _shearForce;
            set => SetProperty(ref _shearForce, value);
        }

        public double BendingMoment
        {
            get => _bendingMoment;
            set => SetProperty(ref _bendingMoment, value);
        }

        // Computed properties
        public string Description => $"{ElementType} {ID} - {Material}";

        public string NodeConnection => $"{StartNode} → {EndNode}";

        public string ForcesSummary => $"N={AxialForce:F1} kN, V={ShearForce:F1} kN, M={BendingMoment:F1} kNm";

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Powiadom o zmianie computed properties
            if (propertyName == nameof(ElementType) || propertyName == nameof(ID) || propertyName == nameof(Material))
            {
                OnPropertyChanged(nameof(Description));
            }

            if (propertyName == nameof(StartNode) || propertyName == nameof(EndNode))
            {
                OnPropertyChanged(nameof(NodeConnection));
            }

            if (propertyName == nameof(AxialForce) || propertyName == nameof(ShearForce) || propertyName == nameof(BendingMoment))
            {
                OnPropertyChanged(nameof(ForcesSummary));
            }
        }
    }

    // Enum dla typów elementów
    public enum RfemElementType
    {
        Beam,
        Column,
        Truss,
        Cable,
        Surface,
        Solid
    }

    // Klasa dla materiałów
    public class RfemMaterial : ViewModelBase
    {
        private int _id;
        private string _name;
        private string _type;
        private double _elasticModulus;
        private double _density;
        private double _poissonRatio;

        public int ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public double ElasticModulus
        {
            get => _elasticModulus;
            set => SetProperty(ref _elasticModulus, value);
        }

        public double Density
        {
            get => _density;
            set => SetProperty(ref _density, value);
        }

        public double PoissonRatio
        {
            get => _poissonRatio;
            set => SetProperty(ref _poissonRatio, value);
        }

        public override string ToString()
        {
            return $"{Name} ({Type})";
        }
    }

    // Klasa dla przekrojów
    public class RfemCrossSection : ViewModelBase
    {
        private int _id;
        private string _name;
        private string _type;
        private double _area;
        private double _momentOfInertiaY;
        private double _momentOfInertiaZ;
        private double _height;
        private double _width;

        public int ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public double Area
        {
            get => _area;
            set => SetProperty(ref _area, value);
        }

        public double MomentOfInertiaY
        {
            get => _momentOfInertiaY;
            set => SetProperty(ref _momentOfInertiaY, value);
        }

        public double MomentOfInertiaZ
        {
            get => _momentOfInertiaZ;
            set => SetProperty(ref _momentOfInertiaZ, value);
        }

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public override string ToString()
        {
            return $"{Name} ({Type}) h={Height}mm";
        }
    }
}