using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;

/// <summary>
/// descomentar o anotação a baixo para o login sp funcionar
/// </summary>
[assembly: OwinStartup(typeof(SAM.Web.Startup))]

namespace SAM.Web
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                CookieManager = new Microsoft.Owin.Host.SystemWeb.SystemWebChunkingCookieManager()


            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "(Id para acesso)",
                Authority = Constants.OpenIdConnectBaseAddress.BaseAddress,
                RedirectUri = "(uri para acesso ao sistema)",
                PostLogoutRedirectUri = "(uri para redirecionar caso saia ao sistema)",
                ResponseType = "code id_token",//
                Scope = "openid profile offline_access",
                ClientSecret = "4fe500b4-f9f2-470f-9db1-8d63d24d0615",


                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "cpf",
                    RoleClaimType = "role"

                },

                SignInAsAuthenticationType = "Cookies",

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async n =>
                    {
                        // use the code to get the access and refresh token
                        var tokenClient = new TokenClient(
                            Constants.OpenIdConnectBaseAddress.TokenEndpoint,
                            "sam.patrimonio",
                            "4fe500b4-f9f2-470f-9db1-8d63d24d0615");

                        var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(
                            n.Code, n.RedirectUri);

                        if (tokenResponse.IsError)
                        {
                            throw new Exception(" Erro: " + tokenResponse.Error);
                        }

                        // use the access token to retrieve claims from userinfo
                        var userInfoClient = new UserInfoClient(
                        new Uri(Constants.OpenIdConnectBaseAddress.UserInfoEndpoint),
                        tokenResponse.AccessToken);

                        var userInfoResponse = await userInfoClient.GetAsync();

                        // create new identity
                        var id = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
                        id.AddClaims(userInfoResponse.GetClaimsIdentity().Claims);

                        id.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
                        id.AddClaim(new Claim("expires_at", DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToLocalTime().ToString()));
                        id.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
                        id.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        id.AddClaim(new Claim("sid", n.AuthenticationTicket.Identity.FindFirst("sid").Value));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            new ClaimsIdentity(id.Claims, n.AuthenticationTicket.Identity.AuthenticationType, "cpf", "role"),
                            n.AuthenticationTicket.Properties);
                    },
                    AuthenticationFailed = async n =>
                    {
                        n.HandleResponse();
                        var context = n.OwinContext.Response;
                        context.Redirect("(uri para acesso ao sistema)");
                    },
                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }

                        }

                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}

