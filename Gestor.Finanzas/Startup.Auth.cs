using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;


[assembly: OwinStartup(typeof(Gestor.Finanzas.Startup))]

namespace Gestor.Finanzas
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType("ExternalCookie");

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ExternalCookie",
                LoginPath = new PathString("/Account/Login")
            });

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["Google:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["Google:ClientSecret"],
                CallbackPath = new PathString("/signin-google"),
                Scope = { "email", "profile" },
                Provider = new GoogleOAuth2AuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        var picture = context.User.Value<string>("picture");
                        if (!string.IsNullOrEmpty(picture))
                            context.Identity.AddClaim(
                                new Claim("urn:google:picture", picture));

                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
















