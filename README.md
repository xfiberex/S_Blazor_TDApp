# 🧩 Task Management System  
**Gestor de Tareas Estáticas, Dinámicas y Recurrentes**

**Task Management System (S_Blazor_TDApp)** es una solución moderna para la gestión de tareas que combina el poder de **Blazor WebAssembly** y **.NET 9**, permitiendo construir aplicaciones web interactivas, escalables y fáciles de mantener. Está diseñada para manejar tareas de distintos tipos: estáticas, dinámicas o recurrentes, con una arquitectura robusta, flexible y extensible.

![image](https://github.com/user-attachments/assets/08c0bae8-4ee9-49df-b21d-922e33f8667c)



---

## 🛠️ Características y Funcionalidades

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

---

## 🚀 Tecnologías Utilizadas

- 🔷 **Blazor WebAssembly**  
  Framework para el frontend que se ejecuta directamente en el navegador, ofreciendo una experiencia SPA (Single Page Application) sin necesidad de plugins.

- 🛠 **ASP.NET Core (.NET 9)**  
  Plataforma moderna y multiplataforma para el desarrollo del backend y servicios API RESTful, encargada de la lógica de negocio y gestión de peticiones.

- 🗃 **Entity Framework Core**  
  ORM para interactuar con **SQL Server** mediante el contexto `DbTdappContext`, facilitando operaciones CRUD de manera eficiente.

- ⏱ **Servicios en Segundo Plano**  
  El servicio `TareaExpiracionService` permite ejecutar procesos periódicos o tareas programadas sin intervención del usuario.

- 🧭 **AutoMapper**  
  Herramienta para mapear automáticamente entre entidades del dominio y modelos de vista, mejorando la separación de responsabilidades.

- 📘 **Swagger UI**  
  Genera documentación interactiva para la API, útil en el entorno de desarrollo para pruebas rápidas y visualización de endpoints.

- 🌟 **SweetAlert2**  
  Librería de alertas modernas y personalizadas, utilizada para mostrar mensajes de validación y errores de forma atractiva.

- 💾 **Blazored.SessionStorage**  
  Permite almacenar y recuperar datos en el **almacenamiento de sesión** como objetos JSON, útil para conservar estados temporales del usuario.

- 🔐 **Microsoft.AspNetCore.Components.Authorization**  
  Manejo del estado de autenticación y autorización del usuario en aplicaciones Blazor, asegurando acceso controlado por rol.

- 🌐 **CORS (Cross-Origin Resource Sharing)**  
  Configuración abierta que permite solicitudes desde cualquier origen, facilitando la integración entre diferentes plataformas y entornos.

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
- Opcional: En visual studio se puede configurar para iniciar varios proyecto de inicio. Siempre debe iniciar el server primero y despues el cliente.
 
## Licencia
- Este proyecto se distribuye bajo la licencia MIT. Consulta el archivo LICENSE para más detalles.
