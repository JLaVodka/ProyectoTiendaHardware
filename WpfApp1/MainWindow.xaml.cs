using System.Collections.ObjectModel;
using System.Windows;
using WpfApp1;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Product> Products { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadSampleData();
            DataContext = this;
        }

        private void LoadSampleData()
        {
            Products = new ObservableCollection<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "GeForce RTX 4080",
                    Brand = "NVIDIA",
                    Category = "GPU",
                    Price = 1199.99m,
                    Specification = "16GB GDDR6X, Ray Tracing, DLSS 3",
                    Stock = 5
                },
                new Product
                {
                    Id = 2,
                    Name = "Ryzen 9 7950X",
                    Brand = "AMD",
                    Category = "CPU",
                    Price = 699.99m,
                    Specification = "16 Cores, 4.5GHz, AM5 Socket",
                    Stock = 8
                },
                new Product
                {
                    Id = 3,
                    Name = "Trident Z5 RGB",
                    Brand = "G.Skill",
                    Category = "RAM",
                    Price = 199.99m,
                    Specification = "32GB DDR5 6000MHz, CL30",
                    Stock = 12
                },
                new Product
                {
                    Id = 4,
                    Name = "ROG Strix Z790-E",
                    Brand = "ASUS",
                    Category = "Motherboard",
                    Price = 499.99m,
                    Specification = "LGA 1700, WiFi 6E, PCIe 5.0",
                    Stock = 3
                },
                new Product
                {
                    Id = 5,
                    Name = "Samsung 980 Pro",
                    Brand = "Samsung",
                    Category = "SSD",
                    Price = 129.99m,
                    Specification = "1TB NVMe M.2, 7000MB/s",
                    Stock = 15
                }
            };

            ProductsItemsControl.ItemsSource = Products;
        }
    }
}