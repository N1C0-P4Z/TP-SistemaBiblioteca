namespace Biblioteca_PazReyes_Moloeznik.Modelos;

public class Multa
{
    public int Id { get; set; }
    public int PrestamoId { get; set; }
    public int SocioId { get; set; }
    public decimal Monto { get; set; }
    public int Pagada { get; set; }

    public virtual Prestamo? Prestamo { get; set; }
    public virtual Socio? Socio { get; set; }
}
