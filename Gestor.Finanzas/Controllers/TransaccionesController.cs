using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gestor.Finanzas.Controllers
{
    public class TransaccionesController : Controller
    {
        // GET: Transacciones
        public ActionResult Index()
        {
            return View();
        }
    }
}