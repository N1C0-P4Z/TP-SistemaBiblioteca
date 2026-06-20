using Biblioteca_PazReyes_Moloeznik.Datos;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca_PazReyes_Moloeznik;

class Program
{
    static void Main(string[] args)
    {
        using var context = new BibliotecaContext();
        context.Database.EnsureCreated();
        
        Console.WriteLine("Libros disponibles en la biblioteca:");
        Console.WriteLine("------------------------------------");
        
        foreach (var libro in context.Libros.ToList())
        {
            int disponibles = libro.CantidadCopias;
            // mas adelante se restaran los prestamos activos
            Console.WriteLine($"ISBN: {libro.ISBN} | {libro.Titulo} - {libro.Autor} ({libro.Genero}) | Copias: {libro.CantidadCopias}");
        }
    }
}
