using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using APPCV_CSE.Datos;

namespace APPCV_CSE.Controllers.API
{
    public class VisualizacionApiController : ApiController
    {
        DBJUNTASEntities db = new DBJUNTASEntities();
        [HttpGet]
      //  [Route("VisualizacionApi")]
        public async Task<HttpResponseMessage> GET() 
        {
            db.Database.CommandTimeout = 300;
            db.Database.Log = Console.Write;
            var muni = (from a in db.cat_Municipio
                       select new {
                           id = a.id_CatMunicipio,
                           valor = a.nomMunicipio,
                           iddpto = a.id_CatDepartamento,
                           imgPlano = a.imgPlano,
                           imgMapa = a.imdMapa,
                       }).ToList();

            var dptos= (from a in db.cat_Departamento
                       select new {
                           id = a.id_CatDepartamento,
                           valor = a.nomDepartamento
                       }).ToList();

            var lstCV = await Task.Run(() => (from a in db.CentrosVotacion
                                        select new
                                        {
                                            a.id_CentroVotacion,
                                            a.nomCenVotacion,
                                            a.direccionCenVotacion,
                                            a.ubicacionCenVotacion,
                                            a.derroteroCenVotacion,
                                            a.circunscripcionCenVotacion,
                                            a.descripcion,
                                            a.latitude,
                                            a.longitude,
                                            a.fotoFachada,
                                            a.Estado,
                                            a.Activo,
                                            a.id_CatMunicipio,
                                            a.idTipoCV,
                                            lstJRV = (from a1 in db.JRV
                                                      where a1.id_CentroVotacion == a.id_CentroVotacion
                                                      select new
                                                      {
                                                          a1.id_JRV,
                                                          a1.codeJRV,
                                                          a1.cantInscritos,
                                                          a1.cantVerificados,
                                                          a1.id_CentroVotacion
                                                      }),
                                            Galeria = (from a2 in db.GaleriaCentroVotacion
                                                       where a2.id_CentroVotacion == a.id_CentroVotacion
                                                       select new
                                                       {
                                                           a2.idGaleriaCentro,
                                                           a2.fotoUrl,
                                                           a2.fechaHoraCreacion,
                                                           a2.id_CentroVotacion
                                                       })
                                        }).ToList());

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                muni,
                dptos,
                lstCV
            });
        }

        ~VisualizacionApiController()
        {
            this.db.Database.Connection.Close();
            this.db.Dispose();
        }
    }
}
