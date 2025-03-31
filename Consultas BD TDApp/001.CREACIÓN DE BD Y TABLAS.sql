	
CREATE DATABASE DB_TDApp;
GO
USE DB_TDApp;
GO

-- Para eliminación de datos y restablecimiento de tablas --
--DELETE FROM Usuarios;
--DBCC CHECKIDENT ('Usuarios', RESEED, 0);

-- Roles --
CREATE TABLE Rol (
    RolId INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME NULL
);
GO

-- Inserciones por defecto para los roles --
--INSERT INTO Rol(NombreRol, Descripcion) VALUES 
--('Administrador', 'Acceso total'),
--('Supervisor', 'Acceso parcial'),
--('Empleado', 'Acceso especifico');
--GO

-- Usuarios --
CREATE TABLE Usuarios (
    UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
	Codigo NVARCHAR(100) NOT NULL, -- Codigo de 5 digitos
    NombreUsuario NVARCHAR(100) NOT NULL,
	NombreCompleto NVARCHAR(100) NOT NULL,
    Clave NVARCHAR(255) NOT NULL, -- Almacena la contraseña hasheada
    Email NVARCHAR(150) NULL,
    RolId INT NOT NULL,               -- Columna para relacionar con la tabla Rol
    Activo BIT NOT NULL DEFAULT 1,    -- 1 = activo, 0 = inactivo
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME NULL,
    CONSTRAINT FK_Usuarios_Rol FOREIGN KEY (RolId)
        REFERENCES Rol(RolId)
);
GO

-- INSERCIONES DE USUARIOS PARA PRUEBAS --
--INSERT INTO Usuarios (Codigo, NombreUsuario, NombreCompleto, Clave, Email, RolId, Activo)
--VALUES
--  ('49382', 'juanp',   'Juan Perez',         'pass123', 'juan.perez@example.com',        1, 1),
--  ('27541', 'mariaL',  'Maria Lopez',        'pass123', 'maria.lopez@example.com',       2, 0),
--  ('93618', 'carlosG', 'Carlos Garcia',      'pass123', 'carlos.garcia@example.com',     3, 0),
--  ('15729', 'anaM',    'Ana Martinez',       'pass123', 'ana.martinez@example.com',      1, 1),
--  ('82450', 'luisR',   'Luis Rodriguez',     'pass123', 'luis.rodriguez@example.com',    2, 1),
--  ('31976', 'sofiaC',  'Sofia Castillo',     'pass123', 'sofia.castillo@example.com',    3, 0),
--  ('56023', 'pabloD',  'Pablo Diaz',         'pass123', 'pablo.diaz@example.com',        1, 1),
--  ('10854', 'carlaF',  'Carla Fernandez',    'pass123', 'carla.fernandez@example.com',   2, 1),
--  ('76291', 'ricardoM','Ricardo Morales',    'pass123', 'ricardo.morales@example.com',   3, 0),
--  ('64537', 'lauraS',  'Laura Sanchez',      'pass123', 'laura.sanchez@example.com',     1, 1);
--GO


-- Tabla para tareas de calendario
CREATE TABLE Tareas_Calendario (
    TareaId INT IDENTITY(1,1) PRIMARY KEY,
    NombreTarea NVARCHAR(100) NOT NULL,
    DescripcionTarea NVARCHAR(250) NULL,
    Habilitado BIT NOT NULL DEFAULT 1,
    Fecha DATETIME NOT NULL,
	Hora DATETIME NOT NULL
);
GO

-- Insercion de prueba para aumentar datos de Tareas_Calendario --
--INSERT INTO Tareas_Calendario (NombreTarea, DescripcionTarea, Habilitado, Fecha, Hora) VALUES
--('Revisión de correos', 'Revisar y responder correos pendientes', 1, '2025-02-21 00:00:00', '2025-02-21 09:00:00'),
--('Actualización de reportes', 'Actualizar reportes de ventas mensuales', 1, '2025-02-21 00:00:00', '2025-02-21 10:00:00'),
--('Backup de base de datos', 'Realizar backup completo de la base de datos', 1, '2025-02-21 00:00:00', '2025-02-21 02:00:00'),
--('Limpieza de sistema', 'Eliminar archivos temporales y limpiar el sistema', 1, '2025-02-21 00:00:00', '2025-02-21 01:00:00'),
--('Sincronización de servidores', 'Sincronizar datos entre servidores principales y secundarios', 1, '2025-02-21 00:00:00', '2025-02-21 12:00:00'),
--('Monitoreo de red', 'Verificar el estado de la red y conexiones', 1, '2025-02-21 00:00:00', '2025-02-21 08:00:00'),
--('Actualización de software', 'Actualizar aplicaciones y sistemas operativos', 1, '2025-02-21 00:00:00', '2025-02-21 14:00:00'),
--('Reporte de incidencias', 'Elaborar reporte de incidencias y resolver problemas', 1, '2025-02-21 00:00:00', '2025-02-21 11:00:00'),
--('Reunión de equipo', 'Coordinar y asistir a reunión de equipo', 1, '2025-02-21 00:00:00', '2025-02-21 16:00:00'),
--('Mantenimiento de equipos', 'Realizar mantenimiento preventivo a equipos', 1, '2025-02-21 00:00:00', '2025-02-21 18:00:00');
--GO

