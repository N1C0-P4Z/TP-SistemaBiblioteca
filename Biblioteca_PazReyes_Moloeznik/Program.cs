using Biblioteca_PazReyes_Moloeznik.Datos;
using Biblioteca_PazReyes_Moloeznik.Negocio;

namespace Biblioteca_PazReyes_Moloeznik;

class Program
{
    static void Main(string[] args)
    {
        using var context = new BibliotecaContext();
        context.Database.EnsureCreated();

        var service = new BibliotecaService(context);

        string? opcion;
        do
        {
            Console.Clear();
            Console.WriteLine("=== SISTEMA DE BIBLIOTECA ===");
            Console.WriteLine("1. Prestar libro");
            Console.WriteLine("2. Devolver libro");
            Console.WriteLine("3. Reservar libro");
            Console.WriteLine("4. Detalle de socio");
            Console.WriteLine("5. Libros mas prestados");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opcion: ");
            opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    service.PrestarLibro();
                    break;
                case "2":
                    service.DevolverLibro();
                    break;
                case "3":
                    service.ReservarLibro();
                    break;
                case "4":
                    service.MostrarDetalleSocio();
                    break;
                case "5":
                    service.MostrarLibrosMasPrestados();
                    break;
                case "6":
                    Console.WriteLine("Gracias por usar el sistema.");
                    break;
                default:
                    Console.WriteLine("Opcion invalida.");
                    break;
            }

            if (opcion != "6")
            {
                Console.WriteLine();
                Console.Write("Presione una tecla para continuar...");
                Console.ReadKey();
            }
        } while (opcion != "6");
    }
}
