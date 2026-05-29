//PRactica 3 Estructuras de datos compuestas
//Contexto es inventario de una tienda de electronicos 

//Crear una instancia del gestor de productos

/*var gestor = new GestorProductos();

//Actividad 1 implementar las estrcuturas
Console.WriteLine("Estructuras de datos");

//Agregar productos

gestor.AgregarProductos(
        new Producto 
        {
            Id =3,
            CodigoBarras ="123456",
            Nombre = "Audifonos",
            Categoria ="Audio",
            Precio = 5.99m,
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
            Precio = 1000m,
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
            Precio = 60m,
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
            Precio = 1500m,
            Stock = 20
        }
    );


//Mostrar inventario

gestor.MostrarInventario();

//Mostrar por categoria
Console.WriteLine("Categoria entretenimiento");
gestor.MostrarProductosPorCategoria("Entretenimiento");

//Buscar por codigo de barras
Console.WriteLine("Buscando producto con codigo 123456:");
var productoEncontrado = gestor.BuscarPorCodigo("123456");
Console.WriteLine(productoEncontrado != null ? productoEncontrado.ToString() : "No encontrado");

//Activiad 2

Console.WriteLine("Algoritmos de ordenacion");

//Creando copia de la lista a ordenar
var prodcutosParaOrdenarPrecio = new List<Producto>(gestor.ObtenerListaProductos());
Console.WriteLine("Orden por precio");
Ordenador.QuickSortPorPrecio(prodcutosParaOrdenarPrecio);
MostrarListaProductos(prodcutosParaOrdenarPrecio);

//Ordenar por nombre
var prodcutosParaOrdenarNombre = new List<Producto>(gestor.ObtenerListaProductos());
Console.WriteLine("Orden por nombre");
var productosOrdenadosMerge = Ordenador.MergeSortPorNombre(prodcutosParaOrdenarNombre);
MostrarListaProductos(productosOrdenadosMerge);

//Actividad 3 algoritmos de busqueda

//ordenar por Id
var productosOrdenadosId = new List<Producto>(gestor.ObtenerListaProductos());
Ordenador.QuickSortPorId(productosOrdenadosId);
Console.WriteLine("Busqueda binaria ID : 3");
var (productoBin, iterBin) = Buscador.BusquedaBinaria(productosOrdenadosId,3);
Console.WriteLine($"Resultado: {productoBin?.ToString() ?? "No encontrado"} | Iteraciones{iterBin}");

//ordenar por nombre
var productosOrdenadosNombre = new List<Producto>(gestor.ObtenerListaProductos());
Ordenador.MergeSortPorNombre(productosOrdenadosNombre);
Console.WriteLine("Busqueda secunecial Nombre : Audifonos");
var (productoNom, iterNom) = Buscador.BusquedaSecuencial(productosOrdenadosNombre, "Audifonos");
Console.WriteLine($"Resultado: {productoNom?.ToString() ?? "No encontrado"} | Iteraciones{iterNom}");

//Funciona auxiliar para mostrar elementos en listas

 void MostrarListaProductos(List<Producto>productos) 
{
    foreach (Producto producto in productos) 
    {
        Console.WriteLine(producto.ToString());
    }
}*/

//Actividad 1

public class Producto 
{
    public int Id { get; set; }
    public string CodigoBarras { get; set; } //Campo unico
    public string Nombre { get; set; }
    public string Categoria { get; set; }
    public double Precio { get; set; }
    public int Stock { get; set; }

    public override string ToString() 
    {
        return $"{Id} | {CodigoBarras} - {Nombre} |{Precio:C} | Stock: {Stock}";
    }
}

public class GestorProductos 
{
    //LISTA : Para mantener el orden de insercion y permitir ordenamientos

    private List<Producto> listaProductos = new List<Producto>();

    //DICCIONARIO: Para busquedas rapidas por codigo de barras

    private Dictionary<string,Producto> diccionarioPorCodigo = new Dictionary<string,Producto>();

    //OPERACIONES CON LISTA

