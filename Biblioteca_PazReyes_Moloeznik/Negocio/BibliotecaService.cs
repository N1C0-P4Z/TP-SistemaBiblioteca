using Biblioteca_PazReyes_Moloeznik.Datos;

namespace Biblioteca_PazReyes_Moloeznik.Negocio;

public class BibliotecaService
{
    private readonly BibliotecaContext _context;

    public BibliotecaService(BibliotecaContext context)
    {
        _context = context;
    }

    public void PrestarLibro()
    {
        Console.WriteLine("Funcionalidad de prestamo proximamente.");
    }

    public void DevolverLibro()
    {
        Console.WriteLine("Funcionalidad de devolucion proximamente.");
    }

    public void MostrarDetalleSocio()
    {
        Console.WriteLine("Funcionalidad de detalle de socio proximamente.");
    }
}
