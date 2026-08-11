using System.ComponentModel.DataAnnotations;

namespace tp05.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(30, MinimumLength = 4, ErrorMessage = "El nombre de usuario debe tener entre 4 y 30 caracteres.")]
    [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "El usuario sólo puede contener letras, números y guión bajo.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    [RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El nombre sólo puede contener letras y espacios.")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
    [RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = "El apellido sólo puede contener letras y espacios.")]
    public string? Apellido { get; set; }

    [Required(ErrorMessage = "El tipo de usuario es obligatorio.")]
    public string? TipoUsuario { get; set; }
}
