using Microsoft.EntityFrameworkCore;
using Biblioteca_PazReyes_Moloeznik.Modelos;

namespace Biblioteca_PazReyes_Moloeznik.Datos;

public class BibliotecaContext : DbContext
{
    public DbSet<Libro> Libros { get; set; }
    public DbSet<Socio> Socios { get; set; }
    public DbSet<TipoSocio> TiposSocio { get; set; }
    public DbSet<Prestamo> Prestamos { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<EstadoPrestamo> EstadosPrestamo { get; set; }
    public DbSet<EstadoReserva> EstadosReserva { get; set; }
    public DbSet<Multa> Multas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=biblioteca.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // =============================================
        // Libro -> libros
        // =============================================
        modelBuilder.Entity<Libro>(entity =>
        {
            entity.ToTable("libros");
            entity.HasKey(e => e.ISBN);
            entity.Property(e => e.ISBN).HasColumnName("isbn");
            entity.Property(e => e.Titulo).HasColumnName("titulo");
            entity.Property(e => e.Autor).HasColumnName("autor");
            entity.Property(e => e.Genero).HasColumnName("genero");
            entity.Property(e => e.CantidadCopias).HasColumnName("cantidad_copias");
        });

        // =============================================
        // TipoSocio -> tipos_socio
        // =============================================
        modelBuilder.Entity<TipoSocio>(entity =>
        {
            entity.ToTable("tipos_socio");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.MaxLibros).HasColumnName("max_libros");
            entity.Property(e => e.DiasPrestamo).HasColumnName("dias_prestamo");
            entity.Property(e => e.MultaDiaria).HasColumnName("multa_diaria");
        });

        // =============================================
        // Socio -> socios
        // =============================================
        modelBuilder.Entity<Socio>(entity =>
        {
            entity.ToTable("socios");
            entity.HasKey(e => e.NroSocio);
            entity.Property(e => e.NroSocio).HasColumnName("nro_socio").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Apellido).HasColumnName("apellido");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.TipoSocioId).HasColumnName("tipo_socio_id");
            entity.Property(e => e.Activo).HasColumnName("activo");

