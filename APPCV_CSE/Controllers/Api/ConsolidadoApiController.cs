using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APPCV_CSE.Datos;

namespace APPCV_CSE.Controllers.Api
{
    public class ConsolidadoApiController : ApiController
    {

        DBJUNTASEntities db = new DBJUNTASEntities();
        [HttpGet]
        [Route("api/ConsolidadoApi/Get")]
        public object Get()
        {
            try
            {
                List<object> lstconsolidado = new List<object>();

                var consolidado = from a in db.cat_Departamento
                                  select new {
                                      id = a.id_CatDepartamento,
                                      Dpto = a.nomDepartamento,
                                    
                                  };


                foreach (var item in consolidado) {

                    var listaDpto = db.Database.SqlQuery<TotalConsolidado_Result>("exec TotalConsolidado " + item.id)
                                                 .Select(b => new
                                                 {
                                                     b.idDpto,
                                                     b.municipio,
                                                     b.URBANOCV,
                                                     b.RURALCV,
                                                     b.TOTALCV,
                                                     b.URBANOJRV,
                                                     b.RURALJRV,
                                                     b.TOTALJRV,
                                                     b.URBANOINSCRITOS,
                                                     b.RURALINSCRITOS,
                                                     b.TOTALINSCRITOS,
                                                 }).ToList();

                    lstconsolidado.Add(new {
                        item.id,
                        item.Dpto,
                        listaDpto
                    });
                }

                return lstconsolidado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
