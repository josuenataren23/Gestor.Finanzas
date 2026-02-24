using Gestor.Finanzas.Models;
using Gestor.Finanzas.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gestor.Finanzas.Controllers
{
    public class ReportesController : BaseController
    {
        private GestorFinanzasModel db = new GestorFinanzasModel();
        // GET: Reportes
        public ActionResult Index()
        {
            var totalTransacciones = db.Transacciones.Count();

            if (totalTransacciones < 5)
            {
                ViewBag.EmptyState = new EmptyStateViewModel
                {
                    Title = "Necesitamos más datos",
                    Description = "Registra al menos 5 transacciones para que podamos generar tus primeros reportes visuales y análisis detallados.",
                    ActionText = "Ir a transacciones",
                    ActionUrl = Url.Action("Index", "Transacciones"),
                    IconClass = "fa-solid fa-chart-simple"
                };
            }

            return View();
        }
    }
}