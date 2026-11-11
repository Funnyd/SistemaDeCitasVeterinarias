using System.ComponentModel.DataAnnotations;

namespace SistemaDeCitasVeterinarias.Models;

public class RegistrarDuenoViewModel
{
    // Límites de CLIENTES en ScriptProyecto.sql.
    [Required(ErrorMessage = "Ingrese el nombre del dueño.")]
    [StringLength(20, ErrorMessage = "El nombre admite hasta 20 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "Ingrese un teléfono de contacto.")]
    [Phone(ErrorMessage = "Ingrese un teléfono válido.")]
    [StringLength(20, ErrorMessage = "El teléfono admite hasta 20 caracteres.")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = "";

    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    [StringLength(30, ErrorMessage = "El correo admite hasta 30 caracteres.")]
    public string? Correo { get; set; }

    public bool ParaMascota { get; set; }
}
