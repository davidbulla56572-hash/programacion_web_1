using final_project.Models;

namespace final_project.ViewModels
{
    public class GastosIndexViewModel
    {
        // Lista de gastos
        public List<Gasto> Gastos { get; set; } = new List<Gasto>();
        
        // Estadísticas generales
        public decimal TotalGastado { get; set; }
        public decimal PresupuestoTotal { get; set; }
        public decimal PresupuestoRestante { get; set; }
        public int CantidadGastos { get; set; }
        
        // Gastos por categoría
        public List<GastoPorCategoria> GastosPorCategoria { get; set; } = new List<GastoPorCategoria>();
        
        // Filtros aplicados (para futuras mejoras)
        public string? CategoriaFiltro { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        
        // Propiedades calculadas
        public bool TienePresupuesto => PresupuestoTotal > 0;
        public decimal PorcentajeUsado => PresupuestoTotal > 0 
            ? (TotalGastado / PresupuestoTotal) * 100 
            : 0;
    }

    public class GastoPorCategoria
    {
        public string Categoria { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Cantidad { get; set; }
        public decimal Porcentaje { get; set; }
    }
}