-- Tabla para tareas recurrentes --
CREATE TABLE Tareas_Recurrentes (
	TareaRecurrId INT IDENTITY(1,1) PRIMARY KEY,
	NombreTareaRecurr NVARCHAR(100) NOT NULL,
	DescripcionTareaRecurr NVARCHAR(100) NOT NULL,
	Recurrente BIT NOT NULL DEFAULT 1,
	HoraDesde DATETIME NOT NULL,
	HorasHasta DATETIME NOT NULL,
	TiempoEjecucion INT NOT NULL,
	CantidadEjecuciones INT NOT NULL,
	Estado BIT NOT NULL DEFAULT 1,
	FechaUltimaRenovacion DATETIME NOT NULL DEFAULT GETDATE(),
	EstadoExpiracion BIT NOT NULL DEFAULT 1
);
GO

-- Insercion de prueba para aumentar datos de Tareas_Recurrentes --
--INSERT INTO Tareas_Recurrentes (NombreTareaRecurr, DescripcionTareaRecurr, Recurrente, HoraDesde, HorasHasta,
--    TiempoEjecucion, CantidadEjecuciones, Estado) 
--VALUES
--('Revisión de correos', 'Revisar y responder correos pendientes', 1, '2025-02-21 09:00:00', '2025-02-21 09:30:00', 30, 1, 1),
--('Actualización de reportes', 'Actualizar reportes de ventas mensuales', 1, '2025-02-21 10:00:00', '2025-02-21 11:00:00', 60, 1, 1),
--('Backup de base de datos', 'Realizar backup completo de la base de datos', 1, '2025-02-21 02:00:00', '2025-02-21 03:00:00', 60, 1, 1),
--('Limpieza de sistema', 'Eliminar archivos temporales y limpiar el sistema', 1, '2025-02-21 01:00:00', '2025-02-21 01:15:00', 15, 1, 1),
--('Sincronización de servidores', 'Sincronizar datos entre servidores principales y secundarios', 1, '2025-02-21 12:00:00', '2025-02-21 12:30:00', 30, 1, 1),
--('Monitoreo de red', 'Verificar el estado de la red y conexiones', 1, '2025-02-21 08:00:00', '2025-02-21 08:45:00', 45, 1, 1),
--('Actualización de software', 'Actualizar aplicaciones y sistemas operativos', 1, '2025-02-21 14:00:00', '2025-02-21 15:30:00', 90, 1, 1),
--('Reporte de incidencias', 'Elaborar reporte de incidencias y resolver problemas', 1, '2025-02-21 11:00:00', '2025-02-21 11:30:00', 30, 1, 1),
--('Reunión de equipo', 'Coordinar y asistir a reunión de equipo', 1, '2025-02-21 16:00:00', '2025-02-21 17:00:00', 60, 1, 1),
--('Mantenimiento de equipos', 'Realizar mantenimiento preventivo a equipos', 1, '2025-02-21 18:00:00', '2025-02-21 19:00:00', 60, 1, 1);
--GO

-- Tabla para los dias --
CREATE TABLE Dias_Disponibles (
	DiaId INT IDENTITY(1,1) PRIMARY KEY,
	NombreDia NVARCHAR(20) NOT NULL,
);
GO

--INSERT INTO Dias_Disponibles (NombreDia) VALUES
--('Lunes'),
--('Martes'),
--('Miércoles'),
--('Jueves'),
--('Viernes'),
--('Sábado'),
--('Domingo');
--GO

-- Tabla para configurar los días en que estará disponible una tarea recurrente
CREATE TABLE Tarea_Dias (
    TareaDiaId INT IDENTITY(1,1) PRIMARY KEY,
    TareaRecurrId INT NOT NULL,
    DiaId INT NOT NULL,
    CONSTRAINT FK_TareaDias_TareasRecurrentes 
        FOREIGN KEY (TareaRecurrId)
        REFERENCES Tareas_Recurrentes(TareaRecurrId),

    CONSTRAINT FK_TareaDias_DiasDisponibles
        FOREIGN KEY (DiaId)
        REFERENCES Dias_Disponibles(DiaId)
);
GO

-- Tabla para registrar los procesos entrantes de la tareas recurrentes --
CREATE TABLE Registro_Procesos (
	ProcesoId INT IDENTITY(1,1) PRIMARY KEY,
	TareaRecurrId INT NOT NULL,
	UsuarioId INT NOT NULL,
	FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
	DescripcionRegistro NVARCHAR(100) NOT NULL,
	CONSTRAINT FK_Registro_Procesos_TareasRecurrentes FOREIGN KEY (TareaRecurrId) REFERENCES Tareas_Recurrentes(TareaRecurrId),
	CONSTRAINT FK_Registro_Procesos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(UsuarioId)
);
GO