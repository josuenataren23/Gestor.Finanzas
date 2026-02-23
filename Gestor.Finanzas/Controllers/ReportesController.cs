using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gestor.Finanzas.Controllers
{
    public class ReportesController : BaseController
    {
        // GET: Reportes
        public ActionResult Index()
        {
            return View();
        }
    }
}