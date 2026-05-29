using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestorProductosWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GestorProductos gestor = new GestorProductos();
        public MainWindow()
        {
            InitializeComponent();
            CargarDatosIniciales();
            dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();
            comboTipoBusqueda.Items.Add("ID");
            comboTipoBusqueda.Items.Add("Nombre");
            //comboTipoBusqueda.Items.Add("Codigo Barras");
            comboTipoBusqueda.SelectedIndex = 0;

            comboCriterioOrden.Items.Add("ID");
            comboCriterioOrden.Items.Add("Nombre");
            comboCriterioOrden.Items.Add("Precio");
            comboCriterioOrden.SelectedIndex = 0;
        }

        public void CargarDatosIniciales() 
        {
            gestor.AgregarProductos(
            new Producto
            {
                Id = 3,
                CodigoBarras = "123456",
                Nombre = "Audifonos",
                Categoria = "Audio",
                Precio = 5.99,
                Stock = 10
            }
            );

            gestor.AgregarProductos(
                    new Producto
                    {
                        Id = 1,
                        CodigoBarras = "789456",
                        Nombre = "XBox",
                        Categoria = "Entretenimiento",
                        Precio = 1000,
                        Stock = 10
                    }
                );

            gestor.AgregarProductos(
                    new Producto
                    {
                        Id = 4,
                        CodigoBarras = "456123",
                        Nombre = "Mouse",
                        Categoria = "Accesorios",
                        Precio = 60,
                        Stock = 15
                    }
                );
            gestor.AgregarProductos(
                    new Producto
                    {
                        Id = 2,
                        CodigoBarras = "741258",
                        Nombre = "PlayStation",
                        Categoria = "Entretenimiento",
                        Precio = 1500,
                        Stock = 20
                    }
                );
        }


        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string criterio = comboTipoBusqueda.SelectedItem.ToString();
            string valor = txtBusqueda.Text;
            List<Producto> productos = gestor.ObtenerListaProductos();

            // Ajustar el máximo de la barra al tamaño de la lista
            iteraciones.Maximum = productos.Count;
            iteraciones.Value = 0;

            switch (criterio)
            {
                case "ID":
                    if (int.TryParse(valor, out int id))
                    {
                        var productosOrdenados = new List<Producto>(productos);
                        Ordenador.QuickSortPorId(productosOrdenados);
                        var (producto, iter) = Buscador.BusquedaBinaria(productosOrdenados, id);
                        MostrarResultadoBusqueda(producto, iter, productos.Count, esBinaria: true);
                    }
                    break;

                case "Nombre":
                    var (productoNombre, iterNombre) = Buscador.BusquedaSecuencial(productos, valor);
                    MostrarResultadoBusqueda(productoNombre, iterNombre, productos.Count, esBinaria: false);
                    break;
            }
        }

        public void MostrarResultadoBusqueda(Producto producto, int iteracionesRealizadas, int totalProductos, bool esBinaria)
        {
            // Mostrar resultado del producto
            txtResultadoBusqueda.Text = producto?.ToString() ?? "No encontrado";

            // Actualizar barra de progreso
            iteraciones.Value = iteracionesRealizadas;

            // Calcular iteraciones que faltaron
            int iteracionesFaltaron;
            if (esBinaria)
            {
                // Para búsqueda binaria, no hay un resto lineal, se puede mostrar 0 o el teórico (log2 total - iteracionesRealizadas)
                // Por simplicidad, mostramos 0.
                iteracionesFaltaron = 0;
            }
            else
            {
                // Búsqueda secuencial: si se encontró, faltaron = total - iteracionesRealizadas
                // Si no se encontró, iteracionesRealizadas = total, entonces faltaron 0.
                iteracionesFaltaron = totalProductos - iteracionesRealizadas;
            }

            // Mostrar en txtIteraciones
            txtIteraciones.Text = $"Iteraciones: {iteracionesRealizadas}";
        }

        private void btnOrdenar_Click(object sender, RoutedEventArgs e) 
        {
            // Obtener copia actualizada de la lista original
            List<Producto> productos = new List<Producto>(gestor.ObtenerListaProductos());

            string criterio = comboCriterioOrden.SelectedItem?.ToString() ?? "Nombre"; // evitar null

            switch (criterio)
            {
                case "ID":
                    Ordenador.QuickSortPorId(productos);
                    break;

                case "Nombre":
                    // CORRECCIÓN: MergeSort devuelve nueva lista ordenada
                    productos = Ordenador.MergeSortPorNombre(productos);
                    break;

                case "Precio":
                    Ordenador.QuickSortPorPrecio(productos);
                    break;
            }

            listViewOrdenados.ItemsSource = productos;
            DibujarGraficoBarras(productos);
        }

        public void DibujarGraficoBarras(List<Producto> productos)
        {
            if (productos == null || productos.Count == 0) return;

            canvasGrafico.Children.Clear();

            // Esperar a que el canvas tenga dimensiones reales (si no, usar valores por defecto)
            double anchoCanvas = canvasGrafico.ActualWidth > 0 ? canvasGrafico.ActualWidth : 800;
            double altoCanvas = canvasGrafico.ActualHeight > 0 ? canvasGrafico.ActualHeight : 400;

            double maxPrecio = productos.Max(p => p.Precio);
            if (maxPrecio <= 0) return;

            double escala = altoCanvas / maxPrecio;
            double anchoBarra = 40;
            double separacion = 10;
            double xInicial = 20;

            for (int i = 0; i < productos.Count; i++)
            {
                var producto = productos[i];
                double altoBarra = producto.Precio * escala;
                double x = xInicial + i * (anchoBarra + separacion);
                double y = altoCanvas - altoBarra;

                // Barra
                Rectangle barra = new Rectangle
                {
                    Width = anchoBarra,
                    Height = altoBarra,
                    Fill = Brushes.CornflowerBlue
                };
                Canvas.SetLeft(barra, x);
                Canvas.SetTop(barra, y);
                canvasGrafico.Children.Add(barra);

                // Etiqueta de precio (opcional, encima de la barra)
                TextBlock precioText = new TextBlock
                {
                    Text = $"{producto.Precio:C}",
                    FontSize = 10,
                    Foreground = Brushes.Black
                };
                Canvas.SetLeft(precioText, x);
                Canvas.SetTop(precioText, y - 15);
                canvasGrafico.Children.Add(precioText);

                // Etiqueta de nombre (debajo de la barra)
                TextBlock nombreText = new TextBlock
                {
                    Text = producto.Nombre.Length > 8 ? producto.Nombre.Substring(0, 6) + "..." : producto.Nombre,
                    FontSize = 9,
                    TextAlignment = TextAlignment.Center,
                    Width = anchoBarra
                };
                Canvas.SetLeft(nombreText, x);
                Canvas.SetTop(nombreText, altoCanvas - 5);
                canvasGrafico.Children.Add(nombreText);
            }
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e) 
        {
            var ventanaAgregar = new AgregarProductoWindow();
            if (ventanaAgregar.ShowDialog() == true) 
            {
                Producto nuevoProducto = ventanaAgregar.Producto;

                try 
                {
                    gestor.AgregarProductos(nuevoProducto);
                    dataGridProductos.ItemsSource = null;
                    dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();

                } 
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e) 
        {
            if (dataGridProductos.SelectedItem is Producto productoSeleccionado) 
            {
                bool eliminado = gestor.EliminarProducto(productoSeleccionado.CodigoBarras);

                if (eliminado) 
                {
                    dataGridProductos.ItemsSource = null;
                    dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();
                    MessageBox.Show("Producto eliminado", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } 
            else
            {
                MessageBox.Show("Seleccion un producto", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
