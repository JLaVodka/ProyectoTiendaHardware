using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    internal class Clases
    {
        public class Producto : INotifyPropertyChanged
        {
            private int _id;
            private string _nombre;
            private string _descripcion;
            private decimal _precio;
            private int _stock;
            private int _categoriaId;
            private string _imagen;

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

            public string Descripcion
            {
                get => _descripcion;
                set { _descripcion = value; OnPropertyChanged(nameof(Descripcion)); }
            }

            public decimal Precio
            {
                get => _precio;
                set { _precio = value; OnPropertyChanged(nameof(Precio)); }
            }

            public int Stock
            {
                get => _stock;
                set { _stock = value; OnPropertyChanged(nameof(Stock)); }
            }

            public int CategoriaId
            {
                get => _categoriaId;
                set { _categoriaId = value; OnPropertyChanged(nameof(CategoriaId)); }
            }

            public string Imagen
            {
                get => _imagen;
                set { _imagen = value; OnPropertyChanged(nameof(Imagen)); }
            }

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

        public class CarritoItem
        {
            public int ProductoId { get; set; }
            public string Nombre { get; set; }
            public decimal Precio { get; set; }
            public int Cantidad { get; set; }
            public decimal Subtotal => Precio * Cantidad;
        }

        public class DatabaseConnection
        {
            private readonly string _connectionString;

            public DatabaseConnection()
            {
                _connectionString = "Server=localhost;Database=tiendahardware;Uid=root;Pwd=;SslMode=none;";
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
                    connection.Open();
                    string query = "SELECT * FROM productos";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productos.Add(new Producto
                                {
                                    Id = reader.GetInt32("id"),
                                    Nombre = reader.GetString("nombre"),
                                    Descripcion = reader.GetString("descripcion"),
                                    Precio = reader.GetDecimal("precio"),
                                    Stock = reader.GetInt32("stock"),
                                    CategoriaId = reader.GetInt32("categoria_id"),
                                    Imagen = reader.IsDBNull(reader.GetOrdinal("imagen")) ?
                                             null : reader.GetString("imagen")
                                });
                            }
                        }
                    }
                }
                return productos;
            }

            public Producto GetProductoById(int id)
            {
                using (var connection = _dbConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM productos WHERE id = @id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Producto
                                {
                                    Id = reader.GetInt32("id"),
                                    Nombre = reader.GetString("nombre"),
                                    Descripcion = reader.GetString("descripcion"),
                                    Precio = reader.GetDecimal("precio"),
                                    Stock = reader.GetInt32("stock"),
                                    CategoriaId = reader.GetInt32("categoria_id"),
                                    Imagen = reader.IsDBNull(reader.GetOrdinal("imagen")) ?
                                             null : reader.GetString("imagen")
                                };
                            }
                        }
                    }
                }
                return null;
            }

            public void AddProducto(Producto producto)
            {
                using (var connection = _dbConnection.GetConnection())
                {
                    connection.Open();
                    string query = @"INSERT INTO productos 
                               (nombre, descripcion, precio, stock, categoria_id, imagen) 
                               VALUES (@nombre, @descripcion, @precio, @stock, @categoriaId, @imagen)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", producto.Nombre);
                        command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                        command.Parameters.AddWithValue("@precio", producto.Precio);
                        command.Parameters.AddWithValue("@stock", producto.Stock);
                        command.Parameters.AddWithValue("@categoriaId", producto.CategoriaId);
                        command.Parameters.AddWithValue("@imagen", producto.Imagen ?? (object)DBNull.Value);

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
                               nombre = @nombre, 
                               descripcion = @descripcion, 
                               precio = @precio, 
                               stock = @stock, 
                               categoria_id = @categoriaId,
                               imagen = @imagen
                               WHERE id = @id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", producto.Nombre);
                        command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                        command.Parameters.AddWithValue("@precio", producto.Precio);
                        command.Parameters.AddWithValue("@stock", producto.Stock);
                        command.Parameters.AddWithValue("@categoriaId", producto.CategoriaId);
                        command.Parameters.AddWithValue("@imagen", producto.Imagen ?? (object)DBNull.Value);
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
                    string query = "DELETE FROM productos WHERE id = @id";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }

            public List<Producto> GetProductosByCategoria(int categoriaId)
            {
                var productos = new List<Producto>();

                using (var connection = _dbConnection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM productos WHERE categoria_id = @categoriaId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@categoriaId", categoriaId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productos.Add(new Producto
                                {
                                    Id = reader.GetInt32("id"),
                                    Nombre = reader.GetString("nombre"),
                                    Descripcion = reader.GetString("descripcion"),
                                    Precio = reader.GetDecimal("precio"),
                                    Stock = reader.GetInt32("stock"),
                                    CategoriaId = reader.GetInt32("categoria_id"),
                                    Imagen = reader.IsDBNull(reader.GetOrdinal("imagen")) ?
                                             null : reader.GetString("imagen")
                                });
                            }
                        }
                    }
                }
                return productos;
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
            private readonly CategoriaRepository _categoriaRepository;
            private ObservableCollection<Producto> _productos;
            private ObservableCollection<Categoria> _categorias;
            private Producto _productoSeleccionado;
            private List<CarritoItem> _carrito;

            public ProductoViewModel()
            {
                _productoRepository = new ProductoRepository();
                _categoriaRepository = new CategoriaRepository();
                _carrito = new List<CarritoItem>();
                CargarProductos();
                CargarCategorias();
                ProductoSeleccionado = new Producto();
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

            public ObservableCollection<Categoria> Categorias
            {
                get => _categorias;
                set
                {
                    _categorias = value;
                    OnPropertyChanged(nameof(Categorias));
                }
            }

            public Producto ProductoSeleccionado
            {
                get => _productoSeleccionado;
                set
                {
                    _productoSeleccionado = value;
                    OnPropertyChanged(nameof(ProductoSeleccionado));
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
                }
            }

            public decimal TotalCarrito
            {
                get
                {
                    decimal total = 0;
                    foreach (var item in Carrito)
                    {
                        total += item.Subtotal;
                    }
                    return total;
                }
            }

            private void CargarProductos()
            {
                Productos = new ObservableCollection<Producto>(_productoRepository.GetAllProductos());
            }

            private void CargarCategorias()
            {
                Categorias = new ObservableCollection<Categoria>(_categoriaRepository.GetAllCategorias());
            }

            public void GuardarProducto()
            {
                if (ProductoSeleccionado.Id == 0)
                {
                    _productoRepository.AddProducto(ProductoSeleccionado);
                }
                else
                {
                    _productoRepository.UpdateProducto(ProductoSeleccionado);
                }
                CargarProductos();
                ProductoSeleccionado = new Producto();
            }

            public void EliminarProducto()
            {
                if (ProductoSeleccionado != null && ProductoSeleccionado.Id > 0)
                {
                    _productoRepository.DeleteProducto(ProductoSeleccionado.Id);
                    CargarProductos();
                    ProductoSeleccionado = new Producto();
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
            }

            public void RemoverDelCarrito(int productoId)
            {
                var item = Carrito.Find(i => i.ProductoId == productoId);
                if (item != null)
                {
                    Carrito.Remove(item);
                    OnPropertyChanged(nameof(Carrito));
                    OnPropertyChanged(nameof(TotalCarrito));
                }
            }

            public void LimpiarCarrito()
            {
                Carrito.Clear();
                OnPropertyChanged(nameof(Carrito));
                OnPropertyChanged(nameof(TotalCarrito));
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
