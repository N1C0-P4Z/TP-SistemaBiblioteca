namespace Biblioteca_PazReyes_Moloeznik.Modelos;

public class Socio
{
    public int NroSocio { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int TipoSocioId { get; set; }
    public int Activo { get; set; }

    public virtual TipoSocio? TipoSocio { get; set; }
    public ICollection<Prestamo>? Prestamos { get; set; }
    public ICollection<Reserva>? Reservas { get; set; }
    public ICollection<Multa>? Multas { get; set; }
}
