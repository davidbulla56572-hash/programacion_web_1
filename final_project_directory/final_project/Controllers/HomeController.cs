using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using final_project.Models;
using final_project.Data;
using final_project.ViewModels;

namespace final_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Home/Index - Dashboard Principal
        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();

            // Obtener presupuesto activo usando LINQ
            viewModel.PresupuestoActivo = await _context.Presupuestos
                .Where(p => p.Activo && p.FechaInicio <= DateTime.Now && p.FechaFin >= DateTime.Now)
                .OrderByDescending(p => p.FechaInicio)
                .FirstOrDefaultAsync();

            // Calcular gastos totales usando LINQ
            var todosLosGastos = await _context.Gastos.ToListAsync();
            viewModel.TotalGastado = todosLosGastos.Sum(g => g.Monto);
            viewModel.CantidadGastos = todosLosGastos.Count;

            // Obtener últimos 5 gastos usando LINQ
            viewModel.UltimosGastos = await _context.Gastos
                .OrderByDescending(g => g.Fecha)
                .Take(5)
                .ToListAsync();

            // Gastos del mes actual usando LINQ
            var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var finMes = inicioMes.AddMonths(1).AddDays(-1);
            
            viewModel.TotalGastadoMes = await _context.Gastos
                .Where(g => g.Fecha >= inicioMes && g.Fecha <= finMes)
                .SumAsync(g => g.Monto);

            // Top 3 categorías con más gastos usando LINQ
            viewModel.TopCategorias = todosLosGastos
                .GroupBy(g => g.Categoria)
                .Select(group => new CategoriaResumen
                {
                    Categoria = group.Key,
                    Total = group.Sum(g => g.Monto),
                    Cantidad = group.Count()
                })
                .OrderByDescending(x => x.Total)
                .Take(3)
                .ToList();

            // Calcular presupuesto restante
            viewModel.PresupuestoTotal = viewModel.PresupuestoActivo?.MontoTotal ?? 0;
            viewModel.PresupuestoRestante = viewModel.PresupuestoTotal - viewModel.TotalGastado;
            viewModel.PorcentajeUsado = viewModel.PresupuestoTotal > 0 
                ? (viewModel.TotalGastado / viewModel.PresupuestoTotal) * 100 
                : 0;

            return View(viewModel);
        }

        // POST: Home/AgregarGastoRapido - Para el formulario del dashboard
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarGastoRapido([Bind("Descripcion,Monto,Categoria,Fecha")] Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gasto);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Gasto agregado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, volver al dashboard con los datos
            TempData["Error"] = "Error al agregar el gasto. Verifica los datos.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}