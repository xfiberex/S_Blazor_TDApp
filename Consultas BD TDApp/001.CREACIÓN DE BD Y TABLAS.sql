	
-- =====================================================================
-- RECREACIÓN COMPLETA DE LA BASE DE DATOS DB_TDApp
-- Elimina la BD si existe y la vuelve a crear desde cero
-- =====================================================================

USE master;
GO

-- Cierra todas las conexiones activas antes de eliminar
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'DB_TDApp')
BEGIN
    ALTER DATABASE DB_TDApp SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE DB_TDApp;
END
GO

CREATE DATABASE DB_TDApp;
GO
USE DB_TDApp;
GO

SET NOCOUNT ON;
GO

-- =====================================================================
-- ROLES
-- =====================================================================
CREATE TABLE Rol (
    RolId              INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol          NVARCHAR(50)  NOT NULL,
    Descripcion        NVARCHAR(250) NULL,
    Activo             BIT           NOT NULL DEFAULT 1,
    FechaCreacion      DATETIME      NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME      NULL,
    CONSTRAINT UQ_Rol_NombreRol UNIQUE (NombreRol)  -- Un nombre de rol no puede repetirse
);
GO

-- Roles por defecto
-- IMPORTANTE: los RolId quedan fijos por el orden de inserción (1-4).
-- No cambiar el orden; los permisos de RolMenu dependen de estos IDs.
INSERT INTO Rol (NombreRol, Descripcion) VALUES
('Administrador',      'Acceso total'),
('Supervisor',         'Acceso parcial'),
('Empleado',           'Acceso específico'),
('Super_Administrador','Acceso total permanente. No puede ser modificado desde la UI.');
GO

-- =====================================================================
-- USUARIOS
-- =====================================================================
CREATE TABLE Usuarios (
    UsuarioId          INT IDENTITY(1,1) PRIMARY KEY,
    Codigo             NVARCHAR(10)  NOT NULL,           -- Código de hasta 5 dígitos
    NombreUsuario      NVARCHAR(100) NOT NULL,
    NombreCompleto     NVARCHAR(100) NOT NULL,
    Clave              NVARCHAR(MAX) NOT NULL,            -- Contraseña hasheada
    Email              NVARCHAR(150) NULL,
    CorreoConfirmado   BIT           NOT NULL DEFAULT 0,
    TokenConfirmacion  NVARCHAR(250) NULL,
    TokenRecuperacion  NVARCHAR(250) NULL,
    FechaExpiracionToken DATETIME    NULL,
    RefreshToken       NVARCHAR(250) NULL,
    RefreshTokenExpiracion DATETIME  NULL,
    RolId              INT           NOT NULL,
    Activo             BIT           NOT NULL DEFAULT 1,
    FechaCreacion      DATETIME      NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME      NULL,
    CONSTRAINT FK_Usuarios_Rol       FOREIGN KEY (RolId) REFERENCES Rol(RolId),
    CONSTRAINT UQ_Usuarios_Codigo    UNIQUE (Codigo),         -- Código único por usuario
    CONSTRAINT UQ_Usuarios_NombreUsr UNIQUE (NombreUsuario),  -- Nombre de usuario único
    CONSTRAINT UQ_Usuarios_Email     UNIQUE (Email)           -- Email único (cuando se provee)
);
GO

-- Índice en RolId para agilizar consultas de usuarios por rol
CREATE INDEX IX_Usuarios_RolId ON Usuarios (RolId);
GO

-- INSERCIÓN POR DEFECTO — ADMINISTRADOR
-- Los datos pueden cambiarse desde la página después de iniciar sesión.
-- Para hashear otra contraseña ejecuta el proyecto de consola S_Blazor_TDApp.Password.
INSERT INTO Usuarios (Codigo, NombreUsuario, NombreCompleto, Clave, Email, CorreoConfirmado, RolId, Activo)
VALUES (
    '00001', 'Admin', 'Admin',
    '2nVKnIgOfbTH4j0ABP2K3/sRcJxvoEB5ZXllMaFCn2HR2ASW1tvqLOPNjapCZ955cBtXQG9/qLCW42PZ4HjeFzhtVQ2fpx3NYqCW68pfcegwubyzA1KGBoxOhFmqckeC6o9o8bCw8DgdW5KFi0Yl7TliJa4hgMwsg7xNg1tm4uk=:5ttgsRdFVWFqPdhjgbn3wGslz/FgzUUDBDAVIvPMoOTaRcppD7jVlsqJVkZZ89CJenfxor3DOgQekTjFunBTdw==',
    'admin@example.com', 1, 1, 1
);
GO

