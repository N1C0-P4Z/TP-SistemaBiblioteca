namespace Biblioteca_PazReyes_Moloeznik.Modelos;

public class Prestamo
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public string LibroId { get; set; } = null!;
    public string FechaPrestamo { get; set; } = null!;
    public string FechaVencimiento { get; set; } = null!;
    public string? FechaDevolucion { get; set; }
    public int EstadoPrestamoId { get; set; }
    public int Renovado { get; set; }

    public virtual Socio? Socio { get; set; }
    public virtual Libro? Libro { get; set; }
    public virtual EstadoPrestamo? EstadoPrestamo { get; set; }
    public ICollection<Multa>? Multas { get; set; }
}
