using System.ComponentModel.DataAnnotations;

namespace SistemaDeCitasVeterinarias.Models;

public class Cliente
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Nombre { get; set; } = "";
    [Required, Phone]
    public string Telefono { get; set; } = "";
    [EmailAddress]
    public string? Correo { get; set; }
    public List<Mascota> Mascotas { get; set; } = new();
}

public class Mascota
{
    public int Id { get; set; }
    [Required, StringLength(60)]
    public string Nombre { get; set; } = "";
    [Required]
    public string Especie { get; set; } = "";   // Perro, Gato, Ave...
    public string? Raza { get; set; }
    public Cliente? Cliente { get; set; }
    public List<Cita> Citas { get; set; } = new();
}

public class Veterinario
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public string Nombre { get; set; } = "";
    public string? Especialidad { get; set; }
    public bool Activo { get; set; } = true;
    public List<Cita> Citas { get; set; } = new();
}

public enum EstadoCita { Pendiente, Confirmada, Atendida, Cancelada }

public class Cita
{
    public int Id { get; set; }

    [Required]
    public int MascotaId { get; set; }
    public Mascota? Mascota { get; set; }

    [Required]
    public int VeterinarioId { get; set; }
    public Veterinario? Veterinario { get; set; }

    [Required, DataType(DataType.DateTime)]
    public DateTime FechaHora { get; set; }

    [Required, StringLength(200)]
    public string Motivo { get; set; } = "";   // Vacunación, control, cirugía...

    public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;
    public string? Diagnostico { get; set; }
    public string? Tratamiento { get; set; }
}