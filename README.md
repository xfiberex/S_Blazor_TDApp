
🧩 Task Management System – Gestor de Tareas Estáticas, Dinámicas y Recurrentes
S_Blazor_TDApp es una solución moderna para la gestión de tareas que aprovecha el poder de Blazor WebAssembly y .NET 9, permitiendo construir aplicaciones web interactivas, escalables y altamente personalizables.

Este sistema está diseñado para manejar tareas estáticas, dinámicas o recurrentes, integrando lo mejor del ecosistema .NET con prácticas actuales de desarrollo frontend y backend.

🚀 Tecnologías y Características
🔷 Blazor WebAssembly
Experiencia de usuario dinámica ejecutándose directamente en el navegador, sin plugins adicionales.

🛠 Backend con ASP.NET Core (.NET 9)
API RESTful sólida que maneja la lógica de negocio y permite una fácil interacción cliente-servidor.

🗃 Entity Framework Core
Acceso a base de datos SQL Server a través del contexto DbTdappContext para operaciones CRUD eficientes.

⏱ Servicios en Segundo Plano
Servicio TareaExpiracionService que gestiona tareas programadas o de expiración de forma continua y automática.

🧭 AutoMapper
Mapeo limpio y rápido entre entidades y modelos de vista mediante MappingProfile.

📘 Swagger
Documentación y pruebas de la API integradas y accesibles desde el entorno de desarrollo.

🌐 Configuración de CORS
Política abierta para permitir solicitudes desde cualquier origen, ideal para entornos distribuidos.

📦 Requisitos
.NET 9 SDK
C# 13.0
SQL Server

Asegúrate de tener instalada la versión adecuada del SDK y configurar la cadena de conexión en appsettings.json.

⚙️ Instalación y Ejecución
Clona el repositorio
git clone https://github.com/tu-usuario/S_Blazor_TDApp.git

Configura la base de datos
Edita appsettings.json con la cadena de conexión de tu instancia SQL Server.

Aplica las migraciones (si es necesario)
Usa el comando adecuado para actualizar la estructura de la base de datos.

Ejecuta la solución
Abre el proyecto con Visual Studio 2022, compílalo y ejecútalo. La UI de Swagger estará disponible en modo desarrollo.

👥 Público Objetivo
Este proyecto está orientado a desarrolladores que buscan crear aplicaciones web modernas y escalables basadas en tecnologías .NET. Gracias a su arquitectura modular y la inclusión de servicios en segundo plano, es ideal para soluciones que requieren flexibilidad, rendimiento y mantenibilidad.