            entity.HasOne(e => e.TipoSocio)
                .WithMany()
                .HasForeignKey(e => e.TipoSocioId);
        });

        // =============================================
        // EstadoPrestamo -> estados_prestamo
        // =============================================
        modelBuilder.Entity<EstadoPrestamo>(entity =>
        {
            entity.ToTable("estados_prestamo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        // =============================================
        // EstadoReserva -> estados_reserva
        // =============================================
        modelBuilder.Entity<EstadoReserva>(entity =>
        {
            entity.ToTable("estados_reserva");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        // =============================================
        // Prestamo -> prestamos
        // =============================================
        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.ToTable("prestamos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.SocioId).HasColumnName("socio_id");
            entity.Property(e => e.LibroId).HasColumnName("libro_id");
            entity.Property(e => e.FechaPrestamo).HasColumnName("fecha_prestamo");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.FechaDevolucion).HasColumnName("fecha_devolucion");
            entity.Property(e => e.EstadoPrestamoId).HasColumnName("estado_prestamo_id");
            entity.Property(e => e.Renovado).HasColumnName("renovado");

            entity.HasOne(e => e.Socio)
                .WithMany(s => s.Prestamos)
                .HasForeignKey(e => e.SocioId);

            entity.HasOne(e => e.Libro)
                .WithMany(l => l.Prestamos)
                .HasForeignKey(e => e.LibroId);

            entity.HasOne(e => e.EstadoPrestamo)
                .WithMany()
                .HasForeignKey(e => e.EstadoPrestamoId);
        });

        // =============================================
        // Reserva -> reservas
        // =============================================
        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.ToTable("reservas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.SocioId).HasColumnName("socio_id");
            entity.Property(e => e.LibroId).HasColumnName("libro_id");
            entity.Property(e => e.FechaReserva).HasColumnName("fecha_reserva");
            entity.Property(e => e.EstadoReservaId).HasColumnName("estado_reserva_id");

            entity.HasOne(e => e.Socio)
                .WithMany(s => s.Reservas)
                .HasForeignKey(e => e.SocioId);

            entity.HasOne(e => e.Libro)
                .WithMany(l => l.Reservas)
                .HasForeignKey(e => e.LibroId);

            entity.HasOne(e => e.EstadoReserva)
                .WithMany()
                .HasForeignKey(e => e.EstadoReservaId);
        });

        // =============================================
        // Multa -> multas
        // =============================================
        modelBuilder.Entity<Multa>(entity =>
        {
            entity.ToTable("multas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.PrestamoId).HasColumnName("prestamo_id");
            entity.Property(e => e.SocioId).HasColumnName("socio_id");
            entity.Property(e => e.Monto).HasColumnName("monto");
            entity.Property(e => e.Pagada).HasColumnName("pagada");

            entity.HasOne(e => e.Prestamo)
                .WithMany(p => p.Multas)
                .HasForeignKey(e => e.PrestamoId);

            entity.HasOne(e => e.Socio)
                .WithMany(s => s.Multas)
                .HasForeignKey(e => e.SocioId);
        });

        // =============================================
        // SEED DATA
        // =============================================

        // TipoSocio
        modelBuilder.Entity<TipoSocio>().HasData(
            new TipoSocio { Id = 1, Descripcion = "Comun", MaxLibros = 3, DiasPrestamo = 7, MultaDiaria = 150m },
            new TipoSocio { Id = 2, Descripcion = "Estudiante", MaxLibros = 5, DiasPrestamo = 14, MultaDiaria = 75m },
            new TipoSocio { Id = 3, Descripcion = "Docente", MaxLibros = 8, DiasPrestamo = 30, MultaDiaria = 50m }
        );

        // EstadoPrestamo
        modelBuilder.Entity<EstadoPrestamo>().HasData(
            new EstadoPrestamo { Id = 1, Descripcion = "Activo" },
            new EstadoPrestamo { Id = 2, Descripcion = "Devuelto" },
            new EstadoPrestamo { Id = 3, Descripcion = "Vencido" }
        );

        // EstadoReserva
        modelBuilder.Entity<EstadoReserva>().HasData(
            new EstadoReserva { Id = 1, Descripcion = "Pendiente" },
            new EstadoReserva { Id = 2, Descripcion = "Cumplida" },
            new EstadoReserva { Id = 3, Descripcion = "Cancelada" }
        );

        // Libros
        modelBuilder.Entity<Libro>().HasData(
            new Libro { ISBN = "978-0-7475-3269-9", Titulo = "Harry Potter y la Piedra Filosofal", Autor = "J.K. Rowling", Genero = "Ficcion", CantidadCopias = 5 },
            new Libro { ISBN = "978-0-452-28423-4", Titulo = "1984", Autor = "George Orwell", Genero = "Ciencia Ficcion", CantidadCopias = 3 },
            new Libro { ISBN = "978-0-061-12008-4", Titulo = "El Principito", Autor = "Antoine de Saint-Exupery", Genero = "Infantil", CantidadCopias = 4 },
            new Libro { ISBN = "978-84-376-0494-7", Titulo = "Cien Anos de Soledad", Autor = "Gabriel Garcia Marquez", Genero = "Realismo Magico", CantidadCopias = 2 },
            new Libro { ISBN = "978-0-143-11951-5", Titulo = "El Alquimista", Autor = "Paulo Coelho", Genero = "Autoayuda", CantidadCopias = 3 }
        );

        // Socios
        modelBuilder.Entity<Socio>().HasData(
            new Socio { NroSocio = 1, Nombre = "Juan", Apellido = "Perez", Email = "juan.perez@email.com", TipoSocioId = 1, Activo = 1 },
            new Socio { NroSocio = 2, Nombre = "Maria", Apellido = "Garcia", Email = "maria.garcia@email.com", TipoSocioId = 2, Activo = 1 },
            new Socio { NroSocio = 3, Nombre = "Carlos", Apellido = "Lopez", Email = "carlos.lopez@email.com", TipoSocioId = 3, Activo = 1 },
            new Socio { NroSocio = 4, Nombre = "Ana", Apellido = "Martinez", Email = "ana.martinez@email.com", TipoSocioId = 1, Activo = 1 },
            new Socio { NroSocio = 5, Nombre = "Pedro", Apellido = "Ramirez", Email = "pedro.ramirez@email.com", TipoSocioId = 2, Activo = 0 }
        );

        // Prestamos
        modelBuilder.Entity<Prestamo>().HasData(
            new Prestamo { Id = 1, SocioId = 1, LibroId = "978-0-7475-3269-9", FechaPrestamo = "01/06/2026", FechaVencimiento = "08/06/2026", FechaDevolucion = null, EstadoPrestamoId = 1, Renovado = 0 },
            new Prestamo { Id = 2, SocioId = 2, LibroId = "978-0-452-28423-4", FechaPrestamo = "25/05/2026", FechaVencimiento = "08/06/2026", FechaDevolucion = null, EstadoPrestamoId = 1, Renovado = 0 },
            new Prestamo { Id = 3, SocioId = 3, LibroId = "978-0-061-12008-4", FechaPrestamo = "15/05/2026", FechaVencimiento = "14/06/2026", FechaDevolucion = null, EstadoPrestamoId = 3, Renovado = 0 }
        );

        // Multas
        modelBuilder.Entity<Multa>().HasData(
            new Multa { Id = 1, PrestamoId = 3, SocioId = 3, Monto = 250m, Pagada = 0 }
        );
    }
}
