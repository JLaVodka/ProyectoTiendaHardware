using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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

        // Cargar productos desde MySQL
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

        // AGREGAR AL CARRITO
        private void AgrCarrito_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var producto = button.DataContext as Producto;

                if (producto != null)
                {
                    _viewModel.AgregarAlCarrito(producto);
                    Carrito.Content = $"🛒 Carrito ({_viewModel.ItemsCarrito})";

                    MessageBox.Show(
                        $"Producto \"{producto.Nombre}\" agregado al carrito.",
                        "Carrito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
        }

        // MOSTRAR CARRITO
        private void Carrito_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Carrito.Count == 0)
            {
                MessageBox.Show("🛒 Tu carrito está vacío");
                return;
            }

            string contenido = "🛒 MI CARRITO\n\n";

            foreach (var item in _viewModel.Carrito)
            {
                contenido += $"{item.Nombre}\n";
                contenido += $"Cantidad: {item.Cantidad} | Total: {item.Subtotal:C}\n\n";
            }

            contenido += $"TOTAL: {_viewModel.TotalCarrito:C}";

            MessageBox.Show(contenido);
        }

        // Botón Inicio — Recargar todo
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadProductsFromDatabase();
        }

        // FILTROS DE CATEGORÍA
        private void FiltGPU_Selected(object sender, RoutedEventArgs e) => FiltrarPorCategoria("GPU");
        private void FiltCPU_Selected(object sender, RoutedEventArgs e) => FiltrarPorCategoria("CPU");
        private void FiltRAM_Selected(object sender, RoutedEventArgs e) => FiltrarPorCategoria("RAM");
        private void FiltMthr_Selected(object sender, RoutedEventArgs e) => FiltrarPorCategoria("Motherboard");
        private void FiltSSD_Selected(object sender, RoutedEventArgs e) => FiltrarPorCategoria("SSD");
        private void FiltMon_Selected(object sender, RoutedEventArgs e) => FiltrarPorCategoria("Monitor");

        private void FiltrarPorCategoria(string categoria)
        {
            var lista = new ObservableCollection<Producto>();

            foreach (var p in Products)
            {
                if (p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase))
                    lista.Add(p);
            }

            ProductsItemsControl.ItemsSource = lista;
        }

        // SLIDER DE PRECIO
        private void PrecioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Monto != null)
            {
                decimal valor = (decimal)e.NewValue;
                Monto.Text = $"${valor:N0}";
                FiltrarPorPrecioMaximo(valor);
            }
        }

        private void FiltrarPorPrecioMaximo(decimal max)
        {
            var lista = new ObservableCollection<Producto>();

            foreach (var p in Products)
            {
                if (p.Precio <= max)
                    lista.Add(p);
            }

            ProductsItemsControl.ItemsSource = lista;
        }

        // BOTÓN CONTACTO
        private void BtnContacto_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Correo: info@techhardware.com\nTel: +57 302 554 1514");
        }

        // BOTÓN OFERTAS
        private void BtnOfertas_Click(object sender, RoutedEventArgs e)
        {
            var lista = new ObservableCollection<Producto>();

            foreach (var p in Products)
            {
                if (p.Precio < 300000)
                    lista.Add(p);
            }

            ProductsItemsControl.ItemsSource = lista;
        }
    }
}
