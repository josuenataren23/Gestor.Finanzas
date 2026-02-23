using System.Web.Mvc;

public class BaseController : Controller
{
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (Session["UsuarioId"] == null)
        {
            filterContext.Result =
                new RedirectResult("~/Account/Login");
        }

        base.OnActionExecuting(filterContext);
    }
}