-- =====================================================================
-- TAREAS DE CALENDARIO
-- =====================================================================
CREATE TABLE Tareas_Calendario (
    TareaId          INT IDENTITY(1,1) PRIMARY KEY,
    NombreTarea      NVARCHAR(100) NOT NULL,
    DescripcionTarea NVARCHAR(250) NULL,
    Habilitado       BIT           NOT NULL DEFAULT 1,
    Fecha            DATETIME      NOT NULL,
    Hora             DATETIME      NOT NULL
);
GO

-- Registra el completado de una tarea de calendario por usuario
CREATE TABLE Tareas_Calendario_Completado (
    TareaCompletoId            INT IDENTITY(1,1) PRIMARY KEY,
    TareaId                    INT           NOT NULL,   -- FK obligatoria
    UsuarioId                  INT           NOT NULL,   -- FK obligatoria
    EstadoCompletado           BIT           NOT NULL DEFAULT 0,
    DescripcionTareaCompletado NVARCHAR(250) NULL,
    Fecha                      DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_TarCalComp_TarCal
        FOREIGN KEY (TareaId)   REFERENCES Tareas_Calendario(TareaId),
    CONSTRAINT FK_TarCalComp_Usuarios
        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(UsuarioId)
);
GO

-- Índices en FK para mejorar rendimiento en consultas de completado
CREATE INDEX IX_TarCalComp_TareaId   ON Tareas_Calendario_Completado (TareaId);
CREATE INDEX IX_TarCalComp_UsuarioId ON Tareas_Calendario_Completado (UsuarioId);
GO

-- =====================================================================
-- TAREAS RECURRENTES
-- =====================================================================
CREATE TABLE Tareas_Recurrentes (
    TareaRecurrId          INT IDENTITY(1,1) PRIMARY KEY,
    NombreTareaRecurr      NVARCHAR(100) NOT NULL,
    DescripcionTareaRecurr NVARCHAR(100) NULL,           -- Descripción opcional
    Recurrente             BIT           NOT NULL DEFAULT 1,
    HoraDesde              DATETIME      NOT NULL,
    HorasHasta             DATETIME      NOT NULL,
    TiempoEjecucion        INT           NOT NULL,
    CantidadEjecuciones    INT           NOT NULL,
    Estado                 BIT           NOT NULL DEFAULT 1,
    FechaUltimaRenovacion  DATETIME      NOT NULL DEFAULT GETDATE(),
    EstadoExpiracion       BIT           NOT NULL DEFAULT 1
);
GO

-- =====================================================================
-- DÍAS DISPONIBLES
-- =====================================================================
CREATE TABLE Dias_Disponibles (
    DiaId     INT IDENTITY(1,1) PRIMARY KEY,
    NombreDia NVARCHAR(20) NOT NULL,
    CONSTRAINT UQ_Dias_NombreDia UNIQUE (NombreDia)  -- Evita días duplicados
);
GO

-- INSERCIÓN OBLIGATORIA — necesaria para configurar tareas recurrentes
INSERT INTO Dias_Disponibles (NombreDia) VALUES
('Lunes'),
('Martes'),
('Miércoles'),
('Jueves'),
('Viernes'),
('Sábado'),
('Domingo');
GO

-- Configura los días en que estará disponible una tarea recurrente
CREATE TABLE Tarea_Dias (
    TareaDiaId    INT IDENTITY(1,1) PRIMARY KEY,
    TareaRecurrId INT NOT NULL,
    DiaId         INT NOT NULL,
    CONSTRAINT FK_TareaDias_TareasRecurrentes
        FOREIGN KEY (TareaRecurrId) REFERENCES Tareas_Recurrentes(TareaRecurrId),
    CONSTRAINT FK_TareaDias_DiasDisponibles
        FOREIGN KEY (DiaId)         REFERENCES Dias_Disponibles(DiaId),
    CONSTRAINT UQ_TareaDias_TareaRec_Dia UNIQUE (TareaRecurrId, DiaId)  -- Un día no puede repetirse en la misma tarea
);
GO

