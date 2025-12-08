using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using final_project.Data;
using final_project.Models;
using final_project.ViewModels;

namespace final_project.Controllers
{
    public class PresupuestoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PresupuestoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Presupuesto
        public async Task<IActionResult> Index()
        {
            var viewModel = new PresupuestoIndexViewModel();

            // Obtener presupuesto activo usando LINQ
            viewModel.PresupuestoActivo = await _context.Presupuestos
                .Where(p => p.Activo && p.FechaInicio <= DateTime.Now && p.FechaFin >= DateTime.Now)
                .OrderByDescending(p => p.FechaInicio)
                .FirstOrDefaultAsync();

            // Calcular gastos del periodo actual usando LINQ
            if (viewModel.PresupuestoActivo != null)
            {
                var gastosDelPeriodo = await _context.Gastos
                    .Where(g => g.Fecha >= viewModel.PresupuestoActivo.FechaInicio && 
                                g.Fecha <= viewModel.PresupuestoActivo.FechaFin)
                    .ToListAsync();

                viewModel.TotalGastado = gastosDelPeriodo.Sum(g => g.Monto);
                viewModel.Restante = viewModel.PresupuestoActivo.MontoTotal - viewModel.TotalGastado;
                viewModel.PorcentajeUsado = viewModel.PresupuestoActivo.MontoTotal > 0 
                    ? (viewModel.TotalGastado / viewModel.PresupuestoActivo.MontoTotal) * 100 
                    : 0;
                viewModel.CantidadGastos = gastosDelPeriodo.Count;

                // Calcular días
                viewModel.DiasRestantes = (viewModel.PresupuestoActivo.FechaFin - DateTime.Now).Days;
                viewModel.DiasTranscurridos = (DateTime.Now - viewModel.PresupuestoActivo.FechaInicio).Days;
                viewModel.DuracionTotal = (viewModel.PresupuestoActivo.FechaFin - viewModel.PresupuestoActivo.FechaInicio).Days;
            }

            // Obtener historial de presupuestos usando LINQ
            viewModel.HistorialPresupuestos = await _context.Presupuestos
                .OrderByDescending(p => p.FechaInicio)
                .Take(5)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: Presupuesto/Create
        public IActionResult Create()
        {
            var nuevoPresupuesto = new Presupuesto
            {
                FechaInicio = DateTime.Now.Date,
                FechaFin = DateTime.Now.Date.AddMonths(1)
            };
            return View(nuevoPresupuesto);
        }

        // POST: Presupuesto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MontoTotal,FechaInicio,FechaFin,Descripcion,Activo")] Presupuesto presupuesto)
        {
            if (ModelState.IsValid)
            {
                // Validar que la fecha de fin sea posterior a la de inicio
                if (presupuesto.FechaFin <= presupuesto.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de fin debe ser posterior a la fecha de inicio");
                    return View(presupuesto);
                }

                // Si el nuevo presupuesto es activo, desactivar otros presupuestos activos usando LINQ
                if (presupuesto.Activo)
                {
                    var presupuestosActivos = await _context.Presupuestos
                        .Where(p => p.Activo)
                        .ToListAsync();

                    foreach (var p in presupuestosActivos)
                    {
                        p.Activo = false;
                    }
                }

                _context.Add(presupuesto);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Presupuesto creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(presupuesto);
        }

        // GET: Presupuesto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presupuesto = await _context.Presupuestos.FindAsync(id);
            if (presupuesto == null)
            {
                return NotFound();
            }
            return View(presupuesto);
        }

        // POST: Presupuesto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MontoTotal,FechaInicio,FechaFin,Descripcion,Activo")] Presupuesto presupuesto)
        {
            if (id != presupuesto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Validar que la fecha de fin sea posterior a la de inicio
                if (presupuesto.FechaFin <= presupuesto.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de fin debe ser posterior a la fecha de inicio");
                    return View(presupuesto);
                }

                try
                {
                    // Si se activa este presupuesto, desactivar otros usando LINQ
                    if (presupuesto.Activo)
                    {
                        var otrosPresupuestosActivos = await _context.Presupuestos
                            .Where(p => p.Activo && p.Id != id)
                            .ToListAsync();

                        foreach (var p in otrosPresupuestosActivos)
                        {
                            p.Activo = false;
                        }
                    }

                    _context.Update(presupuesto);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Presupuesto actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PresupuestoExists(presupuesto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(presupuesto);
        }

        // GET: Presupuesto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var presupuesto = await _context.Presupuestos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (presupuesto == null)
            {
                return NotFound();
            }

            return View(presupuesto);
        }

        // POST: Presupuesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var presupuesto = await _context.Presupuestos.FindAsync(id);
            if (presupuesto != null)
            {
                _context.Presupuestos.Remove(presupuesto);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Presupuesto eliminado exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PresupuestoExists(int id)
        {
            return _context.Presupuestos.Any(e => e.Id == id);
        }
    }
}