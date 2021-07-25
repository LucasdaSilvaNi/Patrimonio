
//using IdentityModel;
//using IdentityModel.Client;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json.Linq;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace SAM.Web.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        private UsuarioContext db;
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult IndexAntigo(string returnUrl)
        {
            //Caso usuário já esteja logado e tenta acessar a tela de login,
            // é redirecionado para a tela de Aviso Geral
            if (HttpContext.User.Identity.IsAuthenticated && Session["UsuarioAutenticado"] != null)
            {
                return RedirectToAction("Index", "Principal");
            }
           

            return View();
        }
        // GET: /Account/Login
        // retirar sufixo LoginSP
        [Authorize]
        public async Task<ActionResult> Index(string returnUrl)
        {
  
            ViewBag.Message = "Claims";

            var cp = (ClaimsPrincipal)User;
           // ViewData["access_token"] = cp.FindFirst("access_token").Value;
            var cpf = cp.Claims.Where(x => x.Type == "cpf").FirstOrDefault();

            var retorno = GetUsuario(cpf.Value);

            if (retorno != null)
            {

                AntiForgeryConfig.UniqueClaimTypeIdentifier = "cpf";

                FormsAuthentication.SetAuthCookie(retorno.CPF, false);
                if (!retorno.ChangePassword)
                {
                    Session["UsuarioAutenticado"] = retorno;
                    return RedirectToAction("Index", "Principal");
                }
                else
                    return RedirectToAction("ChangePassword", "Login", retorno);
            }
            return View();

        }
        //public async Task<ActionResult> RefreshToken()
        //{
        //    var client = new TokenClient(
        //        Constants.OpenIdConnectBaseAddress.TokenEndpoint,
        //        "sam.teste",
        //        "4fe500b4-f9f2-470f-9db1-8d63d24d0615");

        //    var principal = User as ClaimsPrincipal;
        //    var refreshToken = principal.FindFirst("refresh_token").Value;

        //    var response = await client.RequestRefreshTokenAsync(refreshToken);
        //    UpdateCookie(response);

        //    return RedirectToAction("Index");
        //}

        //public async Task<ActionResult> RevokeAccessToken()
        //{
        //    var accessToken = (User as ClaimsPrincipal).FindFirst("access_token").Value;
        //    var client = new HttpClient();
        //    client.SetBasicAuthentication("sam.teste", "4fe500b4-f9f2-470f-9db1-8d63d24d0615");

        //    var postBody = new Dictionary<string, string>
        //    {
        //        { "token", accessToken },
        //        { "token_type_hint", "access_token" }
        //    };

        //    var result = await client.PostAsync(Constants.OpenIdConnectBaseAddress.TokenRevocationEndpoint, new FormUrlEncodedContent(postBody));

        //    return RedirectToAction("Index");
        //}

        //public async Task<ActionResult> RevokeRefreshToken()
        //{
        //    var refreshToken = (User as ClaimsPrincipal).FindFirst("refresh_token").Value;
        //    var client = new HttpClient();
        //    client.SetBasicAuthentication("sam.teste", "4fe500b4-f9f2-470f-9db1-8d63d24d0615");

        //    var postBody = new Dictionary<string, string>
        //    {
        //        { "token", refreshToken },
        //        { "token_type_hint", "refresh_token" }
        //    };

        //    var result = await client.PostAsync(Constants.OpenIdConnectBaseAddress.TokenRevocationEndpoint, new FormUrlEncodedContent(postBody));

        //    return RedirectToAction("Index");
        //}

        //private void UpdateCookie(TokenResponse response)
        //{
        //    //if (response.IsError)
        //    //{
        //    //    throw new Exception(response.Error);
        //    //}

        //    var identity = (User as ClaimsPrincipal).Identities.First();
        //    var result = from c in identity.Claims
        //                 where c.Type != "access_token" &&
        //                       c.Type != "refresh_token" &&
        //                       c.Type != "expires_at"
        //                 select c;

        //    var claims = result.ToList();

        //    claims.Add(new Claim("access_token", response.AccessToken));
        //    claims.Add(new Claim("expires_at", (DateTime.UtcNow.ToEpochTime() + response.ExpiresIn).ToDateTimeFromEpoch().ToString()));
        //    claims.Add(new Claim("refresh_token", response.RefreshToken));

        //    var newId = new ClaimsIdentity(claims, "Cookies");
        //    Request.GetOwinContext().Authentication.SignIn(newId);
        //}
        private User GetUsuario(string cpf)
        {

            var _cpf = cpf.Replace(".", string.Empty).Replace("-", string.Empty);

            db = new UsuarioContext();
            this.db.Configuration.LazyLoadingEnabled = false;
            this.db.Configuration.AutoDetectChangesEnabled = false;

            //Inserir controle de criptografia
            var query = db.Users.Where(m => m.CPF == _cpf && m.Status == true).AsQueryable();
            var retorno = query.FirstOrDefault();
            return retorno;


        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                model.Senha = SAM.Web.Common.UserCommon.EncryptPassword(model.Senha);
                //Convert string CPF to only numbers
                model.CPF = model.CPF.Replace(".", string.Empty).Replace("-", string.Empty);

                db = new UsuarioContext();
                this.db.Configuration.LazyLoadingEnabled = false;
                this.db.Configuration.AutoDetectChangesEnabled = false;

                //Inserir controle de criptografia
                var query = db.Users.Where(m => m.CPF == model.CPF && m.Password == model.Senha && m.Status == true).AsQueryable();
                var retorno = query.FirstOrDefault();

                if (retorno != null)
                {
                    Session["loginSP"] = false;

                    FormsAuthentication.SetAuthCookie(model.CPF, false);
                    if (!retorno.ChangePassword)
                    {
                        Session["UsuarioAutenticado"] = retorno;
                        //ImportarIntegracao();
                     
                        return RedirectToAction("Index", "Principal");
                    }
                    else
                        return RedirectToAction("ChangePassword", "Login", retorno);
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Suas credenciais estão incorretas");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ChangePassword(User user)
        {
            FormsAuthentication.SignOut();
            ChangePasswordViewModel _model = new ChangePasswordViewModel();
            _model.userId = user.Id;
            _model.Password = user.Password;

            return View(_model);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangePassword([Bind(Include = "Password, NewPassword, ConfirmPassword,userId")] ChangePasswordViewModel model)
        {
            db = new UsuarioContext();
            User user = (from q in db.Users where q.Id == model.userId select q).AsQueryable().FirstOrDefault();
            user.Password = SAM.Web.Common.UserCommon.EncryptPassword(model.NewPassword);
            user.ChangePassword = false;
            db.SaveChanges();

            FormsAuthentication.SignOut();
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            RemoveCookieLoginSp();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Sair()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            RemoveCookieLoginSp();

            return RedirectToAction("Index", "Home");
        }
        public void SignoutCleanup(string sid)
        {
            var cp = (ClaimsPrincipal)User;
            var sidClaim = cp.FindFirst("sid");
            if (sidClaim != null && sidClaim.Value == sid)
            {
                Request.GetOwinContext().Authentication.SignOut("Cookies");
            }
        }
        //[Authorize]
        //public async Task<ActionResult> CallApi()
        //{
        //    var token = (User as ClaimsPrincipal).FindFirst("access_token").Value;

        //    var client = new System.Net.Http.HttpClient();
        //    client.SetBearerToken(token);

        //    var result = await client.GetStringAsync(Constants.OpenIdConnectBaseAddress.AspNetWebApiSampleApi + "identity");
        //    ViewBag.Json = JArray.Parse(result.ToString());

        //    return View();
        //}
        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private void RemoveCookieLoginSp()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                                                              CookieAuthenticationDefaults.AuthenticationType,
                                                              OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }
        #endregion
    }
}