using System.Web.Mvc;

public class DashboardController : BaseController
{
    public ActionResult Index()
    {
        ViewBag.Nombre = Session["UsuarioNombre"];
        ViewBag.Foto = Session["UsuarioFoto"];
        return View();
    }
}