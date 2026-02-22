# 📌 Revisión Integral del Sistema (Backend + Frontend + BD)

Proyecto revisado: **S_Blazor_TDApp** (Sistema de Gestión de Tareas Recurrentes y Calendario)

## 1) Estado Actual (Resumen Ejecutivo)

- ✅ Existe autenticación con JWT en login.
- ⚠️ La autorización no está aplicada de forma global en todos los controladores.
- ⚠️ Hay configuración sensible expuesta (JWT key y conexiones SQL en `appsettings.json`).
- ⚠️ CORS está abierto a cualquier origen, método y cabecera.
- ⚠️ Se muestran credenciales por defecto en documentación/UI/script SQL.
- ⚠️ Falta flujo completo de cuenta: registro seguro, olvido de contraseña, verificación por token y perfil de usuario.
- ⚠️ No se observan pruebas automatizadas ni pipeline de calidad.

---

## 2) Hallazgos Clave de la Revisión

### Seguridad y Autenticación

- JWT implementado, pero la mayoría de endpoints CRUD no requieren `[Authorize]`.
- Solo `MenuPermisoController` está protegido correctamente por autenticación/rol.
- No hay refresh token ni revocación de sesión.
- No hay limitación de intentos de login (riesgo de fuerza bruta).
- Falta política robusta de contraseña y validación centralizada del modelo en varios endpoints.

### Configuración y Secretos

- Claves sensibles en repositorio (`Jwt:Key`, connection strings, usuario admin por defecto).
- URL de API hardcodeada en el cliente (`https://localhost:7219/`).
- Exceso de permisos CORS (`AllowAnyOrigin/Method/Header`).

### Backend/API

- Manejo de errores heterogéneo (se devuelve `ex.Message` al cliente en varios casos).
- Falta estandarizar respuestas de error (`ProblemDetails`) y logging estructurado.
- No se observan políticas de rate limiting ni protección anti-abuso.
- Validaciones de negocio dispersas en controladores (conviene mover a capa de aplicación/servicio).

### Frontend (Blazor)

- Se guarda sesión/token en `sessionStorage` (correcto para sesión, pero sin refresh/expiración proactiva).
- El login muestra credenciales por defecto en pantalla (debe eliminarse para producción).
- Estilos inline extensivos en páginas; conviene separar estilos para mantenibilidad.

### Base de Datos

- Existe script inicial completo con índices y restricciones útiles.
- Se eliminan/recrean bases completas en script principal (útil en dev, peligroso fuera de entorno controlado).
- Conviene migrar a estrategia de migraciones versionadas (EF migrations o scripts incrementales).

### Calidad y Operación

- No se detectan proyectos de pruebas (unitarias/integración/e2e).
- Sin CI/CD definido para validación automática, seguridad y despliegue.
- Falta observabilidad completa (métricas, trazas, alertas y auditoría de acciones críticas).

---

## 3) Roadmap Priorizado de Mejoras

## 🔴 Prioridad Crítica (0–2 semanas)

- [ ] Aplicar `[Authorize]` por defecto a controladores y usar `[AllowAnonymous]` solo en login/registro/recuperación.
- [ ] Mover secretos a `User Secrets`/variables de entorno (dev) y a vault seguro (prod).
- [ ] Restringir CORS a dominios concretos del cliente.
- [ ] Eliminar credenciales visibles del README, UI de login y script inicial de producción.
- [ ] Agregar rate limiting al endpoint de login.
- [ ] Estandarizar errores con `ProblemDetails` + middleware global de excepciones.

## 🟠 Corto Plazo (2–6 semanas)

### Autenticación y Cuenta de Usuario (JWT completo)

- [X] Inicio de sesión.
- [ ] Registro de nuevo usuario con confirmación por correo.
- [ ] Olvidé contraseña (generación de token con expiración).
- [ ] Restablecimiento de contraseña validando token.
- [ ] Sección de perfil de usuario (cambio de contraseña, correo y datos básicos).
- [ ] Refresh token con rotación y revocación.

### Seguridad adicional

- [ ] Política de contraseñas (longitud, complejidad, historial opcional).
- [ ] Auditoría de eventos críticos (login, cambio de rol, cambio de clave, eliminación).
- [ ] Hardening de headers HTTP y cookies donde aplique.

## 🟡 Mediano Plazo (1–3 meses)

- [ ] Crear capa de servicios/aplicación para sacar lógica de controladores.
- [ ] Añadir validaciones de entrada consistentes con `FluentValidation` o enfoque equivalente.
- [ ] Implementar pruebas unitarias (servicios), integración (API) y UI básicas (flujos críticos).
- [ ] Introducir paginación, filtrado y ordenado server-side en listados grandes.
- [ ] Implementar caché para catálogos estables (roles, menús, días disponibles).

## 🟢 Largo Plazo (3+ meses)

- [ ] CI/CD (build, tests, análisis estático, escaneo de secretos y dependencias).
- [ ] Observabilidad completa (OpenTelemetry + dashboard + alertas).
- [ ] Versionado de API y documentación Swagger endurecida por entorno.
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

- [ ] No existan secretos en repositorio.
- [ ] El 100% de endpoints sensibles requieran token y rol cuando corresponda.
- [ ] Exista flujo completo de cuenta: registro + verificación + recuperación + cambio de clave.
- [ ] Login protegido contra abuso (rate limiting + monitoreo).
- [ ] Pruebas mínimas automatizadas en flujos críticos de autenticación.
- [ ] Checklist OWASP ASVS básico validado para el proyecto.

---

## 6) Nota de Implementación de Correo

Para envío de email de pruebas se recomienda en desarrollo:

- `Mailtrap` (rápido para sandbox).
- Alternativa: `Resend` o `SendGrid` en entornos reales.

> Recomendación: encapsular envío en un servicio (`IEmailService`) con plantillas y proveedor intercambiable.