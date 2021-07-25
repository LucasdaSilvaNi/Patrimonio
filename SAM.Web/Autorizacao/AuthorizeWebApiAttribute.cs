using SAM.Web.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SAM.Web.Autorizacao
{
    public class AuthorizeWebApiAttribute : AuthorizeAttribute
    {
        private string keyAuthorize = "sam_patrimonio_estoque";
        private readonly AuthorizeServices services = null;

        public AuthorizeWebApiAttribute()
        {
            services = new AuthorizeServices();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

        }

        
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            base.IsAuthorized(actionContext);
            try
            {
                var content = actionContext.Request.Headers.Where(x => x.Key.Equals(this.keyAuthorize)).FirstOrDefault();

                var resultado = services.ValidateToken(content.Value.FirstOrDefault());

                if (resultado)
                    return true;
            }catch(Exception ex)
            {
               return false;
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
        }
    }
}