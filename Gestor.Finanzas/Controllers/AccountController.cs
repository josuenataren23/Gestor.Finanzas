using Gestor.Finanzas.Models;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

public class AccountController : Controller
{
    private GestorFinanzasModel db = new GestorFinanzasModel();
    // Vista Login
    public ActionResult Login()
    {
        if (Session["UsuarioId"] != null)
            return RedirectToAction("Index", "Dashboard");

        return View();
    }

    // Inicia login con Google
    public ActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleResponse", "Account")
        };

        HttpContext.GetOwinContext()
            .Authentication.Challenge(properties, "Google");

        return new HttpUnauthorizedResult();
    }

    // Respuesta de Google
    public ActionResult GoogleResponse()
    {
        var auth = HttpContext.GetOwinContext().Authentication;
        var identity = auth.AuthenticateAsync("ExternalCookie").Result?.Identity;

        if (identity == null)
            return RedirectToAction("Login");

        string email = identity.FindFirst(ClaimTypes.Email)?.Value;
        string nombre = identity.FindFirst(ClaimTypes.Name)?.Value;
        string foto = identity.FindFirst("urn:google:picture")?.Value;

        var usuario = db.Usuarios.FirstOrDefault(u => u.Email == email);

        if (usuario == null)
        {
            usuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                FotoPerfilUrl = foto,
                FechaRegistro = DateTime.Now
            };

            db.Usuarios.Add(usuario);
            db.SaveChanges();
        }

        // Crear sesión
        Session["UsuarioId"] = usuario.id;
        Session["UsuarioNombre"] = usuario.Nombre;
        Session["UsuarioFoto"] = usuario.FotoPerfilUrl;

        return RedirectToAction("Index", "Dashboard");
    }

    // Logout
    public ActionResult Logout()
    {
        Session.Clear();
        Session.Abandon();

        HttpContext.GetOwinContext()
            .Authentication.SignOut();

        return RedirectToAction("Login");
    }
}