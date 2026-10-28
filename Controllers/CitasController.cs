using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeCitasVeterinarias.Models;
using SistemaDeCitasVeterinarias.Data;
using System;

namespace SistemaDeCitasVeterinarias.Controllers;

public class CitasController : Controller
{
    private readonly ContextoClinica _context;
    public CitasController(ContextoClinica context) => _context = context;

    public async Task<IActionResult> Index(DateTime? fecha, int? veterinarioId)
    {
        var query = _context.Citas
            .Include(c => c.Mascota).ThenInclude(m => m!.Cliente)
            .Include(c => c.Veterinario)
            .AsQueryable();

        if (fecha.HasValue)
            query = query.Where(c => c.FechaHora.Date == fecha.Value.Date);
        if (veterinarioId.HasValue)
            query = query.Where(c => c.VeterinarioId == veterinarioId);

        ViewBag.Veterinarios = new SelectList(
            _context.Veterinarios.Where(v => v.Activo), "Id", "Nombre", veterinarioId);

        return View(await query.OrderBy(c => c.FechaHora).ToListAsync());
    }

    public IActionResult AgendarCita()
    {
        CargarListas();
        return View(new Cita { FechaHora = DateTime.Today.AddDays(1).AddHours(9) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AgendarCita(Cita cita)
    {
        if (cita.FechaHora < DateTime.Now)
            ModelState.AddModelError(nameof(cita.FechaHora), "La fecha debe ser futura.");

        // El veterinario no puede tener dos citas con menos de 30 min de diferencia
        var inicio = cita.FechaHora.AddMinutes(-30);
        var fin = cita.FechaHora.AddMinutes(30);
        bool ocupado = await _context.Citas.AnyAsync(c =>
            c.VeterinarioId == cita.VeterinarioId &&
            c.Estado != EstadoCita.Cancelada &&
            c.FechaHora > inicio && c.FechaHora < fin);

        if (ocupado)
            ModelState.AddModelError(nameof(cita.FechaHora),
                "El veterinario ya tiene una cita en ese horario.");

        if (ModelState.IsValid)
        {
            _context.Add(cita);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        CargarListas(cita);
        return View(cita);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, EstadoCita estado)
    {
        var cita = await _context.Citas.FindAsync(id);
        if (cita == null) return NotFound();
        cita.Estado = estado;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private void CargarListas(Cita? c = null)
    {
        ViewBag.MascotaId = new SelectList(
            _context.Mascotas.Include(m => m.Cliente)
                .Select(m => new { m.Id, Texto = m.Nombre + " (" + m.Cliente!.Nombre + ")" }),
            "Id", "Texto", c?.MascotaId);
        ViewBag.VeterinarioId = new SelectList(
            _context.Veterinarios.Where(v => v.Activo), "Id", "Nombre", c?.VeterinarioId);
    }
}