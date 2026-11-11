# Módulo de mascotas

## Alcance del Sprint Backlog

| Código | Implementación | Ruta |
| --- | --- | --- |
| MP-01 | Consultar todas las citas de una mascota, incluyendo su estado y veterinario. | `/Mascotas/Citas/{id}` |
| MP-02 | Registrar nombre, especie, raza opcional y cliente responsable. | `/Mascotas/Crear` |
| MP-03 | Eliminar una mascota sin citas, mediante confirmación. | `/Mascotas/Eliminar/{id}` |
| MP-04 | Modificar los datos de una mascota sin alterar sus citas. | `/Mascotas/Editar/{id}` |

La entrada del módulo es `/Mascotas`, disponible en el menú principal. MP-01 se interpreta como consulta de citas de cada mascota; la consulta general sigue disponible en `/Citas`.

## Ajustes realizados

- Se agregó `MascotasController` y sus vistas Razor, con un formulario compartido.
- Se agregó `Mascota.ClienteId` explícito. EF Core utilizaba una propiedad de sombra con ese nombre; la columna ya existe en el script SQL. La relación pasa a ser obligatoria, de acuerdo con ese script.
- Las operaciones POST utilizan validación antifalsificación y una lista explícita de campos permitidos para evitar modificar identificadores o relaciones de citas por medio del formulario.
- Se verifica que el cliente exista, que nombre y especie no sean vacíos y que nombre, especie y raza no superen 20 caracteres, conforme a `ScriptProyecto.sql`.
- Una raza vacía se guarda como cadena vacía para ser compatible con `Raza NOT NULL` en el script actual.
- Una mascota con citas no se elimina, incluso si están canceladas. Esta decisión conserva el historial y respeta la restricción existente en `ContextoClinica`. También se captura el fallo de persistencia si aparece una relación entre la comprobación y la eliminación.
- Los identificadores inexistentes devuelven HTTP 404.
- No se ejecutaron cambios sobre la base de datos ni se agregaron registros de prueba.

## Requisitos para ejecutar

1. Instalar el SDK de .NET 8 y disponer de SQL Server.
2. Preparar la base con `ScriptProyecto.sql` si no existe; no ejecutar ese script sobre una base existente sin revisar su contenido.
3. Configurar `ConnectionStrings:ConexionSQL` en la configuración local.
4. Disponer de al menos un cliente. El proyecto todavía no tiene una pantalla de registro de clientes.
5. Ejecutar `dotnet restore`, `dotnet build` y `dotnet run --launch-profile http` desde la raíz del proyecto.
6. Abrir `http://localhost:5198/Mascotas`.

Si la base se creó por otro mecanismo, revisar previamente las longitudes, nulabilidad y posibles mascotas sin cliente antes de aplicar migraciones. No se incluyen migraciones existentes en el proyecto.

## Pruebas básicas de aceptación

Ejecutar sobre una base de pruebas, con un cliente y un veterinario activo registrados.

| Caso | Pasos | Resultado esperado | Estado |
| --- | --- | --- | --- |
| MP-02: registro válido | Registrar nombre, especie y cliente; dejar raza vacía. | Mensaje de éxito; mascota visible y persistente al recargar; disponible en Nueva cita. | Pendiente de ejecución |
| MP-02: datos inválidos | Enviar nombre vacío o solo espacios, especie vacía o textos de más de 20 caracteres. | Errores visibles; ningún registro creado. | Pendiente de ejecución |
| MP-02: cliente inválido | Enviar un ClienteId inexistente, incluso manipulando la petición. | Error de cliente; ningún registro creado. | Pendiente de ejecución |
| MP-04: modificación | Cambiar nombre, especie, raza y cliente; guardar y recargar. | Mismo Id y datos actualizados; citas conservadas. | Pendiente de ejecución |
| MP-04: edición inválida | Intentar guardar sin nombre o con cliente inexistente. | Se muestran errores; los datos persistidos no cambian. | Pendiente de ejecución |
| MP-01: sin citas | Abrir Ver citas para una mascota nueva. | Mensaje de lista vacía. | Pendiente de ejecución |
| MP-01: con citas | Agendar una cita desde el módulo existente y abrir Ver citas de esa mascota. | Aparece con fecha, veterinario, motivo y estado; no aparecen citas de otras mascotas. | Pendiente de ejecución |
| MP-03: eliminación | Crear una mascota sin citas, abrir Eliminar y confirmar. | Desaparece del listado y de los selectores de citas. | Pendiente de ejecución |
| MP-03: historial protegido | Intentar eliminar una mascota con citas, también canceladas; repetir enviando POST directamente. | Operación rechazada y mascota conservada. | Pendiente de ejecución |
| Cancelar eliminación | Abrir Eliminar y volver al listado sin confirmar. | Mascota conservada. | Pendiente de ejecución |
| Registro inexistente | Abrir Citas, Editar o Eliminar con un Id inexistente. | HTTP 404. | Pendiente de ejecución |
| Protección del formulario | Enviar un POST sin token antifalsificación. | Petición rechazada sin modificar datos. | Pendiente de ejecución |

## Verificación realizada y límites

Se revisaron los formularios, acciones, relaciones y compatibilidad con el script SQL. Se ejecutó `git diff --check` para revisar el formato del cambio.

No fue posible compilar ni ejecutar pruebas contra SQL Server: `dotnet` no está disponible en la terminal y no se encontró en las ubicaciones habituales de instalación. Los casos anteriores son un plan de pruebas y no constituyen evidencia de ejecución. El módulo requiere esa verificación antes de dar por cumplida la aceptación del sprint.

### Verificación posterior con SDK 10
Se restauraron dependencias y se compiló con SDK 10.0.404: 0 errores y 0 advertencias. La aplicación arrancó en http://localhost:5198 usando DOTNET_ROLL_FORWARD=Major solo en el proceso de prueba, sin cambiar net8.0. La petición GET /Mascotas devolvió HTTP 500 porque SQL Server configurado como localhost no está accesible (SqlException, error 40). Las pruebas funcionales siguen pendientes de configurar la instancia y base de datos correctas.

### Comprobación con SQL Express disponible
Con la conexión de desarrollo localhost\SQLEXPRESS y la base preparada, GET /Mascotas, /Mascotas/Crear, /Citas y /Citas/AgendarCita devolvieron HTTP 200. Las rutas Editar, Eliminar y Citas de mascotas con Id inexistente devolvieron HTTP 404. El listado de mascotas está vacío y el formulario no ofrece clientes registrados. No se ejecutaron todavía operaciones de alta, modificación o eliminación: requieren un cliente de prueba. Estas comprobaciones verifican respuestas HTTP, no una revisión visual de navegador.
