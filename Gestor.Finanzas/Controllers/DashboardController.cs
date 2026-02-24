using Gestor.Finanzas.Models;
using Gestor.Finanzas.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;

public class DashboardController : BaseController
{
    private GestorFinanzasModel db = new GestorFinanzasModel();
    public ActionResult Index()
    {
        ViewBag.Nombre = Session["UsuarioNombre"];
        ViewBag.Foto = Session["UsuarioFoto"];
        var totalTransacciones = db.Transacciones.Count();

        if (totalTransacciones == 0)
        {
            ViewBag.EmptyState = new EmptyStateViewModel
            {
                Title = "Tu viaje financiero comienza aquí",
                Description = "Aún no tienes movimientos registrados. Comienza agregando tu primera transaccion para ver la magia de FinTrack.",
                ActionText = "Registrar mi primera transacción",
                ActionUrl = Url.Action("Create", "Transacciones"),
                IconClass = "fa-regular fa-credit-card"
            };
        }

        return View();
    }
}