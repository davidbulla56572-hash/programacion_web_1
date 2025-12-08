using final_project.Models;

namespace final_project.ViewModels
{
    public class PresupuestoIndexViewModel
    {
        // Presupuesto activo
        public Presupuesto? PresupuestoActivo { get; set; }
        
        // Estadísticas del periodo actual
        public decimal TotalGastado { get; set; }
        public decimal Restante { get; set; }
        public decimal PorcentajeUsado { get; set; }
        public int CantidadGastos { get; set; }
        public int DiasRestantes { get; set; }
        public int DiasTranscurridos { get; set; }
        public int DuracionTotal { get; set; }
        
        // Historial
        public List<Presupuesto> HistorialPresupuestos { get; set; } = new List<Presupuesto>();
        
        // Propiedades calculadas
        public bool TienePresupuestoActivo => PresupuestoActivo != null;
        public bool ExcedioPresupuesto => Restante < 0;
        public bool CercaDelLimite => PorcentajeUsado > 90;
        public bool AlertaMedia => PorcentajeUsado > 70 && PorcentajeUsado <= 90;
        public bool PocoDiasRestantes => DiasRestantes < 7;
        
        // Promedio de gasto diario
        public decimal PromedioGastoDiario => DiasTranscurridos > 0 
            ? TotalGastado / DiasTranscurridos 
            : 0;
            
        // Proyección de gasto al final del periodo
        public decimal GastoProyectado => DiasTranscurridos > 0 && DuracionTotal > 0
            ? (TotalGastado / DiasTranscurridos) * DuracionTotal
            : 0;
    }
}