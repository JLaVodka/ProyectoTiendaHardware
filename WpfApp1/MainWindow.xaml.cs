using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Producto> Products { get; set; }
        private ProductoViewModel _viewModel;

        string connectionString = "Server=localhost;Database=tiendahardware;Uid=root;Pwd=;";

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new ProductoViewModel();
            Products = new ObservableCollection<Producto>();

            LoadProductsFromDatabase();
            DataContext = this;
        }

        


        private void LoadProductsFromDatabase()
        {
            Products.Clear();

            using (MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();

                    string query = @"
                        SELECT id_producto, nombre_productos, precio, categoria,
                               especificaciones, stock_producto, marca_productos
                        FROM productos;
                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conexion);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Products.Add(new Producto
                        {
                            Id = reader.GetInt32("id_producto"),
                            Nombre = reader.GetString("nombre_productos"),
                            Marca = reader.GetString("marca_productos"),
                            Categoria = reader.GetString("categoria"),
                            Precio = reader.GetDecimal("precio"),
                            Especificaciones = reader.GetString("especificaciones"),
                            Stock = reader.GetInt32("stock_producto")
                        });
                    }

                    ProductsItemsControl.ItemsSource = Products;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar: " + ex.Message);
                }
            }
        }

        private void CargarProductos()
        {
            ProductsItemsControl.ItemsSource = _viewModel.Productos;
        }
        private void MostrarMensajeFiltro(string categoria)
        {
            this.Title = $"TechHardware Store - {categoria}";
        }

        private void FiltGPU_Selected(object sender, RoutedEventArgs e)
        {
            // Filtrar por todas las GPUs
            LoadProductsFromDatabase();
            MostrarMensajeFiltro("GPU");
        }

        private void FiltCPU_Selected(object sender, RoutedEventArgs e)
        {
            FiltrarPorCategoria("CPU");
        }

        private void FiltRAM_Selected(object sender, RoutedEventArgs e)
        {
            FiltrarPorCategoria("RAM");
        }

        private void FiltMthr_Selected(object sender, RoutedEventArgs e)
        {
            FiltrarPorCategoria("Motherboard");
        }

        private void FiltSSD_Selected(object sender, RoutedEventArgs e)
        {
            FiltrarPorCategoria("SSD");
        }

        private void FiltMon_Selected(object sender, RoutedEventArgs e)
        {
            FiltrarPorCategoria("Monitores");
        }

        private void FiltrarPorCategoria(string categoria)
        {
            var productosFiltrados = new ObservableCollection<Producto>();

            foreach (var producto in Products)
            {
                if (producto.Categoria?.Equals(categoria, StringComparison.OrdinalIgnoreCase) == true)
                {
                    productosFiltrados.Add(producto);
                }
            }

            ProductsItemsControl.ItemsSource = productosFiltrados;
            MostrarMensajeFiltro(categoria);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadProductsFromDatabase();
            MostrarMensajeFiltro("Todos los productos");
        }

        private void BtnProductos_Click(object sender, RoutedEventArgs e)
        {
            LoadProductsFromDatabase();
            MostrarMensajeFiltro("Todos los productos");
        }

        private void BtnOfertas_Click(object sender, RoutedEventArgs e)
        {
            var productosOferta = new ObservableCollection<Producto>();

            foreach (var producto in Products)
            {
                if (producto.Precio < 300)
                {
                    productosOferta.Add(producto);
                }
            }

            ProductsItemsControl.ItemsSource = productosOferta;
            MostrarMensajeFiltro("Ofertas Especiales");

            MessageBox.Show($"Mostrando {productosOferta.Count} productos en oferta",
                          "Ofertas", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnContacto_Click(object sender, RoutedEventArgs e)
        {
            string infoContacto = "🛠️ TechHardware Store\n\n" +
                                 "📧 Email: info@techhardware.com\n" +
                                 "📞 Teléfono: +57 302 554 1514\n" +
                                 "📍 Dirección: Calle Tecnología 123\n" +
                                 "🕒 Horario: Lunes a Viernes 9:00 - 18:00\n\n" +
                                 "¡Estamos aquí para ayudarte!";
            MessageBox.Show(infoContacto, "Información de Contacto",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Carrito_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Carrito.Count == 0)
            {
                MessageBox.Show("🛒 Tu carrito está vacío",
                              "Carrito", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string contenidoCarrito = "🛒 MI CARRITO\n\n";
            foreach (var item in _viewModel.Carrito)
            {
                contenidoCarrito += $"{item.Nombre}\n";
                contenidoCarrito += $"Cantidad: {item.Cantidad} - {item.Subtotal:C}\n\n";
            }
            contenidoCarrito += $"TOTAL: {_viewModel.TotalCarrito:C}";

            MessageBox.Show(contenidoCarrito, "Mi Carrito",
                          MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}