-- =============================================
-- Script SQL para el Sistema de Biblioteca
-- Ejecutar: sqlite3 biblioteca.db < biblioteca.sql
-- =============================================

CREATE TABLE IF NOT EXISTS tipos_socio (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    descripcion TEXT NOT NULL,
    max_libros INTEGER NOT NULL,
    dias_prestamo INTEGER NOT NULL,
    multa_diaria REAL NOT NULL
);

CREATE TABLE IF NOT EXISTS estados_prestamo (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    descripcion TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS estados_reserva (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    descripcion TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS libros (
    isbn TEXT PRIMARY KEY,
    titulo TEXT NOT NULL,
    autor TEXT NOT NULL,
    genero TEXT NOT NULL,
    cantidad_copias INTEGER NOT NULL
);

CREATE TABLE IF NOT EXISTS socios (
    nro_socio INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    apellido TEXT NOT NULL,
    email TEXT NOT NULL,
    tipo_socio_id INTEGER NOT NULL,
    activo INTEGER NOT NULL,
    FOREIGN KEY (tipo_socio_id) REFERENCES tipos_socio(id)
);

CREATE TABLE IF NOT EXISTS prestamos (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    socio_id INTEGER NOT NULL,
    libro_id TEXT NOT NULL,
    fecha_prestamo TEXT NOT NULL,
    fecha_vencimiento TEXT NOT NULL,
    fecha_devolucion TEXT,
    estado_prestamo_id INTEGER NOT NULL,
    renovado INTEGER NOT NULL,
    FOREIGN KEY (socio_id) REFERENCES socios(nro_socio),
    FOREIGN KEY (libro_id) REFERENCES libros(isbn),
    FOREIGN KEY (estado_prestamo_id) REFERENCES estados_prestamo(id)
);

CREATE TABLE IF NOT EXISTS reservas (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    socio_id INTEGER NOT NULL,
    libro_id TEXT NOT NULL,
    fecha_reserva TEXT NOT NULL,
    estado_reserva_id INTEGER NOT NULL,
    FOREIGN KEY (socio_id) REFERENCES socios(nro_socio),
    FOREIGN KEY (libro_id) REFERENCES libros(isbn),
    FOREIGN KEY (estado_reserva_id) REFERENCES estados_reserva(id)
);

CREATE TABLE IF NOT EXISTS multas (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    prestamo_id INTEGER NOT NULL,
    socio_id INTEGER NOT NULL,
    monto REAL NOT NULL,
    pagada INTEGER NOT NULL,
    FOREIGN KEY (prestamo_id) REFERENCES prestamos(id),
    FOREIGN KEY (socio_id) REFERENCES socios(nro_socio)
);

-- =============================================
-- DATOS INICIALES
-- =============================================

-- Tipos de Socio
INSERT OR IGNORE INTO tipos_socio (id, descripcion, max_libros, dias_prestamo, multa_diaria) VALUES (1, 'Comun', 3, 7, 150.0);
INSERT OR IGNORE INTO tipos_socio (id, descripcion, max_libros, dias_prestamo, multa_diaria) VALUES (2, 'Estudiante', 5, 14, 75.0);
INSERT OR IGNORE INTO tipos_socio (id, descripcion, max_libros, dias_prestamo, multa_diaria) VALUES (3, 'Docente', 8, 30, 50.0);

-- Estados de Prestamo
INSERT OR IGNORE INTO estados_prestamo (id, descripcion) VALUES (1, 'Activo');
INSERT OR IGNORE INTO estados_prestamo (id, descripcion) VALUES (2, 'Devuelto');
INSERT OR IGNORE INTO estados_prestamo (id, descripcion) VALUES (3, 'Vencido');

-- Estados de Reserva
INSERT OR IGNORE INTO estados_reserva (id, descripcion) VALUES (1, 'Pendiente');
INSERT OR IGNORE INTO estados_reserva (id, descripcion) VALUES (2, 'Cumplida');
INSERT OR IGNORE INTO estados_reserva (id, descripcion) VALUES (3, 'Cancelada');

-- Libros
INSERT OR IGNORE INTO libros (isbn, titulo, autor, genero, cantidad_copias) VALUES ('978-0-7475-3269-9', 'Harry Potter y la Piedra Filosofal', 'J.K. Rowling', 'Ficcion', 5);
INSERT OR IGNORE INTO libros (isbn, titulo, autor, genero, cantidad_copias) VALUES ('978-0-452-28423-4', '1984', 'George Orwell', 'Ciencia Ficcion', 3);
INSERT OR IGNORE INTO libros (isbn, titulo, autor, genero, cantidad_copias) VALUES ('978-0-061-12008-4', 'El Principito', 'Antoine de Saint-Exupery', 'Infantil', 4);
INSERT OR IGNORE INTO libros (isbn, titulo, autor, genero, cantidad_copias) VALUES ('978-84-376-0494-7', 'Cien Anos de Soledad', 'Gabriel Garcia Marquez', 'Realismo Magico', 2);
INSERT OR IGNORE INTO libros (isbn, titulo, autor, genero, cantidad_copias) VALUES ('978-0-143-11951-5', 'El Alquimista', 'Paulo Coelho', 'Autoayuda', 3);

-- Socios
INSERT OR IGNORE INTO socios (nro_socio, nombre, apellido, email, tipo_socio_id, activo) VALUES (1, 'Juan', 'Perez', 'juan.perez@email.com', 1, 1);
INSERT OR IGNORE INTO socios (nro_socio, nombre, apellido, email, tipo_socio_id, activo) VALUES (2, 'Maria', 'Garcia', 'maria.garcia@email.com', 2, 1);
INSERT OR IGNORE INTO socios (nro_socio, nombre, apellido, email, tipo_socio_id, activo) VALUES (3, 'Carlos', 'Lopez', 'carlos.lopez@email.com', 3, 1);
INSERT OR IGNORE INTO socios (nro_socio, nombre, apellido, email, tipo_socio_id, activo) VALUES (4, 'Ana', 'Martinez', 'ana.martinez@email.com', 1, 1);
INSERT OR IGNORE INTO socios (nro_socio, nombre, apellido, email, tipo_socio_id, activo) VALUES (5, 'Pedro', 'Ramirez', 'pedro.ramirez@email.com', 2, 0);

-- Prestamos
INSERT OR IGNORE INTO prestamos (id, socio_id, libro_id, fecha_prestamo, fecha_vencimiento, fecha_devolucion, estado_prestamo_id, renovado) VALUES (1, 1, '978-0-7475-3269-9', '01/06/2026', '08/06/2026', NULL, 1, 0);
INSERT OR IGNORE INTO prestamos (id, socio_id, libro_id, fecha_prestamo, fecha_vencimiento, fecha_devolucion, estado_prestamo_id, renovado) VALUES (2, 2, '978-0-452-28423-4', '25/05/2026', '08/06/2026', NULL, 1, 0);
INSERT OR IGNORE INTO prestamos (id, socio_id, libro_id, fecha_prestamo, fecha_vencimiento, fecha_devolucion, estado_prestamo_id, renovado) VALUES (3, 3, '978-0-061-12008-4', '15/05/2026', '14/06/2026', NULL, 3, 0);

-- Multas
INSERT OR IGNORE INTO multas (id, prestamo_id, socio_id, monto, pagada) VALUES (1, 3, 3, 250.0, 0);
