# Módulo básico de dueños

## Alcance

El dueño se almacena como `Cliente` en la tabla existente `CLIENTES`. Esta entrega permite registrar y consultar dueños para probar su asociación con mascotas. No requiere cambios de esquema. La autenticación, los roles, la edición y la eliminación de dueños quedan fuera de este alcance básico.

- `/Duenos`: listado de nombre, teléfono y correo, con acceso a registrar una mascota para cada dueño.
- `/Duenos/Crear`: formulario con nombre y teléfono obligatorios y correo opcional.
- `/Duenos/Crear?paraMascota=true`: al guardar, abre el formulario de mascota con el dueño seleccionado.
- `/Mascotas/Crear?clienteId={id}`: selecciona al cliente si existe. Un identificador inexistente no se selecciona.

El menú incluye Dueños. El formulario de mascotas tiene un enlace al registro de dueños; el texto avisa que regresará a un formulario nuevo, por lo que conviene registrar primero al dueño.

## Validaciones

`RegistrarDuenoViewModel` limita nombre a 20 caracteres, teléfono a 20 y correo a 30, según `ScriptProyecto.sql`. Valida campos obligatorios, formato telefónico y correo tanto mediante MVC como validación del formulario. El correo vacío se persiste como cadena vacía para respetar `Correo NOT NULL` del script existente. El POST tiene protección antifalsificación y recibe exclusivamente los campos del formulario, sin entidades relacionadas.

## Verificación realizada

- `dotnet build` con SDK 10.0.404: 0 errores y 0 advertencias.
- Ejecución local con `DOTNET_ROLL_FORWARD=Major`, destino del proyecto conservado en .NET 8 y conexión de desarrollo SQL Express existente.
- Navegador integrado: Mascotas → Registrar mascota → Registrar dueño.
- Envío vacío: aparecieron errores en español de nombre y teléfono obligatorios.
- Registro sin correo de `Dueño de prueba` con teléfono ficticio `8888-0000`: guardado y retorno a mascotas con ese dueño seleccionado.
- Registro de `Luna de prueba`, especie Gato, sin raza: guardado asociado al dueño; vínculo conservado al recargar el listado.
- Listado de dueños: mostró el cliente, Sin correo y el enlace para registrar otra mascota.
- Consola del navegador sin errores durante este flujo.

Los dos registros ficticios permanecen en la base local para facilitar las pruebas manuales. Esta comprobación no cubre toda la modificación/eliminación de mascotas ni el flujo de citas.
