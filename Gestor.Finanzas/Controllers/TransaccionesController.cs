using Gestor.Finanzas.Models;
using Gestor.Finanzas.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gestor.Finanzas.Controllers
{
    public class TransaccionesController : BaseController
    {
        private GestorFinanzasModel db = new GestorFinanzasModel();
        // GET: Transacciones
        public ActionResult Index()
        {
            var totalTransacciones = db.Transacciones.Count();

            if (totalTransacciones == 0)
            {
                ViewBag.EmptyState = new EmptyStateViewModel
                {
                    Title = "Sin movimientos aún",
                    Description = "Tus transacciones aparecerán aquí una vez que las registres. Comienza a rastrear tus gastos diarios ahora mismo.",
                    ActionText = "Registrar mi primera transacción",
                    ActionUrl = Url.Action("Create", "Transacciones"),
                    IconClass = "fa-solid fa-magnifying-glass-dollar"
                };
            }

            return View();
        }
    }
}