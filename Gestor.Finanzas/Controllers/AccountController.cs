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

    // ===============================
    // VISTA LOGIN
    // ===============================
    public ActionResult Login()
    {
        if (Session["UsuarioId"] != null)
            return RedirectToAction("Index", "Dashboard");

        return View();
    }

    // ===============================
    // INICIAR LOGIN CON GOOGLE
    // ===============================
    public ActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleResponse", "Account")
        };

        HttpContext.GetOwinContext()
            .Authentication
            .Challenge(properties, "Google");

        return new HttpUnauthorizedResult();
    }

    // ===============================
    // RESPUESTA DE GOOGLE
    // ===============================
    public ActionResult GoogleResponse()
    {
        var auth = HttpContext.GetOwinContext().Authentication;
        var identity = auth.AuthenticateAsync("ExternalCookie").Result?.Identity;

        if (identity == null)
            return RedirectToAction("Login");

        // Claims principales
        string email = identity.FindFirst(ClaimTypes.Email)?.Value;
        string nombre = identity.FindFirst(ClaimTypes.Name)?.Value;
        string foto = identity.FindFirst("urn:google:picture")?.Value;

        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login");

        // Buscar usuario
        var usuario = db.Usuarios.FirstOrDefault(u => u.Email == email);

        if (usuario == null)
        {
            usuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                FechaRegistro = DateTime.Now
            };

            db.Usuarios.Add(usuario);
        }

        // Actualizar datos SIEMPRE
        usuario.Nombre = nombre;
        usuario.FotoPerfilUrl = foto;

        db.SaveChanges();

        // ===============================
        // CREAR SESIÓN
        // ===============================
        Session["UsuarioId"] = usuario.Id;
        Session["UsuarioNombre"] = usuario.Nombre;
        Session["UsuarioFoto"] = usuario.FotoPerfilUrl;

        return RedirectToAction("Index", "Dashboard");
    }

    // ===============================
    // LOGOUT
    // ===============================
    public ActionResult Logout()
    {
        Session.Clear();
        Session.Abandon();

        HttpContext.GetOwinContext()
            .Authentication
            .SignOut("ExternalCookie");

        return RedirectToAction("Login");
    }
}