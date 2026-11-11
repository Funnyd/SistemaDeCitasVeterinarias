using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeCitasVeterinarias.Data;
using SistemaDeCitasVeterinarias.Models;

namespace SistemaDeCitasVeterinarias.Controllers;

public class MascotasController(ContextoClinica context) : Controller
{
    public async Task<IActionResult> Index()
        => View(await context.Mascotas.AsNoTracking().Include(m => m.Cliente)
            .OrderBy(m => m.Nombre).ToListAsync());

    public async Task<IActionResult> Citas(int id)
    {
        var mascota = await context.Mascotas.AsNoTracking().Include(m => m.Cliente)
            .Include(m => m.Citas).ThenInclude(c => c.Veterinario)
            .SingleOrDefaultAsync(m => m.Id == id);
        return mascota is null ? NotFound() : View(mascota);
    }

    public async Task<IActionResult> Crear(int? clienteId)
    {
        var seleccionado = clienteId.HasValue && await context.Clientes.AnyAsync(c => c.Id == clienteId.Value)
            ? clienteId.Value : 0;
        await CargarClientes(seleccionado);
        return View(new Mascota { ClienteId = seleccionado });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear([Bind("Nombre,Especie,Raza,ClienteId")] Mascota mascota)
    {
        await Validar(mascota);
        if (!ModelState.IsValid)
        {
            await CargarClientes(mascota.ClienteId);
            return View(mascota);
        }
        context.Mascotas.Add(mascota);
        await context.SaveChangesAsync();
        TempData["Mensaje"] = "Mascota registrada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var mascota = await context.Mascotas.FindAsync(id);
        if (mascota is null) return NotFound();
        await CargarClientes(mascota.ClienteId);
        return View(mascota);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, [Bind("Nombre,Especie,Raza,ClienteId")] Mascota datos)
    {
        var mascota = await context.Mascotas.FindAsync(id);
        if (mascota is null) return NotFound();
        await Validar(datos);
        if (!ModelState.IsValid)
        {
            datos.Id = id;
            await CargarClientes(datos.ClienteId);
            return View(datos);
        }
        mascota.Nombre = datos.Nombre;
        mascota.Especie = datos.Especie;
        mascota.Raza = datos.Raza;
        mascota.ClienteId = datos.ClienteId;
        await context.SaveChangesAsync();
        TempData["Mensaje"] = "Mascota actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        var mascota = await context.Mascotas.AsNoTracking().Include(m => m.Cliente)
            .SingleOrDefaultAsync(m => m.Id == id);
        if (mascota is null) return NotFound();
        ViewBag.TieneCitas = await context.Citas.AnyAsync(c => c.MascotaId == id);
        return View(mascota);
    }

    [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        var mascota = await context.Mascotas.FindAsync(id);
        if (mascota is null) return NotFound();
        if (await context.Citas.AnyAsync(c => c.MascotaId == id))
        {
            TempData["Error"] = "No se puede eliminar una mascota con citas asociadas, incluso canceladas.";
            return RedirectToAction(nameof(Eliminar), new { id });
        }
        context.Mascotas.Remove(mascota);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Una cita puede haberse creado después de comprobar las relaciones.
            TempData["Error"] = "No se pudo eliminar la mascota. Compruebe si tiene registros asociados e inténtelo de nuevo.";
            return RedirectToAction(nameof(Eliminar), new { id });
        }
        TempData["Mensaje"] = "Mascota eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarClientes(int? seleccionado = null)
        => ViewBag.Clientes = new SelectList(await context.Clientes.AsNoTracking()
            .OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre", seleccionado);

    private async Task Validar(Mascota mascota)
    {
        mascota.Nombre = mascota.Nombre?.Trim() ?? "";
        mascota.Especie = mascota.Especie?.Trim() ?? "";
        mascota.Raza = string.IsNullOrWhiteSpace(mascota.Raza) ? null : mascota.Raza.Trim();
        // El script SQL existente limita estos campos a varchar(20).
        if (mascota.Nombre.Length is 0 or > 20)
            ModelState.AddModelError(nameof(Mascota.Nombre), "El nombre debe tener entre 1 y 20 caracteres.");
        if (mascota.Especie.Length is 0 or > 20)
            ModelState.AddModelError(nameof(Mascota.Especie), "La especie debe tener entre 1 y 20 caracteres.");
        if (mascota.Raza?.Length > 20)
            ModelState.AddModelError(nameof(Mascota.Raza), "La raza admite hasta 20 caracteres.");
        if (!await context.Clientes.AnyAsync(c => c.Id == mascota.ClienteId))
            ModelState.AddModelError(nameof(Mascota.ClienteId), "Seleccione un cliente registrado.");
        // Compatible con Raza NOT NULL en el script SQL actual.
        mascota.Raza ??= "";
    }
}