    public void AgregarProductos(Producto p) 
    {
        //Validar codigo de barras unico
        if (diccionarioPorCodigo.ContainsKey(p.CodigoBarras)) 
        {
            throw new Exception("El codigo de barras ya existe");
        }

        listaProductos.Add(p);
        diccionarioPorCodigo[p.CodigoBarras] = p;
    }
    public List<Producto> ObtenerListaProductos() 
    {
        return new List<Producto>(listaProductos);
    }

    public bool EliminarProducto(string codigoBarras) 
    {
        //Validar codigo de barras unico
        if (diccionarioPorCodigo.TryGetValue(codigoBarras, out var producto)) 
        {
            listaProductos.Remove(producto);
            diccionarioPorCodigo.Remove(codigoBarras);
            return true;
        }
        return false;
    }

    public void MostrarInventario() 
    {
        Console.WriteLine("Inventario completo (orden de ingreso):");

        foreach (Producto p in listaProductos) 
        {
            Console.WriteLine(p.ToString());
        }
    }

    //OPERACIONES CON DICCIONARIO (para busquedas especificas)

    public Producto BuscarPorCodigo(string codigoBarras) 
    {
        return diccionarioPorCodigo.TryGetValue(codigoBarras, out var producto)
            ? producto : null;
    }

    public bool ExisteProducto(string codigoBarras) 
    {
        return diccionarioPorCodigo.ContainsKey(codigoBarras);
    }

    public void MostrarProductosPorCategoria(string categoria) 
    {
        foreach (Producto producto in diccionarioPorCodigo.Values) 
        {
            if (producto.Categoria.Equals(categoria,StringComparison.OrdinalIgnoreCase)) 
            {
                Console.WriteLine(producto.ToString());
            }
        }
    }
}

//ACTIVIDAD 2

public class Ordenador 
{
    //Algoritmo QuickSort
    public static void QuickSortPorId(List<Producto> productos) 
    {
        //Caso en el que termina la recursivad
        if (productos.Count <=1) 
        {
            return;
        }

        //1- Selecccionar pivote
        Producto pivote = productos[productos.Count-1];
        var menores = new List<Producto>();
        var mayores = new List<Producto>();

        //2- Reorganizar lista para determinar menores al pivo o mayores al pivote
        for (int i =0; i<productos.Count -1; i++) 
        {
            if (productos[i].Id < pivote.Id) 
            {
                menores.Add(productos[i]);
            } 
            else
            {
                mayores.Add(productos[i]);
            }
        }

        //3- Recursivamente aplicar el algoritmo
        QuickSortPorId(menores);
        QuickSortPorId(mayores);

        //4- Reconsrturir lista con elementos ordenados
        productos.Clear();
        productos.AddRange(menores);
        productos.Add(pivote);
        productos.AddRange(mayores);
    }

    public static void QuickSortPorPrecio(List<Producto> productos) 
    {
        //Caso en el que termina la recursivad
        if (productos.Count <= 1) 
        {
            return;
        }

        //1- Selecccionar pivote
        Producto pivote = productos[productos.Count - 1];
        var menores = new List<Producto>();
        var mayores = new List<Producto>();

        //2- Reorganizar lista para determinar menores al pivo o mayores al pivote
        for (int i = 0; i < productos.Count - 1; i++) 
        {
            if (productos[i].Precio < pivote.Precio) 
            {
                menores.Add(productos[i]);
            } 
            else 
            {
                mayores.Add(productos[i]);
            }
        }

        //3- Recursivamente aplicar el algoritmo
        QuickSortPorPrecio(menores);
        QuickSortPorPrecio(mayores);

        //4- Reconstruir lista con elementos ordenados
        productos.Clear();
        productos.AddRange(menores);
        productos.Add(pivote);
        productos.AddRange(mayores);
    }

    //MergeSort

