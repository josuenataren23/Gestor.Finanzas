using Gestor.Finanzas.Models;
using Gestor.Finanzas.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gestor.Finanzas.Controllers
{
    public class PresupuestosController : BaseController
    {
        // GET: Presupuestos
        private GestorFinanzasModel db = new GestorFinanzasModel();
        public ActionResult Index()
        {
            var totalPresupuestos = db.Presupuestos.Count();

            if (totalPresupuestos == 0)
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

            return View();
        }
    }
}