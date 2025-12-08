using System.ComponentModel.DataAnnotations;

namespace final_project.Models
{
    public class Gasto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 200 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, 999999999.99, ErrorMessage = "El monto debe estar entre $0.01 y $999,999,999.99")]
        [Display(Name = "Monto ($)")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(50, ErrorMessage = "La categoría no puede exceder 50 caracteres")]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha del Gasto")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notas adicionales")]
        public string? Notas { get; set; }

        // Validación personalizada: La fecha no puede ser futura
        public bool EsFechaValida()
        {
            return Fecha <= DateTime.Now;
        }
    }
}