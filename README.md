# 🧩 Task Management System  
**Gestor de Tareas Estáticas, Dinámicas y Recurrentes**

**Task Management System (S_Blazor_TDApp)** es una solución moderna para la gestión de tareas que combina el poder de **Blazor WebAssembly** y **.NET 9**, permitiendo construir aplicaciones web interactivas, escalables y fáciles de mantener. Está diseñada para manejar tareas de distintos tipos: estáticas, dinámicas o recurrentes, con una arquitectura robusta, flexible y extensible.

---

## 🛠️ Características y Funcionalidades

- 🔐 **Inicio de sesión con control de roles**  
  Soporte para roles de **Administrador**, **Supervisor** y **Empleado**, con autorización personalizada por vista o menú.

- 📊 **Reportes de procesos y gestión de tareas**  
  Visualización y seguimiento de tareas **recurrentes** y **calendario de actividades**.

- ⚙️ **Configuración de disponibilidad de tareas**  
  Permite definir la disponibilidad específica para cada tarea dentro del sistema.

- 👥 **Gestión de usuarios y roles**  
  Administración de usuarios, con **asignación automática de roles por defecto**.

- 🧾 **Validaciones en formularios**  
  Validaciones completas en los campos donde el usuario ingresa información para mantener integridad de datos.

- 🔁 **Asignación de días a tareas recurrentes**  
  Posibilidad de definir en qué días específicos deben ejecutarse las tareas repetitivas.

- 🚫 **Control de tareas deshabilitadas**  
  Las tareas deshabilitadas no se mostrarán en los menús de procesos ni en disponibilidad.

- 🛑 **Control de acceso de usuarios inactivos**  
  Usuarios marcados como inactivos no podrán iniciar sesión en el sistema.

- 🔒 **Autorización por rol**  
  Si un usuario no está autorizado, **no podrá acceder a ciertas páginas o menús** según su rol asignado.

- 🔍 **Búsqueda y filtrado por fecha**  
  Permite buscar y filtrar tareas registradas mediante fechas específicas.

- 📄 **Paginación de datos**  
  Implementación de paginación para mejorar la navegación entre grandes volúmenes de registros.

---
## 🚀 Tecnologías

- **🔷 Blazor WebAssembly**  
  Frontend ejecutado directamente en el navegador, brindando una experiencia dinámica sin necesidad de plugins.

- **🛠 ASP.NET Core (.NET 9)**  
  Backend robusto con API RESTful, encargado de la lógica de negocio y la comunicación cliente-servidor.

- **🗃 Entity Framework Core**  
  Gestión de datos en SQL Server mediante `DbTdappContext`, facilitando las operaciones CRUD.

- **⏱ Servicios en Segundo Plano**  
  El servicio `TareaExpiracionService` procesa tareas programadas o de expiración de forma continua.

- **🧭 AutoMapper**  
  Mapeo eficiente entre entidades y modelos de vista usando perfiles configurables.

- **📘 Swagger UI**  
  Documentación interactiva de la API disponible en entorno de desarrollo.

- **🌐 Configuración CORS**  
  Política abierta que permite solicitudes desde cualquier origen, ideal para integración multiplataforma.

---

## ⚙️ Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- C# 13.0
- Visual Studio 2022 o superior
- SQL Server

---

## 📦 Instalación y Ejecución

### 1. Clonar el repositorio
`bash git clone https://github.com/tu-usuario/S_Blazor_TDApp.git`

### 2. Configurar la base de datos
- Crea la BD, Tablas e inserciones por defecto.
- Abre el proyecto con Visual Studio y edita el archivo appsettings.json dentro de S_Blazor_TDApp.Server y ajusta la cadena de conexión para tu instancia de SQL Server.

### 3. Ejecutar la solución
- Ejecuta primero el Server (S_Blazor_TDApp.Server), el cual tendra la UI de Swagger para probar las APIs.
- Despues ejecuta el Cliente (S_Blazor_TDApp.Client), iniciara la interfaz del proyecto hacia el login.
 
## Licencia
- Este proyecto se distribuye bajo la licencia MIT. Consulta el archivo LICENSE para más detalles.