-- =====================================================================
-- REGISTRO DE PROCESOS
-- =====================================================================
CREATE TABLE Registro_Procesos (
    ProcesoId           INT IDENTITY(1,1) PRIMARY KEY,
    TareaRecurrId       INT           NOT NULL,
    UsuarioId           INT           NOT NULL,
    FechaRegistro       DATETIME      NOT NULL DEFAULT GETDATE(),
    DescripcionRegistro NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_RegProcesos_TareasRecurrentes
        FOREIGN KEY (TareaRecurrId) REFERENCES Tareas_Recurrentes(TareaRecurrId),
    CONSTRAINT FK_RegProcesos_Usuarios
        FOREIGN KEY (UsuarioId)     REFERENCES Usuarios(UsuarioId)
);
GO

-- Índices en FK para agilizar consultas de registros por tarea o usuario
CREATE INDEX IX_RegProcesos_TareaRecurrId ON Registro_Procesos (TareaRecurrId);
CREATE INDEX IX_RegProcesos_UsuarioId     ON Registro_Procesos (UsuarioId);
GO

-- =====================================================================
-- MENÚS DE LA APLICACIÓN
-- Almacena los menús disponibles en la UI
-- =====================================================================
CREATE TABLE Menu (
    MenuId     INT IDENTITY(1,1) PRIMARY KEY,
    NombreMenu NVARCHAR(50)  NOT NULL,
    Ruta       NVARCHAR(100) NOT NULL,
    Icono      NVARCHAR(100) NOT NULL,
    Seccion    NVARCHAR(50)  NOT NULL,
    Orden      INT           NOT NULL DEFAULT 0,
    CONSTRAINT UQ_Menu_Ruta UNIQUE (Ruta)  -- Cada ruta debe ser única en la aplicación
);
GO

INSERT INTO Menu (NombreMenu, Ruta, Icono, Seccion, Orden) VALUES
('Procesos',         'registroProcesos',  'bi bi-house-gear-fill',      'GESTIÓN',        1),
('Recurrentes',      'tareasRecurrentes', 'bi bi-list-task',            'GESTIÓN',        2),
('Disponibilidad',   'tareasDias',        'bi bi-calendar3',            'GESTIÓN',        3),
('Calendario',       'tareasCalendario',  'bi bi-calendar2-range-fill', 'GESTIÓN',        4),
('Usuarios',         'usuarios',          'bi bi-people-fill',          'ADMINISTRACIÓN', 5),
('Roles y Permisos', 'rolesPermisos',     'bi bi-person-fill-gear',     'ADMINISTRACIÓN', 6);
GO

-- =====================================================================
-- PERMISOS DE MENÚ POR ROL
-- Mapea qué menús puede ver cada rol; existencia de fila = tiene acceso
-- =====================================================================
CREATE TABLE RolMenu (
    RolId  INT NOT NULL,
    MenuId INT NOT NULL,
    CONSTRAINT PK_RolMenu      PRIMARY KEY (RolId, MenuId),
    CONSTRAINT FK_RolMenu_Rol  FOREIGN KEY (RolId)  REFERENCES Rol(RolId),
    CONSTRAINT FK_RolMenu_Menu FOREIGN KEY (MenuId) REFERENCES Menu(MenuId)
);
GO

-- Índice en MenuId para consultas inversas (qué roles pueden ver un menú)
CREATE INDEX IX_RolMenu_MenuId ON RolMenu (MenuId);
GO

-- Administrador       (RolId=1) — todos los menús, incluyendo Roles y Permisos
INSERT INTO RolMenu (RolId, MenuId) VALUES (1,1),(1,2),(1,3),(1,4),(1,5),(1,6);

-- Supervisor          (RolId=2) — gestión + usuarios (sin Roles y Permisos)
INSERT INTO RolMenu (RolId, MenuId) VALUES (2,1),(2,2),(2,3),(2,4),(2,5);

-- Empleado            (RolId=3) — solo gestión
INSERT INTO RolMenu (RolId, MenuId) VALUES (3,1),(3,2),(3,3),(3,4);

-- Super_Administrador (RolId=4) — todos los menús (fijo, no modificable)
INSERT INTO RolMenu (RolId, MenuId) VALUES (4,1),(4,2),(4,3),(4,4),(4,5),(4,6);
GO