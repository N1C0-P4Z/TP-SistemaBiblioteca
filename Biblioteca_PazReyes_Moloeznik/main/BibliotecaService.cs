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
        {
            Console.Write("No hay copias disponibles. ¿Desea reservarlo? (S/N): ");
            string? respuesta = Console.ReadLine();
            if (respuesta?.Trim().ToUpper() == "S")
                CrearReserva(socio, libroSeleccionado);
            return;
        }

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

        List<Prestamo> prestamosActivos = _context.Prestamos
            .Include(p => p.Libro)
            .Where(p => p.SocioId == nroSocio && p.EstadoPrestamoId == 1)
            .ToList();

        if (prestamosActivos.Count == 0)
        {
            Console.WriteLine("El socio no tiene prestamos activos.");
            return;
        }

        Console.WriteLine("Prestamos activos:");
        for (int i = 0; i < prestamosActivos.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {prestamosActivos[i].Libro?.Titulo} (Prestado: {prestamosActivos[i].FechaPrestamo}, Vence: {prestamosActivos[i].FechaVencimiento})");
        }

        Console.Write("Seleccione el numero de prestamo a devolver: ");
        string? inputPrestamo = Console.ReadLine();
        if (!int.TryParse(inputPrestamo, out int opcionPrestamo) || opcionPrestamo < 1 || opcionPrestamo > prestamosActivos.Count)
        {
            Console.WriteLine("Opcion invalida.");
            return;
        }

        Prestamo prestamo = prestamosActivos[opcionPrestamo - 1];
        string fechaDevolucion = FormatFecha(DateTime.Now);
        prestamo.FechaDevolucion = fechaDevolucion;
        prestamo.EstadoPrestamoId = 2;

        DateTime fechaVenc = ParseFecha(prestamo.FechaVencimiento);
        DateTime fechaDev = DateTime.Now;

        if (fechaDev > fechaVenc)
        {
            int diasDemora = (fechaDev - fechaVenc).Days;
            decimal multaDiaria = socio.TipoSocio?.MultaDiaria ?? 150m;
            decimal montoMulta = diasDemora * multaDiaria;

            Multa multa = new Multa
            {
                PrestamoId = prestamo.Id,
                SocioId = socio.NroSocio,
                Monto = montoMulta,
                Pagada = 0
            };
            _context.Multas.Add(multa);

            Console.WriteLine($"Devolucion con demora de {diasDemora} dias. Multa generada: ${montoMulta}");
        }
        else
        {
            Console.WriteLine("Devolucion en termino. Sin multa.");
        }

        _context.SaveChanges();

        Libro? libro = prestamo.Libro;
        if (libro != null)
        {
            Reserva? reservaPendiente = _context.Reservas
                .Include(r => r.Socio)
                .Where(r => r.LibroId == libro.ISBN && r.EstadoReservaId == 1)
                .OrderBy(r => r.FechaReserva)
                .FirstOrDefault();

            if (reservaPendiente != null)
            {
                reservaPendiente.EstadoReservaId = 2;
                _context.SaveChanges();
                Console.WriteLine($"Se notifica al socio {reservaPendiente.Socio?.Nombre} {reservaPendiente.Socio?.Apellido} que su reserva de {libro.Titulo} ya esta disponible.");
            }
        }

        Console.WriteLine($"Libro {libro?.Titulo} devuelto correctamente.");
    }

    public void ReservarLibro()
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
        CrearReserva(socio, libroSeleccionado);
    }

    public void MostrarDetalleSocio()
    {
        Console.Write("Ingrese el numero de socio: ");
        string? inputNro = Console.ReadLine();
        if (!int.TryParse(inputNro, out int nroSocio))
        {
            Console.WriteLine("Numero de socio invalido.");
            return;
        }

        Socio? socio = _context.Socios
            .Include(s => s.TipoSocio)
            .FirstOrDefault(s => s.NroSocio == nroSocio);

        if (socio == null)
        {
            Console.WriteLine($"Socio {nroSocio} no encontrado.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"Socio #{socio.NroSocio}: {socio.Nombre} {socio.Apellido}");
        Console.WriteLine($"Email: {socio.Email}");
        Console.WriteLine($"Tipo: {socio.TipoSocio?.Descripcion}");
        Console.WriteLine($"Estado: {(socio.Activo == 1 ? "Activo" : "Inactivo")}");

        Console.WriteLine();
        Console.WriteLine("--- Prestamos Activos ---");
        List<Prestamo> activos = _context.Prestamos
            .Include(p => p.Libro)
            .Where(p => p.SocioId == nroSocio && p.EstadoPrestamoId == 1)
            .ToList();

        if (activos.Count == 0)
            Console.WriteLine("No tiene prestamos activos.");
        else
            foreach (var p in activos)
                Console.WriteLine($"  {p.Libro?.Titulo} | Prestado: {p.FechaPrestamo} | Vence: {p.FechaVencimiento}");

        Console.WriteLine();
        Console.WriteLine("--- Historial de Devoluciones ---");
        List<Prestamo> devueltos = _context.Prestamos
            .Include(p => p.Libro)
            .Where(p => p.SocioId == nroSocio && p.EstadoPrestamoId == 2)
            .ToList();

        if (devueltos.Count == 0)
            Console.WriteLine("No tiene devoluciones registradas.");
        else
            foreach (var p in devueltos)
                Console.WriteLine($"  {p.Libro?.Titulo} | Prestado: {p.FechaPrestamo} | Devuelto: {p.FechaDevolucion}");

        Console.WriteLine();
        Console.WriteLine("--- Multas Pendientes ---");
        List<Multa> multas = _context.Multas
            .Where(m => m.SocioId == nroSocio && m.Pagada == 0)
            .ToList();

        if (multas.Count == 0)
            Console.WriteLine("No tiene multas pendientes.");
        else
        {
            decimal total = 0;
            foreach (var m in multas)
            {
                total += m.Monto;
                Console.WriteLine($"  Prestamo #{m.PrestamoId} | Monto: ${m.Monto}");
            }
            Console.WriteLine($"  Total: ${total}");
        }
    }

    // Reportes
    public void MostrarLibrosMasPrestados()
    {
        var top = _context.Libros
            .Select(l => new {
                l.Titulo,
                l.Autor,
                Total = _context.Prestamos.Count(p => p.LibroId == l.ISBN)
            })
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToList();

        Console.WriteLine("=== LIBROS MAS PRESTADOS ===");
        if (top.Count == 0)
        {
            Console.WriteLine("No hay prestamos registrados.");
            return;
        }

        for (int i = 0; i < top.Count; i++)
            Console.WriteLine($"{i + 1}. {top[i].Titulo} - {top[i].Autor} ({top[i].Total} prestamos)");
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
            return false;
        }
        return true;
    }

    private void CrearReserva(Socio socio, Libro libro)
    {
        List<Reserva> reservasActivas = _context.Reservas
            .Where(r => r.SocioId == socio.NroSocio && r.LibroId == libro.ISBN && r.EstadoReservaId == 1)
            .ToList();

        if (reservasActivas.Count > 0)
        {
            Console.WriteLine("Ya tiene una reserva pendiente para este libro.");
            return;
        }

        Reserva reserva = new Reserva
        {
            SocioId = socio.NroSocio,
            LibroId = libro.ISBN,
            FechaReserva = FormatFecha(DateTime.Now),
            EstadoReservaId = 1
        };

        _context.Reservas.Add(reserva);
        _context.SaveChanges();

        Console.WriteLine($"Reserva registrada exitosamente.");
        Console.WriteLine($"Socio: {socio.Nombre} {socio.Apellido}");
        Console.WriteLine($"Libro: {libro.Titulo}");
        Console.WriteLine($"Se le notificara cuando el libro este disponible.");
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
