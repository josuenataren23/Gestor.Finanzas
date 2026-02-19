using System.Web.Mvc;

public static class HtmlHelpers
{
    public static string IsCurrent(this HtmlHelper html,
        string controller, string action)
    {
        var routeData = html.ViewContext.RouteData;

        var currentController = routeData.Values["controller"]?.ToString();
        var currentAction = routeData.Values["action"]?.ToString();

        return (controller == currentController && action == currentAction)
            ? "bg-purple-600 text-white"
            : "";
    }
}
