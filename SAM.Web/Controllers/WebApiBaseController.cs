using Newtonsoft.Json;
using PatrimonioBusiness.integracao.abstracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class WebApiBaseController : ApiController
    {
        protected readonly CultureInfo cultureInfo = new CultureInfo("pt-br");
        protected readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
        protected JsonResult JsonResultado = null;
        protected MovimentoIntegracaoAbstract  movimentoIntegracaoAbstract = null;

        /// <summary>
        /// Isolamento sem restrições(Padrão)
        /// </summary>
        protected IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted;
    }
}
