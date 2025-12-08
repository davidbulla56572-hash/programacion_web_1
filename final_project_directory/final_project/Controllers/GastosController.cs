using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using final_project.Data;
using final_project.Models;
using final_project.ViewModels;

namespace final_project.Controllers
{
    public class GastosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GastosController> _logger;

        public GastosController(ApplicationDbContext context, ILogger<GastosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Gastos
        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new GastosIndexViewModel();

                // Obtener todos los gastos ordenados por fecha descendente usando LINQ
                viewModel.Gastos = await _context.Gastos
                    .OrderByDescending(g => g.Fecha)
                    .ToListAsync();

                // Calcular estadísticas usando LINQ
                viewModel.TotalGastado = viewModel.Gastos.Sum(g => g.Monto);
                viewModel.CantidadGastos = viewModel.Gastos.Count;

                // Obtener presupuesto activo usando LINQ
                var presupuestoActivo = await _context.Presupuestos
                    .Where(p => p.Activo && p.FechaInicio <= DateTime.Now && p.FechaFin >= DateTime.Now)
                    .OrderByDescending(p => p.FechaInicio)
                    .FirstOrDefaultAsync();

                viewModel.PresupuestoTotal = presupuestoActivo?.MontoTotal ?? 0;
                viewModel.PresupuestoRestante = viewModel.PresupuestoTotal - viewModel.TotalGastado;

                // Gastos por categoría usando LINQ con porcentajes
                viewModel.GastosPorCategoria = viewModel.Gastos
                    .GroupBy(g => g.Categoria)
                    .Select(group => new GastoPorCategoria
                    {
                        Categoria = group.Key,
                        Total = group.Sum(g => g.Monto),
                        Cantidad = group.Count(),
                        Porcentaje = viewModel.TotalGastado > 0
                            ? (group.Sum(g => g.Monto) / viewModel.TotalGastado) * 100
                            : 0
                    })
                    .OrderByDescending(x => x.Total)
                    .ToList();

                // Pasar datos al ViewBag para compatibilidad con la vista actual
                ViewBag.TotalGastado = viewModel.TotalGastado;
                ViewBag.PresupuestoTotal = viewModel.PresupuestoTotal;
                ViewBag.PresupuestoRestante = viewModel.PresupuestoRestante;
                ViewBag.GastosPorCategoria = viewModel.GastosPorCategoria;

                // Retornar la lista de gastos directamente al modelo
                return View(viewModel.Gastos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de gastos");
                TempData["Error"] = "Ocurrió un error al cargar los gastos. Por favor, intenta de nuevo.";

                // Inicializar ViewBag con valores por defecto en caso de error
                ViewBag.TotalGastado = 0m;
                ViewBag.PresupuestoTotal = 0m;
                ViewBag.PresupuestoRestante = 0m;
                ViewBag.GastosPorCategoria = new List<GastoPorCategoria>();

                return View(new List<Gasto>());
            }
        }

        // GET: Gastos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID de gasto no válido";
                return RedirectToAction(nameof(Index));
            }

            var gasto = await _context.Gastos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gasto == null)
            {
                TempData["Error"] = "Gasto no encontrado";
                return RedirectToAction(nameof(Index));
            }

            return View(gasto);
        }

        // GET: Gastos/Create
        public IActionResult Create()
        {
            var nuevoGasto = new Gasto
            {
                Fecha = DateTime.Now
            };
            return View(nuevoGasto);
        }

        // POST: Gastos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,Monto,Categoria,Fecha,Notas")] Gasto gasto)
        {
            // Validaciones adicionales del lado del servidor
            if (gasto.Monto <= 0)
            {
                ModelState.AddModelError("Monto", "El monto debe ser mayor a 0");
            }

            if (gasto.Fecha > DateTime.Now)
            {
                ModelState.AddModelError("Fecha", "La fecha no puede ser futura");
            }

            if (string.IsNullOrWhiteSpace(gasto.Descripcion))
            {
                ModelState.AddModelError("Descripcion", "La descripción no puede estar vacía");
            }

            if (string.IsNullOrWhiteSpace(gasto.Categoria))
            {
                ModelState.AddModelError("Categoria", "Debes seleccionar una categoría");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(gasto);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Gasto creado: {gasto.Descripcion} - ${gasto.Monto}");
                    TempData["Mensaje"] = $"✅ Gasto de ${gasto.Monto:N2} creado exitosamente";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear el gasto");
                    ModelState.AddModelError("", "Ocurrió un error al guardar el gasto. Por favor, intenta de nuevo.");
                }
            }

            return View(gasto);
        }

        // GET: Gastos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID de gasto no válido";
                return RedirectToAction(nameof(Index));
            }

            var gasto = await _context.Gastos.FindAsync(id);

            if (gasto == null)
            {
                TempData["Error"] = "Gasto no encontrado";
                return RedirectToAction(nameof(Index));
            }

            return View(gasto);
        }

        // POST: Gastos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Monto,Categoria,Fecha,Notas")] Gasto gasto)
        {
            if (id != gasto.Id)
            {
                TempData["Error"] = "ID de gasto no coincide";
                return RedirectToAction(nameof(Index));
            }

            // Validaciones adicionales
            if (gasto.Monto <= 0)
            {
                ModelState.AddModelError("Monto", "El monto debe ser mayor a 0");
            }

            if (string.IsNullOrWhiteSpace(gasto.Descripcion))
            {
                ModelState.AddModelError("Descripcion", "La descripción no puede estar vacía");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gasto);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Gasto actualizado: ID {gasto.Id}");
                    TempData["Mensaje"] = "✅ Gasto actualizado exitosamente";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GastoExists(gasto.Id))
                    {
                        TempData["Error"] = "El gasto ya no existe";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogError("Error de concurrencia al actualizar gasto");
                        ModelState.AddModelError("", "Error al actualizar. Otro usuario pudo haber modificado este registro.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar el gasto");
                    ModelState.AddModelError("", "Ocurrió un error al actualizar el gasto.");
                }
            }

            return View(gasto);
        }

        // GET: Gastos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID de gasto no válido";
                return RedirectToAction(nameof(Index));
            }

            var gasto = await _context.Gastos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gasto == null)
            {
                TempData["Error"] = "Gasto no encontrado";
                return RedirectToAction(nameof(Index));
            }

            return View(gasto);
        }

        // POST: Gastos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var gasto = await _context.Gastos.FindAsync(id);

                if (gasto == null)
                {
                    TempData["Error"] = "Gasto no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                _context.Gastos.Remove(gasto);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Gasto eliminado: ID {id}");
                TempData["Mensaje"] = "🗑️ Gasto eliminado exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar gasto ID {id}");
                TempData["Error"] = "Ocurrió un error al eliminar el gasto";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GastoExists(int id)
        {
            return _context.Gastos.Any(e => e.Id == id);
        }
    }
}