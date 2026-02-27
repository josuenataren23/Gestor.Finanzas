using Gestor.Finanzas.Models;
using Gestor.Finanzas.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Gestor.Finanzas.Controllers
{
    public class MetasController : BaseController
    {
        private GestorFinanzasModel db = new GestorFinanzasModel();

        // GET: Metas
        public ActionResult Index()
        {
            var userId = UsuarioActualId;

            var metas = db.Metas
                .Where(m => m.usuario_id == userId)
                .OrderByDescending(m => m.id)
                .ToList();

            if (!metas.Any())
            {
                ViewBag.EmptyState = new EmptyStateViewModel
                {
                    Title = "Aún no tienes metas de ahorro",
                    Description = "Define tus objetivos financieros y empieza a ahorrar hacia ellos.",
                    ActionText = "Crear mi primera meta",
                    ActionUrl = Url.Action("Create", "Metas"),
                    IconClass = "fa-solid fa-piggy-bank"
                };
                return View(metas);
            }

            // Stats para las tarjetas superiores
            var totalAcumulado = metas.Sum(m => m.monto_actual ?? 0);
            var metasActivas = metas.Count(m => (m.monto_actual ?? 0) < m.monto_objetivo);
            var ahorroPromedio = metas.Any() ? Math.Round(totalAcumulado / metas.Count, 2) : 0;
            var proximaMeta = metas
                .Where(m => m.monto_objetivo > 0 && (m.monto_actual ?? 0) < m.monto_objetivo)
                .OrderByDescending(m => (m.monto_actual ?? 0) / m.monto_objetivo)
                .FirstOrDefault();

            ViewBag.TotalAcumulado = totalAcumulado;
            ViewBag.MetasActivas = metasActivas;
            ViewBag.AhorroPromedio = ahorroPromedio;
            ViewBag.ProximaMeta = proximaMeta;

            return View(metas);
        }

        // GET: Metas/Create
        public ActionResult Create()
        {
            return View(new MetaViewModel
            {
                icono = "fa-solid fa-piggy-bank"
            });
        }

        // POST: Metas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MetaViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // El monto actual no puede superar al objetivo
            if ((vm.monto_actual ?? 0) > vm.monto_objetivo)
                ModelState.AddModelError("monto_actual", "El monto guardado no puede superar el monto objetivo.");

            if (!ModelState.IsValid)
                return View(vm);

            var montoActual = vm.monto_actual ?? 0;

            db.Metas.Add(new Meta
            {
                usuario_id     = UsuarioActualId,
                nombre         = vm.nombre,
                descripcion    = vm.descripcion,
                monto_objetivo = vm.monto_objetivo,
                monto_actual   = montoActual,
                fecha_estimada = vm.fecha_estimada,
                icono          = string.IsNullOrWhiteSpace(vm.icono) ? "fa-solid fa-piggy-bank" : vm.icono,
                fecha_creacion     = DateTime.Now,
                // Si al crear ya se cumple el objetivo, se registra la fecha de cumplimiento
                fecha_cumplimiento = montoActual >= vm.monto_objetivo ? (DateTime?)DateTime.Now : null
            });

            db.SaveChanges();
            TempData["Success"] = "Meta creada correctamente.";
            return RedirectToAction("Index");
        }

        // GET: Metas/Edit/5
        public ActionResult Edit(int id)
        {
            var meta = db.Metas
                .FirstOrDefault(m => m.id == id && m.usuario_id == UsuarioActualId);

            if (meta == null) return HttpNotFound();

            return View(new MetaViewModel
            {
                id             = meta.id,
                nombre         = meta.nombre,
                descripcion    = meta.descripcion,
                monto_objetivo = meta.monto_objetivo,
                monto_actual   = meta.monto_actual,
                fecha_estimada = meta.fecha_estimada,
                icono          = meta.icono
            });
        }

        // POST: Metas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MetaViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if ((vm.monto_actual ?? 0) > vm.monto_objetivo)
                ModelState.AddModelError("monto_actual", "El monto guardado no puede superar el monto objetivo.");

            if (!ModelState.IsValid)
                return View(vm);

            var meta = db.Metas
                .FirstOrDefault(m => m.id == vm.id && m.usuario_id == UsuarioActualId);

            if (meta == null) return HttpNotFound();

            var montoActual = vm.monto_actual ?? 0;
            var yaEstabaCompletada = meta.monto_actual >= meta.monto_objetivo;
            var ahoraEstaCompleta  = montoActual >= vm.monto_objetivo;

            meta.nombre         = vm.nombre;
            meta.descripcion    = vm.descripcion;
            meta.monto_objetivo = vm.monto_objetivo;
            meta.monto_actual   = montoActual;
            meta.fecha_estimada = vm.fecha_estimada;
            meta.icono          = string.IsNullOrWhiteSpace(vm.icono) ? "fa-solid fa-piggy-bank" : vm.icono;

            // Registra la fecha de cumplimiento la primera vez que se alcanza el objetivo.
            // Si el monto baja de nuevo (corrección), limpia la fecha.
            if (ahoraEstaCompleta && !yaEstabaCompletada)
                meta.fecha_cumplimiento = DateTime.Now;
            else if (!ahoraEstaCompleta)
                meta.fecha_cumplimiento = null;

            db.SaveChanges();
            TempData["Success"] = "Meta actualizada correctamente.";
            return RedirectToAction("Index");
        }

        // GET: Metas/Delete/5
        public ActionResult Delete(int id)
        {
            var meta = db.Metas
                .FirstOrDefault(m => m.id == id && m.usuario_id == UsuarioActualId);

            if (meta == null) return HttpNotFound();
            return View(meta);
        }

        // POST: Metas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var meta = db.Metas
                .FirstOrDefault(m => m.id == id && m.usuario_id == UsuarioActualId);

            if (meta == null) return HttpNotFound();

            db.Metas.Remove(meta);
            db.SaveChanges();
            TempData["Success"] = "Meta eliminada correctamente.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
