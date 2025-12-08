using System.ComponentModel.DataAnnotations;

namespace final_project.Models
{
    public class Presupuesto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El monto total es obligatorio")]
        [Range(1, 999999999.99, ErrorMessage = "El presupuesto debe estar entre $1 y $999,999,999.99")]
        [Display(Name = "Monto Total del Presupuesto")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal MontoTotal { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaFin { get; set; } = DateTime.Now.AddMonths(1);

        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string? Descripcion { get; set; }

        [Display(Name = "Presupuesto Activo")]
        public bool Activo { get; set; } = true;

        // Propiedades calculadas
        public int DuracionDias => (FechaFin - FechaInicio).Days;
        
        public bool EstaVigente => DateTime.Now >= FechaInicio && DateTime.Now <= FechaFin;
        
        public bool HaFinalizado => DateTime.Now > FechaFin;

        // Validación personalizada
        public bool FechasValidas()
        {
            return FechaFin > FechaInicio;
        }
    }
}