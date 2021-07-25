using SAM.Web.Autorizacao;
using SAM.Web.Models;
using SAM.Web.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SAM.Web.Controllers
{
    public class ApiManagerUnitController : ApiController
    {
        private SAMContext Context = null;
        private readonly ManagerUnitServices services = null;
        public ApiManagerUnitController()
        {
            this.Context = new SAMContext();
            services = new ManagerUnitServices(this.Context);
        }
        //NET 4.5 - Sem a Anotação [Route]
        //api/ApiManagerUnit?managerUnitCode=512701
        //NET 4.5.1 * ou NET Core
        //api/ApiManagerUnit/512701
        [HttpGet]
        [Route("api/ManagerUnit/{managerUnitCode:int}")]
        [AuthorizeWebApiAttribute]
        public async Task<IHttpActionResult> Get(int managerUnitCode)
        {
            try
            {
                var resultado = await services.ConsultarPorCodigo(managerUnitCode.ToString());
                return Ok(resultado);
            }catch(Exception ex)
            {
                return Ok(ex.Message);
            }

        }
        //NET 4.5 - Sem a Anotação [Route]
        //api/ApiManagerUnit?managerUnitCode=512701&DataReferencia
        //NET 4.5.1 * ou NET Core
        //api/ApiManagerUnit/512701/DataReferencia
        [HttpGet]
        [Route("api/ManagerUnit/{managerUnitCode:int}/DataReferencia")]
        [AuthorizeWebApiAttribute]
        public async Task<IHttpActionResult> GetDataReferencia(int managerUnitCode)
        {
            try
            {
                var resultado = await services.GetDataReferencia(managerUnitCode.ToString());
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }
    }
}
