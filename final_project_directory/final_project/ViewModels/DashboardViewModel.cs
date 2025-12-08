using final_project.Models;

namespace final_project.ViewModels
{
    public class DashboardViewModel
    {
        // Presupuesto
        public Presupuesto? PresupuestoActivo { get; set; }
        public decimal PresupuestoTotal { get; set; }
        public decimal PresupuestoRestante { get; set; }
        public decimal PorcentajeUsado { get; set; }
        
        // Estadísticas de Gastos
        public decimal TotalGastado { get; set; }
        public decimal TotalGastadoMes { get; set; }
        public int CantidadGastos { get; set; }
        
        // Listas
        public List<Gasto> UltimosGastos { get; set; } = new List<Gasto>();
        public List<CategoriaResumen> TopCategorias { get; set; } = new List<CategoriaResumen>();
        
        // Propiedades calculadas
        public bool TienePresupuestoActivo => PresupuestoActivo != null;
        public bool ExcedioPresupuesto => PresupuestoRestante < 0;
        public bool CercaDelLimite => PorcentajeUsado > 90;
    }

    public class CategoriaResumen
    {
        public string Categoria { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Cantidad { get; set; }
    }
}