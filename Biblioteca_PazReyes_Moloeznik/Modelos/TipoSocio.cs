namespace Biblioteca_PazReyes_Moloeznik.Modelos;

public class TipoSocio
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = null!;
    public int MaxLibros { get; set; }
    public int DiasPrestamo { get; set; }
    public decimal MultaDiaria { get; set; }
}
