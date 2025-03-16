	
CREATE DATABASE DB_TDApp;
GO
USE DB_TDApp;
GO
-- Para eliminaci�n de datos y restablecimiento de tablas --
--DELETE FROM Tarea_Dias;
--DBCC CHECKIDENT ('Tarea_Dias', RESEED, 0);

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
GO

-- Usuarios --
CREATE TABLE Usuarios (
    UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario NVARCHAR(100) NOT NULL,
    Clave NVARCHAR(255) NOT NULL, -- Almacena la contrase�a hasheada
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
--INSERT INTO Usuarios (NombreUsuario, Clave, Email, RolId) VALUES
--('rijimenez', '123', 'rijimenez@gmail.com', 1),
--('caperez', '123', 'caperez@gmail.com', 2),
--('mabaez', '123', 'mabaez@gmail.com', 3);


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
--INSERT INTO Tareas_Calendario (NombreTarea, DescripcionTarea,
--    Habilitado, Fecha, Hora
--)
--VALUES
--('Revisi�n de correos', 'Revisar y responder correos pendientes', 1, '2025-02-21 00:00:00', '2025-02-21 09:00:00'),
--('Actualizaci�n de reportes', 'Actualizar reportes de ventas mensuales', 1, '2025-02-21 00:00:00', '2025-02-21 10:00:00'),
--('Backup de base de datos', 'Realizar backup completo de la base de datos', 1, '2025-02-21 00:00:00', '2025-02-21 02:00:00'),
--('Limpieza de sistema', 'Eliminar archivos temporales y limpiar el sistema', 1, '2025-02-21 00:00:00', '2025-02-21 01:00:00'),
--('Sincronizaci�n de servidores', 'Sincronizar datos entre servidores principales y secundarios', 1, '2025-02-21 00:00:00', '2025-02-21 12:00:00'),
--('Monitoreo de red', 'Verificar el estado de la red y conexiones', 1, '2025-02-21 00:00:00', '2025-02-21 08:00:00'),
--('Actualizaci�n de software', 'Actualizar aplicaciones y sistemas operativos', 1, '2025-02-21 00:00:00', '2025-02-21 14:00:00'),
--('Reporte de incidencias', 'Elaborar reporte de incidencias y resolver problemas', 1, '2025-02-21 00:00:00', '2025-02-21 11:00:00'),
--('Reuni�n de equipo', 'Coordinar y asistir a reuni�n de equipo', 1, '2025-02-21 00:00:00', '2025-02-21 16:00:00'),
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
);
GO

-- Insercion de prueba para aumentar datos de Tareas_Recurrentes --
--INSERT INTO Tareas_Recurrentes (NombreTareaRecurr, DescripcionTareaRecurr, Recurrente, HoraDesde, HorasHasta,
--    TiempoEjecucion, CantidadEjecuciones, Estado ) 
--VALUES
--('Revisi�n de correos', 'Revisar y responder correos pendientes', 1, '2025-02-21 09:00:00', '2025-02-21 09:30:00', 30, 1, 1),
--('Actualizaci�n de reportes', 'Actualizar reportes de ventas mensuales', 1, '2025-02-21 10:00:00', '2025-02-21 11:00:00', 60, 1, 1),
--('Backup de base de datos', 'Realizar backup completo de la base de datos', 1, '2025-02-21 02:00:00', '2025-02-21 03:00:00', 60, 1, 1),
--('Limpieza de sistema', 'Eliminar archivos temporales y limpiar el sistema', 1, '2025-02-21 01:00:00', '2025-02-21 01:15:00', 15, 1, 1),
--('Sincronizaci�n de servidores', 'Sincronizar datos entre servidores principales y secundarios', 1, '2025-02-21 12:00:00', '2025-02-21 12:30:00', 30, 1, 1),
--('Monitoreo de red', 'Verificar el estado de la red y conexiones', 1, '2025-02-21 08:00:00', '2025-02-21 08:45:00', 45, 1, 1),
--('Actualizaci�n de software', 'Actualizar aplicaciones y sistemas operativos', 1, '2025-02-21 14:00:00', '2025-02-21 15:30:00', 90, 1, 1),
--('Reporte de incidencias', 'Elaborar reporte de incidencias y resolver problemas', 1, '2025-02-21 11:00:00', '2025-02-21 11:30:00', 30, 1, 1),
--('Reuni�n de equipo', 'Coordinar y asistir a reuni�n de equipo', 1, '2025-02-21 16:00:00', '2025-02-21 17:00:00', 60, 1, 1),
--('Mantenimiento de equipos', 'Realizar mantenimiento preventivo a equipos', 1, '2025-02-21 18:00:00', '2025-02-21 19:00:00', 60, 1, 1);
--GO

ALTER TABLE Tareas_Recurrentes
ADD FechaUltimaRenovacion DATETIME NOT NULL DEFAULT GETDATE();
GO

-- Tabla para los dias --
CREATE TABLE Dias_Disponibles (
	DiaId INT IDENTITY(1,1) PRIMARY KEY,
	NombreDia NVARCHAR(20) NOT NULL,
);
GO

--INSERT INTO Dias_Disponibles (NombreDia) VALUES
--('Lunes'),
--('Martes'),
--('Mi�rcoles'),
--('Jueves'),
--('Viernes'),
--('S�bado'),
--('Domingo');
--GO


-- Tabla para configurar los d�as en que estar� disponible una tarea recurrente
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

--INSERT INTO Tarea_Dias (TareaRecurrId, NombreTareaRecurrDia, Dia) VALUES
--(1, 'Revisi�n de correos', 'Lunes')

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

--INSERT INTO Usuarios (NombreUsuario, Clave, Email, RolId)
--VALUES
--('Usuario1', '12345', 'usuario1@ejemplo.com', 1),
--('Usuario2', '12345', 'usuario2@ejemplo.com', 2),
--('Usuario3', '12345', 'usuario3@ejemplo.com', 3),
--('Usuario4', '12345', 'usuario4@ejemplo.com', 1),
--('Usuario5', '12345', 'usuario5@ejemplo.com', 2),
--('Usuario6', '12345', 'usuario6@ejemplo.com', 3),
--('Usuario7', '12345', 'usuario7@ejemplo.com', 1),
--('Usuario8', '12345', 'usuario8@ejemplo.com', 2),
--('Usuario9', '12345', 'usuario9@ejemplo.com', 3),
--('Usuario10', '12345', 'usuario10@ejemplo.com', 1);

