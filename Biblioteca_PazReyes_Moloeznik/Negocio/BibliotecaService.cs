using Microsoft.EntityFrameworkCore;
using Biblioteca_PazReyes_Moloeznik.Datos;
using Biblioteca_PazReyes_Moloeznik.Modelos;

namespace Biblioteca_PazReyes_Moloeznik.Negocio;

public class BibliotecaService
{
    private readonly BibliotecaContext _context;

    public BibliotecaService(BibliotecaContext context)
    {
        _context = context;
    }

    //Prestamos
    public void PrestarLibro()
    {
        Console.Write("Ingrese el numero de socio: ");
        string? inputNro = Console.ReadLine();
        if (!int.TryParse(inputNro, out int nroSocio))
        {
            Console.WriteLine("Numero de socio invalido.");
            return;
        }

        Socio? socio = BuscarSocio(nroSocio);
        if (socio == null)
        {
            Console.WriteLine($"Socio {nroSocio} no encontrado.");
            return;
        }

        if (!ValidarSocioActivo(socio))
            return;

        if (TieneMultasPendientes(socio))
            return;

        Console.Write("Ingrese el titulo o autor del libro: ");
        string? textoBusqueda = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(textoBusqueda))
        {
            Console.WriteLine("Debe ingresar un titulo o autor.");
            return;
        }

        List<Libro> libros = BuscarLibros(textoBusqueda);
        if (libros.Count == 0)
        {
            Console.WriteLine("No se encontraron libros con ese criterio.");
            return;
        }

        Console.WriteLine("Libros encontrados:");
        for (int i = 0; i < libros.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {libros[i].Titulo} - {libros[i].Autor} (ISBN: {libros[i].ISBN})");
        }

        Console.Write("Seleccione el numero de libro: ");
        string? inputLibro = Console.ReadLine();
        if (!int.TryParse(inputLibro, out int opcionLibro) || opcionLibro < 1 || opcionLibro > libros.Count)
        {
            Console.WriteLine("Opcion invalida.");
            return;
        }

        Libro libroSeleccionado = libros[opcionLibro - 1];

        if (!CopiasDisponibles(libroSeleccionado))
            return;

        if (!ValidarLimiteLibros(socio))
            return;

        string fechaPrestamo = FormatFecha(DateTime.Now);
        string fechaVencimiento = CalcularFechaVencimiento(socio);

        Prestamo prestamo = new Prestamo
        {
            SocioId = socio.NroSocio,
            LibroId = libroSeleccionado.ISBN,
            FechaPrestamo = fechaPrestamo,
            FechaVencimiento = fechaVencimiento,
            EstadoPrestamoId = 1,
            Renovado = 0
        };

        _context.Prestamos.Add(prestamo);
        _context.SaveChanges();

        Console.WriteLine($"Prestamo registrado exitosamente.");
        Console.WriteLine($"Socio: {socio.Nombre} {socio.Apellido}");
        Console.WriteLine($"Libro: {libroSeleccionado.Titulo}");
        Console.WriteLine($"Fecha de prestamo: {fechaPrestamo}");
        Console.WriteLine($"Fecha de vencimiento: {fechaVencimiento}");
    }

    public void DevolverLibro()
    {
        Console.WriteLine("Funcionalidad de devolucion proximamente.");
    }

    public void MostrarDetalleSocio()
    {
        Console.WriteLine("Funcionalidad de detalle de socio proximamente.");
    }

    private Socio? BuscarSocio(int nroSocio)
    {
        return _context.Socios
            .Include(s => s.TipoSocio)
            .FirstOrDefault(s => s.NroSocio == nroSocio);
    }

    private bool ValidarSocioActivo(Socio socio)
    {
        if (socio.Activo == 0)
        {
            Console.WriteLine($"El socio {socio.NroSocio} esta inactivo. No puede realizar prestamos.");
            return false;
        }
        return true;
    }

    private bool TieneMultasPendientes(Socio socio)
    {
        List<Multa> multasPendientes = _context.Multas
            .Where(m => m.SocioId == socio.NroSocio && m.Pagada == 0)
            .ToList();
        decimal totalMultas = 0;
        foreach (var m in multasPendientes)
            totalMultas += m.Monto;

        if (totalMultas > 0)
        {
            string montoStr = totalMultas.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine($"El socio tiene multas pendientes por ${montoStr}. Debe abonarlas antes de retirar libros.");
            return true;
        }
        return false;
    }

    private List<Libro> BuscarLibros(string texto)
    {
        return _context.Libros
            .Where(l => l.Titulo.Contains(texto) || l.Autor.Contains(texto))
            .ToList();
    }

    private bool CopiasDisponibles(Libro libro)
    {
        int prestamosActivos = _context.Prestamos
            .Count(p => p.LibroId == libro.ISBN && p.EstadoPrestamoId == 1);

        int disponibles = libro.CantidadCopias - prestamosActivos;

        if (disponibles <= 0)
        {
            Console.WriteLine($"No hay copias disponibles de {libro.Titulo}. Ofrecemos opcion de reserva en futura version.");
            return false;
        }
        return true;
    }

    private bool ValidarLimiteLibros(Socio socio)
    {
        int prestamosActivos = _context.Prestamos
            .Count(p => p.SocioId == socio.NroSocio && p.EstadoPrestamoId == 1);

        if (socio.TipoSocio != null && prestamosActivos >= socio.TipoSocio.MaxLibros)
        {
            Console.WriteLine($"Alcanzaste el limite de {socio.TipoSocio.MaxLibros} libros simultaneos como socio {socio.TipoSocio.Descripcion}.");
            return false;
        }
        return true;
    }

    private string CalcularFechaVencimiento(Socio socio)
    {
        int dias = socio.TipoSocio?.DiasPrestamo ?? 7;
        DateTime vencimiento = DateTime.Now.AddDays(dias);
        return FormatFecha(vencimiento);
    }

    private DateTime ParseFecha(string fecha)
    {
        string[] partes = fecha.Split('/');
        int dia = int.Parse(partes[0]);
        int mes = int.Parse(partes[1]);
        int anio = int.Parse(partes[2]);
        return new DateTime(anio, mes, dia);
    }

    private string FormatFecha(DateTime fecha)
    {
        return $"{fecha.Day:00}/{fecha.Month:00}/{fecha.Year}";
    }
}