    public static List<Producto> MergeSortPorNombre(List<Producto> productos)
    {
        // Caso que detiene la recursividad
        if (productos.Count <= 1)
        {
            return productos;
        }

        // 1. Dividir la lista en dos mitades
        int mitad = productos.Count / 2;
        var izquierda = productos.GetRange(0, mitad);
        var derecha = productos.GetRange(mitad, productos.Count - mitad);

        // 2. Ordenar recursivamente cada mitad
        izquierda = MergeSortPorNombre(izquierda);
        derecha = MergeSortPorNombre(derecha);

        // 3. Mezclar las dos mitades ordenadas
        return Mezclar(izquierda, derecha);

    }
    private static List<Producto> Mezclar(List<Producto> izquierda, List<Producto> derecha)
    {
        var resultado = new List<Producto>();
        int i = 0, j = 0;

        // 4. Comparar y agregar en orden 

        while (i < izquierda.Count && j < derecha.Count)
        {
            if (string.Compare(izquierda[i].Nombre, derecha[j].Nombre) < 0)
            {
                resultado.Add(izquierda[i++]);
            }
            else
            {
                resultado.Add(derecha[j++]);
            }
        }

        // 5. Agregar los elementos restantes (si los hay)
        while (i < izquierda.Count)
        {
            resultado.Add(izquierda[i++]);
        }
        while (j < derecha.Count)
        {
            resultado.Add(derecha[j++]);
        }
        return resultado;
    }

    public static List<Producto> MergeSortPorPrecio(List<Producto> productos) 
    {
        //Caso que detiene la recursividad
        if (productos.Count <= 1) 
        {
            return productos;
        }

        //1- Dividir la lista en dos partes
        int mitad = productos.Count / 2;
        var izquierda = productos.GetRange(0, mitad);
        var derecha = productos.GetRange(mitad, productos.Count - mitad);

        //2-Ordenar recurisvamente cada mitad
        izquierda = MergeSortPorPrecio(izquierda);
        derecha = MergeSortPorPrecio(derecha);

        //3- Mezclar las dos mitades ordenadas
        return MezclarPrecio(izquierda, derecha);
    }

    private static List<Producto> MezclarPrecio(List<Producto> izquierda, List<Producto> derecha) 
    {
        var resultado = new List<Producto>();
        int i = 0; int j = 0;

        //4- Comparar y agregar en orden

        while (i < izquierda.Count && j < derecha.Count) 
        {
            if (izquierda[i].Precio < derecha[j].Precio ) 
            {
                resultado.Add(izquierda[i++]);
            } 
            else 
            {
                resultado.Add(derecha[j++]);
            }
        }

        //5- Agregar los elementos restantes
        while (i < izquierda.Count) 
        {
            resultado.Add(izquierda[i++]);
        }
        while (j < derecha.Count) 
        {
            resultado.Add(derecha[j++]);
        }

        return resultado;
    }

}

//ACTIVIDAD 3

public class Buscador 
{
    //Busqueda Binaria (lista ordenada por ID)
    public static (Producto, int) BusquedaBinaria(List<Producto> productos,int idBuscado) 
    {
        int inicio = 0;
        int fin = productos.Count - 1;
        int iteraciones = 0;

        while (inicio<= fin) 
        {
            iteraciones++;

            //1- Calcular el punto medio
            int medio = (inicio+fin)/2;

            //2- Comprobar si encontramos el Id
            if (productos[medio].Id == idBuscado) 
            {
                return (productos[medio], iteraciones);
            }

            //3- Ajustar los limites de busqueda

            if (productos[medio].Id < idBuscado) 
            {
                inicio = medio + 1; //Buscar en la mitad derecha
            } 
            else
            {
                fin = medio - 1; //Buscar en la mitad izquierda
            }
        }

        return (null, iteraciones); //No encontrado
    }

    //Busqueda secuencial

    public static (Producto, int) BusquedaSecuencial(List<Producto>productos, string nombreBuscado) 
    {
        int iteraciones = 0;

        //1- Recorrer la lista elemento a elemento
        foreach (Producto producto in productos) 
        {
            iteraciones++;
            //2- Comparar el nombre con el buscado
            if (producto.Nombre.Equals(nombreBuscado,StringComparison.OrdinalIgnoreCase)) 
            {
                return (producto, iteraciones);
            }
        }
        return (null, iteraciones); //No encontrado 
    }
}