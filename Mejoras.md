# Revisión Integral del Sistema (Backend + Frontend + BD)

Proyecto revisado: **S_Blazor_TDApp** (Sistema de Gestión de Tareas Recurrentes y Calendario)

> Estado actualizado y validado contra el código actual del repositorio.
> Leyenda: `[x]` implementado, `[~]` parcial, `[ ]` pendiente.

## 1) Estado Actual (Resumen Ejecutivo)

- [x] La API ya aplica autenticación/autorización global por defecto.
- [x] Existe flujo completo de cuenta: login, registro, confirmación por correo, recuperación, perfil, cambio de contraseña y refresh/revocación.
- [x] El acceso usa JWT con cookies `HttpOnly`/`Secure`, refresh token rotatorio y hash de tokens sensibles en base de datos.
- [x] CORS ya está restringido por configuración y el login tiene rate limiting.
- [x] Existe una base inicial de pruebas automatizadas enfocadas en autenticación.
- [~] Se incorporó `GlobalExceptionHandler` + `ProblemDetails`, pero todavía convive con manejo manual y heterogéneo de errores en controladores.
- [~] La configuración por entorno ya existe, pero el `appsettings.json` base mantiene secretos versionados por conveniencia práctica.
- [~] La UI de login no publica credenciales en pantalla, pero README y script SQL sí mantienen usuarios y credenciales de prueba para uso inmediato.

---

## 2) Hallazgos Clave de la Revisión

### Seguridad y Autenticación

- `Program.cs` aplica `AuthorizeFilter` global; los endpoints públicos de cuenta usan `AllowAnonymous` de forma explícita.
- `JwtBearer` ya toma el access token desde la cookie `tdapp.access_token`.
- `UsuarioController` implementa login, registro con confirmación, olvido/restablecimiento de contraseña, perfil, cambio de contraseña, refresh token con rotación y revocación.
- Los tokens de confirmación, recuperación y refresh se guardan hasheados.
- El endpoint de login ya tiene `EnableRateLimiting("LoginPolicy")`.
- Sigue pendiente una política de contraseñas más robusta y una auditoría explícita de eventos críticos.

### Configuración y Secretos

- Cliente y servidor ya leen URLs por entorno (`appsettings.Development.json`, `appsettings.Production.json` y override local opcional).
- CORS ya no usa `AllowAnyOrigin`; toma orígenes desde `Cors:AllowedOrigins` y permite credenciales.
- El `appsettings.json` base mantiene `ConnectionStrings:cadenaSQLPrimary` y `Jwt:Key` para facilitar el arranque práctico del proyecto.
- El README documenta credenciales de prueba y el script SQL publica usuarios semilla listos para uso inmediato (`SuperAdmin`, `Admin`, `Supervisor`, `Empleado`).

### Backend/API

- Existe `GlobalExceptionHandler` y `AddProblemDetails()`.
- El manejo de errores todavía no es uniforme: muchos controladores siguen retornando `ResponseAPI` con `StatusCode(500)` y el login expone detalle técnico en desarrollo.
- La lógica de negocio sigue concentrada en controladores; aún no hay una capa de aplicación/servicios consolidada.
- La validación todavía está dispersa y no hay una estrategia centralizada tipo `FluentValidation`.
- Se corrigió la dependencia crítica de AutoMapper y actualmente está en `16.1.1`.

### Frontend (Blazor)

- `sessionStorage` ya no almacena access token ni refresh token; solo conserva `InicioSesionDTO` con identidad y claims básicos.
- El cliente envía cookies con `BrowserRequestCredentials.Include` y reintenta peticiones tras refresh cuando recibe `401`.
- La pantalla de login ya no publica cuentas de ejemplo, aunque el proyecto mantiene mucho estilo inline que conviene desacoplar.

### Base de Datos

- El script inicial sigue siendo útil para bootstrap de desarrollo y mantiene índices/restricciones relevantes.
- El mismo script recrea la base e inserta usuarios semilla, por lo que no debe considerarse script de producción.
- Sigue pendiente una estrategia formal de migraciones versionadas (EF Migrations o scripts incrementales).

### Calidad y Operación

