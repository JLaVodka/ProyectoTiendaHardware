using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace WpfApp1
{
    public class Producto : INotifyPropertyChanged
    {
        private int _id;
        private string _nombre;
        private string _marca;
        private string _categoria;
        private decimal _precio;
        private string _especificaciones;
        private int _stock;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(nameof(Nombre)); }
        }

        public string Marca
        {
            get => _marca;
            set { _marca = value; OnPropertyChanged(nameof(Marca)); }
        }

        public string Categoria
        {
            get => _categoria;
            set { _categoria = value; OnPropertyChanged(nameof(Categoria)); }
        }

        public decimal Precio
        {
            get => _precio;
            set { _precio = value; OnPropertyChanged(nameof(Precio)); }
        }

        public string Especificaciones
        {
            get => _especificaciones;
            set { _especificaciones = value; OnPropertyChanged(nameof(Especificaciones)); }
        }

        public int Stock
        {
            get => _stock;
            set { _stock = value; OnPropertyChanged(nameof(Stock)); }
        }

        public bool IsAvailable => Stock > 0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }

    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }

    public class DetallePedido
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class CarritoItem : INotifyPropertyChanged
    {
        private int _cantidad;

        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }

        public int Cantidad
        {
            get => _cantidad;
            set { _cantidad = value; OnPropertyChanged(nameof(Cantidad)); OnPropertyChanged(nameof(Subtotal)); }
        }

        public decimal Subtotal => Precio * Cantidad;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DatabaseConnection
    {
        private readonly string _connectionString;

        public DatabaseConnection()
        {
            _connectionString = "Server=localhost;Database=tiendahardware;Uid=root;Pwd=;";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }

    public class ProductoRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public ProductoRepository()
        {
            _dbConnection = new DatabaseConnection();
        }

        public List<Producto> GetAllProductos()
        {
            var productos = new List<Producto>();

            using (var connection = _dbConnection.GetConnection())
            {
                try
                {
                    connection.Open();
                    string query = "SELECT id_producto, nombre_productos, precio, categoria, especificaciones, stock_producto, marca_productos FROM productos";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productos.Add(new Producto
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
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error al cargar productos: " + ex.Message);
                }
            }
            return productos;
        }

        public Producto GetProductoById(int id)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                string query = "SELECT id_producto, nombre_productos, precio, categoria, especificaciones, stock_producto, marca_productos FROM productos WHERE id_producto = @id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Producto
                            {
                                Id = reader.GetInt32("id_producto"),
                                Nombre = reader.GetString("nombre_productos"),
                                Marca = reader.GetString("marca_productos"),
                                Categoria = reader.GetString("categoria"),
                                Precio = reader.GetDecimal("precio"),
                                Especificaciones = reader.GetString("especificaciones"),
                                Stock = reader.GetInt32("stock_producto")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Producto> GetProductosByCategoria(string categoria)
        {
            var productos = new List<Producto>();

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                string query = "SELECT id_producto, nombre_productos, precio, categoria, especificaciones, stock_producto, marca_productos FROM productos WHERE categoria = @categoria";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@categoria", categoria);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
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
                    }
                }
            }
            return productos;
        }

        public void AddProducto(Producto producto)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                string query = @"INSERT INTO productos 
                               (nombre_productos, marca_productos, categoria, precio, especificaciones, stock_producto) 
                               VALUES (@nombre, @marca, @categoria, @precio, @especificaciones, @stock)";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@marca", producto.Marca);
                    command.Parameters.AddWithValue("@categoria", producto.Categoria);
                    command.Parameters.AddWithValue("@precio", producto.Precio);
                    command.Parameters.AddWithValue("@especificaciones", producto.Especificaciones);
                    command.Parameters.AddWithValue("@stock", producto.Stock);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProducto(Producto producto)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                string query = @"UPDATE productos SET 
                               nombre_productos = @nombre, 
                               marca_productos = @marca, 
                               categoria = @categoria, 
                               precio = @precio, 
                               especificaciones = @especificaciones,
                               stock_producto = @stock
                               WHERE id_producto = @id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@marca", producto.Marca);
                    command.Parameters.AddWithValue("@categoria", producto.Categoria);
                    command.Parameters.AddWithValue("@precio", producto.Precio);
                    command.Parameters.AddWithValue("@especificaciones", producto.Especificaciones);
                    command.Parameters.AddWithValue("@stock", producto.Stock);
                    command.Parameters.AddWithValue("@id", producto.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProducto(int id)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM productos WHERE id_producto = @id";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public class CategoriaRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public CategoriaRepository()
        {
            _dbConnection = new DatabaseConnection();
        }

        public List<Categoria> GetAllCategorias()
        {
            var categorias = new List<Categoria>();

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM categorias";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categorias.Add(new Categoria
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre")
                            });
                        }
                    }
                }
            }
            return categorias;
        }
    }

    public class ProductoViewModel : INotifyPropertyChanged
    {
        private readonly ProductoRepository _productoRepository;
        private ObservableCollection<Producto> _productos;
        private List<CarritoItem> _carrito;
        private int _itemsCarrito;

        public ProductoViewModel()
        {
            _productoRepository = new ProductoRepository();
            _carrito = new List<CarritoItem>();
            CargarProductos();
        }

        public ObservableCollection<Producto> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged(nameof(Productos));
            }
        }

        public List<CarritoItem> Carrito
        {
            get => _carrito;
            set
            {
                _carrito = value;
                OnPropertyChanged(nameof(Carrito));
                OnPropertyChanged(nameof(TotalCarrito));
                ItemsCarrito = _carrito.Sum(item => item.Cantidad);
            }
        }

        public int ItemsCarrito
        {
            get => _itemsCarrito;
            set
            {
                _itemsCarrito = value;
                OnPropertyChanged(nameof(ItemsCarrito));
            }
        }

        public decimal TotalCarrito => Carrito.Sum(item => item.Subtotal);

        private void CargarProductos()
        {
            Productos = new ObservableCollection<Producto>(_productoRepository.GetAllProductos());
        }

        public void CargarProductosPorCategoria(string categoria)
        {
            if (categoria == "Todas las GPUs" || categoria == "Todos")
            {
                CargarProductos();
            }
            else
            {
                var productosFiltrados = _productoRepository.GetProductosByCategoria(categoria);
                Productos = new ObservableCollection<Producto>(productosFiltrados);
            }
        }

        public void AgregarAlCarrito(Producto producto, int cantidad = 1)
        {
            var itemExistente = Carrito.Find(item => item.ProductoId == producto.Id);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                Carrito.Add(new CarritoItem
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad
                });
            }
            OnPropertyChanged(nameof(Carrito));
            OnPropertyChanged(nameof(TotalCarrito));
            ItemsCarrito = Carrito.Sum(item => item.Cantidad);
        }

        public void RemoverDelCarrito(int productoId)
        {
            var item = Carrito.Find(i => i.ProductoId == productoId);
            if (item != null)
            {
                Carrito.Remove(item);
                OnPropertyChanged(nameof(Carrito));
                OnPropertyChanged(nameof(TotalCarrito));
                ItemsCarrito = Carrito.Sum(Item => item.Cantidad);
            }
        }

        public void LimpiarCarrito()
        {
            Carrito.Clear();
            OnPropertyChanged(nameof(Carrito));
            OnPropertyChanged(nameof(TotalCarrito));
            ItemsCarrito = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}