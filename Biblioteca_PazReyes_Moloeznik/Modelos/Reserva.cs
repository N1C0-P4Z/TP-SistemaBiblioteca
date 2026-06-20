namespace Biblioteca_PazReyes_Moloeznik.Modelos;

public class Reserva
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public string LibroId { get; set; } = null!;
    public string FechaReserva { get; set; } = null!;
    public int EstadoReservaId { get; set; }

    public virtual Socio? Socio { get; set; }
    public virtual Libro? Libro { get; set; }
    public virtual EstadoReserva? EstadoReserva { get; set; }
}
