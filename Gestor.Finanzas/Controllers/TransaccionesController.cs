using Gestor.Finanzas.Models;
using Gestor.Finanzas.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Gestor.Finanzas.Controllers
{
    public class TransaccionesController : BaseController
    {
        private GestorFinanzasModel db = new GestorFinanzasModel();

        // 1 = GASTO | 2 = INGRESO
        private const int TIPO_GASTO = 1;
        private const int TIPO_INGRESO = 2;

        public ActionResult Index()
        {
            var transacciones = db.Transacciones
                .Include("Categoria")
                .Include("TipoTransaccion")
                .Where(t => t.usuario_id == UsuarioActualId)
                .OrderByDescending(t => t.fecha_transaccion)
                .ToList();

            if (!transacciones.Any())
            {
                ViewBag.EmptyState = new EmptyStateViewModel
                {
                    Title = "Sin movimientos aún",
                    Description = "Tus transacciones aparecerán aquí una vez que las registres.",
                    ActionText = "Registrar mi primera transacción",
                    ActionUrl = Url.Action("Create", "Transacciones"),
                    IconClass = "fa-solid fa-magnifying-glass-dollar"
                };
            }

            return View(transacciones);
        }

        public ActionResult Details(int id)
        {
            var tx = db.Transacciones
                .Include("Categoria")
                .Include("TipoTransaccion")
                .FirstOrDefault(t => t.id == id && t.usuario_id == UsuarioActualId);

            if (tx == null) return HttpNotFound();
            return View(tx);
        }

        public ActionResult Create()
        {
            CargarSelectLists();
            return View(new TransaccionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransaccionViewModel vm)
        {
            // Categoría obligatoria solo para gastos
            if (vm.tipo_id == TIPO_GASTO && vm.categoria_id == null)
                ModelState.AddModelError("categoria_id", "La categoría es obligatoria para gastos.");

            if (!ModelState.IsValid)
            {
                CargarSelectLists(vm.tipo_id, vm.categoria_id);
                return View(vm);
            }

            db.Transacciones.Add(new Transaccione
            {
                usuario_id = UsuarioActualId,
                tipo_id = vm.tipo_id,
                categoria_id = vm.tipo_id == TIPO_INGRESO ? (int?)null : vm.categoria_id,
                monto = Math.Abs(vm.monto),
                descripcion = vm.descripcion,
                fecha_transaccion = DateTime.Now
            });

            db.SaveChanges();
            TempData["Success"] = "Transacción registrada correctamente.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var tx = db.Transacciones
                .FirstOrDefault(t => t.id == id && t.usuario_id == UsuarioActualId);

            if (tx == null) return HttpNotFound();

            CargarSelectLists(tx.tipo_id, tx.categoria_id);
            return View(new TransaccionViewModel
            {
                id = tx.id,
                tipo_id = tx.tipo_id,
                categoria_id = tx.categoria_id,
                monto = Math.Abs(tx.monto),
                fecha_transaccion = tx.fecha_transaccion,
                descripcion = tx.descripcion
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TransaccionViewModel vm)
        {
            if (vm.tipo_id == TIPO_GASTO && vm.categoria_id == null)
                ModelState.AddModelError("categoria_id", "La categoría es obligatoria para gastos.");

            if (!ModelState.IsValid)
            {
                CargarSelectLists(vm.tipo_id, vm.categoria_id);
                return View(vm);
            }

            var tx = db.Transacciones
                .FirstOrDefault(t => t.id == vm.id && t.usuario_id == UsuarioActualId);

            if (tx == null) return HttpNotFound();

            tx.tipo_id = vm.tipo_id;
            tx.categoria_id = vm.tipo_id == TIPO_INGRESO ? (int?)null : vm.categoria_id;
            tx.monto = Math.Abs(vm.monto);
            tx.descripcion = vm.descripcion;

            db.SaveChanges();
            TempData["Success"] = "Transacción actualizada correctamente.";
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var tx = db.Transacciones
                .Include("Categoria")
                .Include("TipoTransaccion")
                .FirstOrDefault(t => t.id == id && t.usuario_id == UsuarioActualId);

            if (tx == null) return HttpNotFound();
            return View(tx);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tx = db.Transacciones
                .FirstOrDefault(t => t.id == id && t.usuario_id == UsuarioActualId);

            if (tx == null) return HttpNotFound();
            db.Transacciones.Remove(tx);
            db.SaveChanges();
            TempData["Success"] = "Transacción eliminada correctamente.";
            return RedirectToAction("Index");
        }

        private void CargarSelectLists(int? tipoId = null, int? categoriaId = null)
        {
            ViewBag.tipo_id = new SelectList(db.TipoTransacciones, "id", "nombre", tipoId);
            ViewBag.categoria_id = new SelectList(db.Categorias, "id", "nombre", categoriaId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}