- Existe `S_Blazor_TDApp.Tests` con 4 pruebas xUnit enfocadas en autenticación y seguridad.
- Aún faltan pruebas de integración/UI, CI/CD, observabilidad completa y auditoría operativa.

---

## 3) Roadmap Priorizado de Mejoras

## Prioridad Crítica (0-2 semanas)

- [x] Aplicar `[Authorize]` por defecto a controladores y usar `[AllowAnonymous]` solo en login/registro/recuperación.
- [~] Mover secretos a `User Secrets`/variables de entorno (dev) y a vault seguro (prod).
- [x] Restringir CORS a dominios concretos del cliente.
- [~] Eliminar credenciales visibles del README, UI de login y script inicial de producción.
- [x] Agregar rate limiting al endpoint de login.
- [~] Estandarizar errores con `ProblemDetails` + middleware global de excepciones.

## Corto Plazo (2-6 semanas)

### Autenticación y Cuenta de Usuario (JWT completo)

- [x] Inicio de sesión.
- [x] Registro de nuevo usuario con confirmación por correo.
- [x] Olvidé contraseña (generación de token con expiración).
- [x] Restablecimiento de contraseña validando token.
- [x] Sección de perfil de usuario (cambio de contraseña, correo y datos básicos).
- [x] Refresh token con rotación y revocación.

### Seguridad adicional

- [ ] Política de contraseñas (longitud, complejidad, historial opcional).
- [ ] Auditoría de eventos críticos (login, cambio de rol, cambio de clave, eliminación).
- [x] Hardening básico de headers HTTP y cookies donde aplique.

## Mediano Plazo (1-3 meses)

- [ ] Crear capa de servicios/aplicación para sacar lógica de controladores.
- [ ] Añadir validaciones de entrada consistentes con `FluentValidation` o enfoque equivalente.
- [~] Implementar pruebas unitarias (servicios), integración (API) y UI básicas (flujos críticos).
- [~] Introducir paginación, filtrado y ordenado server-side en listados grandes.
- [ ] Implementar caché para catálogos estables (roles, menús, días disponibles).

## Largo Plazo (3+ meses)

- [ ] CI/CD (build, tests, análisis estático, escaneo de secretos y dependencias).
- [ ] Observabilidad completa (OpenTelemetry + dashboard + alertas).
- [~] Versionado de API y documentación Swagger endurecida por entorno.
- [ ] Despliegue con infraestructura separada por ambientes (Dev/QA/Prod) y feature flags.

---

## 4) Mejoras Funcionales Recomendadas para este tipo de Sistema

- [ ] Recordatorios y notificaciones (email y/o push) para tareas próximas a vencer.
- [ ] Reglas avanzadas de recurrencia (semanal, mensual, días hábiles, excepciones).
- [ ] Reasignación masiva de tareas por ausencias o cambios de equipo.
- [ ] Dashboard con KPIs: tareas vencidas, cumplimiento por usuario/rol, tiempos promedio.
- [ ] Exportación de reportes (PDF/Excel) con filtros por fecha, usuario y estado.
- [ ] Historial de cambios por tarea (bitácora completa).

---

## 5) Definición de Hecho (DoD) para cerrar la fase de seguridad

Se considera cerrada la fase de hardening cuando:

- [ ] No existan secretos operativos en los archivos base versionados del repositorio.
- [~] El 100% de endpoints sensibles requieran token y rol cuando corresponda.
- [x] Exista flujo completo de cuenta: registro + verificación + recuperación + cambio de clave.
- [~] Login protegido contra abuso (rate limiting + monitoreo).
- [~] Pruebas mínimas automatizadas en flujos críticos de autenticación.
- [ ] Checklist OWASP ASVS básico validado para el proyecto.

---

## 6) Nota de Implementación de Correo

Para envío de email de pruebas se recomienda en desarrollo:

- `Mailtrap` (rápido para sandbox).
- Alternativa: `Resend` o `SendGrid` en entornos reales.

> El proyecto ya encapsula el correo mediante `IEmailService`; el siguiente paso natural es desacoplar plantillas/proveedor y endurecer configuración por entorno.