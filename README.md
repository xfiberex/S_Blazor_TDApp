# 🧩 Task Management System (S_Blazor_TDApp)
**Sistema de Gestión de Tareas Recurrentes y de Calendario**

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-blue.svg)](https://blazor.net/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**S_Blazor_TDApp** es una aplicación web moderna desarrollada con **Blazor WebAssembly** y **.NET 9** para la gestión integral de tareas recurrentes y calendarios. El sistema permite administrar diferentes tipos de tareas (estáticas, dinámicas y recurrentes) con un sistema robusto de autenticación, autorización por roles y seguimiento de procesos.

## 📋 Tabla de Contenido
- [Características Principales](#-características-principales)
- [Capturas de Pantalla](#-capturas-de-pantalla)
- [Arquitectura del Sistema](#️-arquitectura-del-sistema)
- [Tecnologías Utilizadas](#-tecnologías-utilizadas)
- [Requisitos del Sistema](#️-requisitos-del-sistema)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Contribución](#-contribución)
- [Licencia](#-licencia)

## ✨ Características Principales

- 🔐 **Inicio de sesión con control de roles**  
  Soporte para roles de **Administrador**, **Supervisor** y **Empleado**, con autorización personalizada por vista o menú.

- 📊 **Reportes de procesos y gestión de tareas**  
  Visualización y seguimiento de tareas **recurrentes** y **calendario de actividades**.

- ⚙️ **Configuración de disponibilidad de tareas**  
  Permite definir la disponibilidad específica para cada tarea dentro del sistema.

- 👥 **Gestión de usuarios y roles**  
  Administración de usuarios, con **asignación automática de roles por defecto y generación de codigo**.

- 🧾 **Validaciones en formularios**  
  Validaciones completas en los campos donde el usuario ingresa información para mantener integridad de datos.

- 🔁 **Asignación de días a tareas recurrentes**  
  Posibilidad de definir en qué días específicos deben ejecutarse las tareas repetitivas.

- 🚫 **Control de tareas deshabilitadas**  
  Las tareas deshabilitadas no se mostrarán en los menús de procesos ni en disponibilidad.

- 🛑 **Control de acceso de usuarios inactivos**  
  Usuarios marcados como inactivos o deshabilitados no podrán iniciar sesión en el sistema.

- 🔒 **Autorización por rol**  
  Si un usuario no está autorizado, **no podrá acceder a ciertas páginas o menús** según su rol asignado.

- 🔍 **Búsqueda y filtrado por fecha**  
  Permite buscar y filtrar tareas registradas mediante fechas específicas.

- 📄 **Paginación de datos**  
  Implementación de paginación para mejorar la navegación entre grandes volúmenes de registros.

## 📸 Capturas de Pantalla

![Pantalla de Login](https://github.com/user-attachments/assets/01a2e0cc-0dd9-4f35-b534-ef9c5d2c1ebe)
*Pantalla de inicio de sesión con validación de credenciales*

![Dashboard Principal](https://github.com/user-attachments/assets/e5abd1ab-b8a5-46fb-b460-eb338b79fea1)
*Panel principal con navegación por roles*

![Gestión de Tareas Recurrentes](https://github.com/user-attachments/assets/c39449c3-e04b-4851-8f1f-f6cd48cbb840)
*Administración de tareas recurrentes con configuración de días*

![Calendario de Tareas](https://github.com/user-attachments/assets/31ba143b-8d84-4a50-bb60-860eb5aff148)
*Vista de calendario para programación de tareas*

![Gestión de Usuarios](https://github.com/user-attachments/assets/b7114c3b-57eb-4bce-ab1d-bd103e338366)
*Panel de administración de usuarios y roles*

![Reportes y Registro](https://github.com/user-attachments/assets/7c22d9e2-9408-4089-8b51-1cbc698aca8b)
*Sistema de reportes y registro de procesos*

## 🏗️ Arquitectura del Sistema

El proyecto sigue una arquitectura de **aplicación distribuida** con separación clara de responsabilidades:

```
┌─────────────────────┐    ┌─────────────────────┐    ┌─────────────────────┐
│   Blazor Client     │    │   ASP.NET Server    │    │   SQL Server DB     │
│   (WebAssembly)     │◄──►│   (.NET 9 API)      │◄──►│   (Entity Framework)│
│                     │    │                     │    │                     │
│ • UI Components     │    │ • Controllers       │    │ • Entidades         │
│ • Services          │    │ • Business Logic    │    │ • Stored Procedures │
│ • Authentication    │    │ • Data Access       │    │ • Relaciones        │
└─────────────────────┘    └─────────────────────┘    └─────────────────────┘
```

### Componentes Principales:

- **Cliente Blazor WebAssembly**: Interfaz de usuario reactiva que se ejecuta en el navegador
- **Servidor API**: Backend con controladores RESTful y lógica de negocio
- **Base de Datos**: SQL Server con Entity Framework Core para persistencia
- **Proyecto Compartido**: DTOs y modelos compartidos entre cliente y servidor

---

## 🚀 Tecnologías Utilizadas

### Frontend
- **🔷 Blazor WebAssembly** - Framework SPA que permite usar C# en el frontend
- **🔐 Microsoft.AspNetCore.Components.Authorization** - Sistema de autenticación y autorización
- **💾 Blazored.SessionStorage** - Gestión del almacenamiento de sesión del navegador
- **🌟 CurrieTechnologies.Razor.SweetAlert2** - Alertas y notificaciones modernas

### Backend
- **🛠 ASP.NET Core (.NET 9)** - Framework web moderno y multiplataforma
- **🗃 Entity Framework Core** - ORM para SQL Server con contexto `DbTdappContext`
- **🧭 AutoMapper** - Mapeo automático entre entidades y DTOs
- **📘 Swagger/OpenAPI** - Documentación interactiva de la API
- **⏱ Background Services** - Servicios en segundo plano para tareas programadas

### Base de Datos
- **💽 SQL Server** - Sistema de gestión de base de datos relacional
- **📊 Entity Framework Core** - Code-first con migraciones automáticas

### Herramientas de Desarrollo
- **🔍 Visual Studio 2022** - IDE recomendado para desarrollo
- **🌐 CORS** - Configuración para solicitudes cross-origin
- **🔄 Hot Reload** - Desarrollo ágil con recarga en caliente

## ⚙️ Requisitos del Sistema

### Software Requerido
- **[.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)** - Framework de desarrollo
- **Visual Studio 2022** (17.8 o superior) - IDE recomendado
- **SQL Server** (Express, Developer o superior) - Base de datos
- **Git** - Control de versiones

### Especificaciones Técnicas
- **C# 13.0** - Lenguaje de programación
- **Nullable reference types** habilitado
- **Implicit usings** habilitado

## 📦 Instalación y Configuración

### 1. 📥 Clonar el Repositorio
```bash
git clone https://github.com/xfiberex/S_Blazor_TDApp.git
cd S_Blazor_TDApp
```

### 2. 🗄️ Configurar la Base de Datos

#### Crear la Base de Datos
Ejecuta el script SQL ubicado en `Consultas BD TDApp/001.CREACIÓN DE BD Y TABLAS.sql` para:
- Crear la base de datos `TDApp`
- Crear todas las tablas necesarias
- Insertar datos iniciales (roles, usuarios por defecto)

#### Configurar Cadena de Conexión
Edita el archivo `S_Blazor_TDApp.Server/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=TDApp;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 3. 🚀 Ejecutar la Aplicación

#### Opción A: Desde Visual Studio
1. Abre `S_Blazor_TDApp.sln` en Visual Studio 2022
2. Configura múltiples proyectos de inicio:
   - Click derecho en la solución → "Set Startup Projects"
   - Selecciona "Multiple startup projects"
   - Configura en este orden:
     - `S_Blazor_TDApp.Server` → **Start**
     - `S_Blazor_TDApp.Client` → **Start**
3. Presiona `F5` o click en "Start"

#### Opción B: Desde la Terminal
```bash
# Terminal 1 - Servidor API
cd S_Blazor_TDApp.Server
dotnet run

# Terminal 2 - Cliente Blazor
cd S_Blazor_TDApp.Client  
dotnet run
```

### 4. 🌐 Acceder a la Aplicación

- **Cliente Blazor**: `https://localhost:7264` (Puerto por defecto)
- **API Swagger**: `https://localhost:7125/swagger` (Documentación de la API)

#### Credenciales por Defecto
- **Administrador**: `admin@tdapp.com` / `admin123`
- **Supervisor**: `supervisor@tdapp.com` / `supervisor123`
- **Empleado**: `empleado@tdapp.com` / `empleado123`

## 📁 Estructura del Proyecto

```
S_Blazor_TDApp/
├── 📁 S_Blazor_TDApp.Client/          # Frontend Blazor WebAssembly
│   ├── 📁 Pages/                      # Páginas y componentes de la UI
│   ├── 📁 Services/                   # Servicios para consumir APIs
│   ├── 📁 Layout/                     # Layouts y navegación
│   └── 📁 Extensions/                 # Extensiones y utilidades
├── 📁 S_Blazor_TDApp.Server/          # Backend API REST
│   ├── 📁 Controllers/                # Controladores de la API
│   ├── 📁 DBContext/                  # Contexto de Entity Framework
│   ├── 📁 Entities/                   # Modelos de datos
│   └── 📁 Utilities/                  # Servicios y utilidades
├── 📁 S_Blazor_TDApp.Shared/          # DTOs y modelos compartidos
├── 📁 S_Blazor_TDApp.Password/        # Utilidad de encriptación
└── 📁 Consultas BD TDApp/             # Scripts de base de datos
```

## 🤝 Contribución

¿Quieres contribuir al proyecto? ¡Excelente! Sigue estos pasos:

1. **Fork** el repositorio
2. **Crea** una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. **Push** a la rama (`git push origin feature/AmazingFeature`)
5. **Abre** un Pull Request

### Mejoras Planificadas
- 📋 Mostrar tareas de calendario en el historial de procesos
- 🔧 Submenús para tareas recurrentes y de calendario
- ⚙️ Gestión dinámica de permisos de roles desde el cliente

## 📄 Licencia

Este proyecto está licenciado bajo los términos de la [Licencia MIT](LICENSE). 

```
MIT License - Puedes usar, modificar y distribuir el código libremente, 
manteniendo el aviso de copyright original.
```

---

<div align="center">

**⭐ Si te gusta este proyecto, no olvides darle una estrella en GitHub ⭐**

Desarrollado con ❤️ usando Blazor WebAssembly y .NET 9

</div>
