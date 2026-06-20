namespace Biblioteca_PazReyes_Moloeznik.Modelos;

public class Libro
{
    public string ISBN { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public string Genero { get; set; } = null!;
    public int CantidadCopias { get; set; }

    public ICollection<Prestamo>? Prestamos { get; set; }
    public ICollection<Reserva>? Reservas { get; set; }
}
