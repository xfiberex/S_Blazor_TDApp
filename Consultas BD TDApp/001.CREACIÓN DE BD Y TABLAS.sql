	
CREATE DATABASE DB_TDApp;
GO
USE DB_TDApp;
GO

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
INSERT INTO Rol(NombreRol, Descripcion) VALUES 
('Administrador', 'Acceso total'),
('Supervisor', 'Acceso parcial'),
('Empleado', 'Acceso especifico');
GO

-- Usuarios --
CREATE TABLE Usuarios (
    UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
	Codigo NVARCHAR(100) NOT NULL, -- Codigo de 5 digitos
    NombreUsuario NVARCHAR(100) NOT NULL,
	NombreCompleto NVARCHAR(100) NOT NULL,
    Clave NVARCHAR(MAX) NOT NULL, -- Almacena la contraseña hasheada
    Email NVARCHAR(150) NULL,
    RolId INT NOT NULL,               -- Columna para relacionar con la tabla Rol
    Activo BIT NOT NULL DEFAULT 1,    -- 1 = activo, 0 = inactivo
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME NULL,
    CONSTRAINT FK_Usuarios_Rol FOREIGN KEY (RolId)
        REFERENCES Rol(RolId)
);
GO

-- INSERCIÓN POR DEFECTO PARA TENER AL ADMINISTRADOR COMO PRIMER USUARIO, NO CAMBIAR NADA, CONTRASEÑA: pass123 --
-- LOS DATOS SOLO PUEDEN CAMBIARSE EN LA PAGINA DESPUES DE INICIAR SESIÓN ---

-- PUEDES EJECUTAR EL PROGRAMA DE CONSOLA QUE ESTA EN EL PROYECTO EN VS (S_Blazor_TDApp.Password), PARA HASHEAR OTRO CONTRASEÑA DESDE FUERA --

INSERT INTO Usuarios (Codigo, NombreUsuario, NombreCompleto, Clave, Email, RolId, Activo)
VALUES ('00001', 'Admin', 'Admin', '2nVKnIgOfbTH4j0ABP2K3/sRcJxvoEB5ZXllMaFCn2HR2ASW1tvqLOPNjapCZ955cBtXQG9/qLCW42PZ4HjeFzhtVQ2fpx3NY
									qCW68pfcegwubyzA1KGBoxOhFmqckeC6o9o8bCw8DgdW5KFi0Yl7TliJa4hgMwsg7xNg1tm4uk=:5ttgsRdFVWFqPdhjgbn3wGs
									lz/FgzUUDBDAVIvPMoOTaRcppD7jVlsqJVkZZ89CJenfxor3DOgQekTjFunBTdw==', 'admin@example.com', 1, 1);
GO

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

-- Tabla para registrar el completado de las tareas de calendario, la tarea solo estara disponible el dia y hora en la que se asigno mediante el usuario --
CREATE TABLE Tareas_Calendario_Completado (
	TareaCompletoId INT IDENTITY(1,1) PRIMARY KEY,
	TareaId INT,
	UsuarioId INT,
	EstadoCompletado BIT NOT NULL DEFAULT 0,
	DescripcionTareaCompletado NVARCHAR(250) NULL,
	Fecha DATETIME NOT NULL DEFAULT GETDATE(),
	CONSTRAINT FK_Tareas_Calendario_Completado_Tareas_Calendario 
        FOREIGN KEY (TareaId)
        REFERENCES Tareas_Calendario(TareaId),
	CONSTRAINT FK_Tareas_Calendario_Completado_Usuarios
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuarios(UsuarioId)
);
GO

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

-- Tabla para los dias --
CREATE TABLE Dias_Disponibles (
	DiaId INT IDENTITY(1,1) PRIMARY KEY,
	NombreDia NVARCHAR(20) NOT NULL,
);
GO

-- INSERCIÓN DE DIAS POR DEFECTO OBLIGATORIO, PARA PODER CONFIGURAR LAS TAREAS RECURRENTES --
INSERT INTO Dias_Disponibles (NombreDia) VALUES
('Lunes'),
('Martes'),
('Miércoles'),
('Jueves'),
('Viernes'),
('Sábado'),
('Domingo');
GO

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