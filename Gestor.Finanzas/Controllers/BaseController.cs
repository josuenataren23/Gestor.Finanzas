using System.Web.Mvc;

public class BaseController : Controller
{
    protected int UsuarioActualId
    {
        get
        {
            if (Session["UsuarioId"] != null)
                return (int)Session["UsuarioId"];
            return 0;
        }
    }

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (Session["UsuarioId"] == null)
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action", "Login" }
                }
            );
        }
        base.OnActionExecuting(filterContext);
    }
}