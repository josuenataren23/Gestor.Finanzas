using Gestor.Finanzas.Models;
using Gestor.Finanzas.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Gestor.Finanzas.Controllers
{
    public class PresupuestosController : BaseController
    {
        private GestorFinanzasModel db = new GestorFinanzasModel();

        // =============================================
        // GET: /Presupuestos
        // =============================================
        public ActionResult Index()
        {
            var presupuestos = db.Presupuestos
                .Include("Categoria")
                .Where(p => p.usuario_id == UsuarioActualId)
                .OrderBy(p => p.fecha_creacion)
                .ToList();

            if (!presupuestos.Any())
            {
                ViewBag.EmptyState = new EmptyStateViewModel
                {
                    Title = "Define tus límites financieros",
                    Description = "Aún no has creado ningún presupuesto. Empieza a organizar tus gastos por categorías para tener un control total.",
                    ActionText = "Crear mi primer presupuesto",
                    ActionUrl = Url.Action("Create", "Presupuestos"),
                    IconClass = "fa-solid fa-wallet"
                };
            }

            // Resumen para las tarjetas superiores
            ViewBag.TotalObjetivo = presupuestos.Sum(p => p.monto_objetivo);
            ViewBag.TotalGastado = presupuestos.Sum(p => p.monto_gastado ?? 0);
            ViewBag.ProgresoGeneral = ViewBag.TotalObjetivo > 0
                ? Math.Min(100, (double)(ViewBag.TotalGastado / ViewBag.TotalObjetivo) * 100)
                : 0;

            return View(presupuestos);
        }

        // =============================================
        // GET: /Presupuestos/Details/5
        // =============================================
        public ActionResult Details(int id)
        {
            var p = db.Presupuestos
                .Include("Categoria")
                .FirstOrDefault(x => x.id == id && x.usuario_id == UsuarioActualId);

            if (p == null) return HttpNotFound();
            return View(p);
        }

        // =============================================
        // GET: /Presupuestos/Create
        // =============================================
        public ActionResult Create()
        {
            CargarCategorias();
            return View(new PresupuestoViewModel { fecha_creacion = DateTime.Today });
        }

        // =============================================
        // POST: /Presupuestos/Create
        // =============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PresupuestoViewModel vm)
        {
            // Verificar que no exista ya un presupuesto para esa categoría del usuario
            bool duplicado = db.Presupuestos.Any(p =>
                p.usuario_id == UsuarioActualId && p.categoria_id == vm.categoria_id);

            if (duplicado)
                ModelState.AddModelError("categoria_id",
                    "Ya tienes un presupuesto activo para esta categoría.");

            if (!ModelState.IsValid)
            {
                CargarCategorias(vm.categoria_id);
                return View(vm);
            }

            db.Presupuestos.Add(new Presupuesto
            {
                usuario_id = UsuarioActualId,
                categoria_id = vm.categoria_id,
                monto_objetivo = vm.monto_objetivo,
                monto_gastado = 0,
                fecha_creacion = DateTime.Now
            });

            db.SaveChanges();
            TempData["Success"] = "Presupuesto creado correctamente.";
            return RedirectToAction("Index");
        }

        // =============================================
        // GET: /Presupuestos/Edit/5
        // =============================================
        public ActionResult Edit(int id)
        {
            var p = db.Presupuestos
                .FirstOrDefault(x => x.id == id && x.usuario_id == UsuarioActualId);

            if (p == null) return HttpNotFound();

            ModelState.Clear();
            CargarCategorias(p.categoria_id);

            return View(new PresupuestoViewModel
            {
                id = p.id,
                categoria_id = p.categoria_id ?? 0,
                monto_objetivo = p.monto_objetivo,
                monto_gastado = p.monto_gastado ?? 0,
                fecha_creacion = p.fecha_creacion
            });
        }

        // =============================================
        // POST: /Presupuestos/Edit/5
        // =============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PresupuestoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                CargarCategorias(vm.categoria_id);
                return View(vm);
            }

            var p = db.Presupuestos
                .FirstOrDefault(x => x.id == vm.id && x.usuario_id == UsuarioActualId);

            if (p == null) return HttpNotFound();

            p.categoria_id = vm.categoria_id;
            p.monto_objetivo = vm.monto_objetivo;

            db.SaveChanges();
            TempData["Success"] = "Presupuesto actualizado correctamente.";
            return RedirectToAction("Index");
        }

        // =============================================
        // GET: /Presupuestos/Delete/5
        // =============================================
        public ActionResult Delete(int id)
        {
            var p = db.Presupuestos
                .Include("Categoria")
                .FirstOrDefault(x => x.id == id && x.usuario_id == UsuarioActualId);

            if (p == null) return HttpNotFound();
            return View(p);
        }

        // =============================================
        // POST: /Presupuestos/Delete/5
        // =============================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var p = db.Presupuestos
                .FirstOrDefault(x => x.id == id && x.usuario_id == UsuarioActualId);

            if (p == null) return HttpNotFound();

            db.Presupuestos.Remove(p);
            db.SaveChanges();
            TempData["Success"] = "Presupuesto eliminado correctamente.";
            return RedirectToAction("Index");
        }

        // ── helpers ─────────────────────────────────
        private void CargarCategorias(int? selected = null)
        {
            ViewBag.categoria_id = new SelectList(
                db.Categorias.OrderBy(c => c.nombre),
                "id", "nombre", selected);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}