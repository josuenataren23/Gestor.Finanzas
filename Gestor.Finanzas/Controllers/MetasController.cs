using Gestor.Finanzas.Models;
using Gestor.Finanzas.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gestor.Finanzas.Controllers
{
    public class MetasController : BaseController
    {
        // GET: Metas
        private GestorFinanzasModel db = new GestorFinanzasModel();
        public ActionResult Index()
        {
            var totalMetas = db.Metas.Count();

            if (totalMetas == 0)
            {
                ViewBag.EmptyState = new EmptyStateViewModel
                {
                    Title = "Define tus sueños financieros",
                    Description = "Aún no has creado metas de ahorro. Comienza a planificar tus próximos hitos, desde viajes hasta fondos de emergencia.",
                    ActionText = "Crear mi primer meta",
                    ActionUrl = Url.Action("Create", "Metas"),
                    IconClass = "fa-solid fa-bullseye"
                };
            }

            return View();
        }
    }
}