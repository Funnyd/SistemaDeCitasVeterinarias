using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeCitasVeterinarias.Data;
using SistemaDeCitasVeterinarias.Models;

namespace SistemaDeCitasVeterinarias.Controllers;

public class DuenosController(ContextoClinica context) : Controller
{
    public async Task<IActionResult> Index()
        => View(await context.Clientes.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync());

    public IActionResult Crear(bool paraMascota = false)
        => View(new RegistrarDuenoViewModel { ParaMascota = paraMascota });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(RegistrarDuenoViewModel datos)
    {
        if (!ModelState.IsValid) return View(datos);
        var cliente = new Cliente
        {
            Nombre = datos.Nombre.Trim(),
            Telefono = datos.Telefono.Trim(),
            // Compatible con Correo NOT NULL en el SQL existente.
            Correo = datos.Correo?.Trim() ?? ""
        };
        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();
        TempData["Mensaje"] = "Dueño registrado correctamente.";
        return datos.ParaMascota
            ? RedirectToAction("Crear", "Mascotas", new { clienteId = cliente.Id })
            : RedirectToAction(nameof(Index));
    }
}
