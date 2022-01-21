using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APPCV_CSE.Datos;
using Tools;
using System.IO;
using System.Web;

namespace APPCV_CSE.Controllers.Api
{
    public class RegistrosApiController : ApiController
    {
        DBJUNTASEntities db = new DBJUNTASEntities();
        
        // GET: api/RegistrosApi
        [HttpGet]
        [Route("api/RegistrosApi/Get")]
        public Object Get()
        {
            var xtipoCV = from a in db.cat_TipoCV
                          select new
                          {
                              id = a.idTipoCV,
                              valor = a.Nombre,
                          };
            var xdpto = from a in db.cat_Departamento 
                        select new {
                            id = a.id_CatDepartamento,
                            valor = a.descripcion
                        };

            var xmuni = from a in db.cat_Municipio
                        select new {

                            id = a.id_CatMunicipio,
                            valor = a.descripcion,
                            iddpto = a.id_CatDepartamento
                        };

            return new  { dpto = xdpto.ToList(), muni = xmuni.ToList(), tipoCV = xtipoCV.ToList() };
        }

        // GET: api/RegistrosApi/5
        [HttpGet]
        [Route("api/RegistrosApi/Get")]
        public Object Get(string CV)
        {
            var lstCV = (from a in db.CentrosVotacion
                        where a.nomCenVotacion == CV
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
                            a.idTipoCV,
                            a.Estado,
                            a.Activo,
                            a.id_CatMunicipio,
                            lstJRV = from a1 in db.JRV
                                     where a1.id_CentroVotacion == a.id_CentroVotacion
                                     select new
                                     {
                                         a1.id_JRV,
                                         a1.codeJRV,
                                         a1.cantInscritos,
                                         a1.cantVerificados,
                                         a1.id_CentroVotacion
                                     },
                            Galeria = (from a1 in db.GaleriaCentroVotacion
                                       where a1.id_CentroVotacion == a.id_CentroVotacion
                                       select new
                                       {
                                           a1.idGaleriaCentro,
                                           a1.fotoUrl,
                                           a1.fechaHoraCreacion,
                                           a1.id_CentroVotacion
                                       }).Count()

                        }).FirstOrDefault();

            return lstCV;
        }

        // POST: api/RegistrosApi
        [HttpPost]
        [Route("api/RegistrosApi/Post")]
        public HttpResponseMessage Post(dtRegCV dtreg)
        {
            Imagenes img;
            string urlimg;
            using (var transaccion = db.Database.BeginTransaction())
            {
                try
                {
                    var existCV = db.CentrosVotacion.ToList().Exists(x => x.nomCenVotacion == dtreg.CV);
                    if (existCV)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "El Centro de Vocacion ya existe por favor revificar");
                    }

                    db.CentrosVotacion.Add(new CentrosVotacion
                    {
                        nomCenVotacion = dtreg.CV,
                        direccionCenVotacion = dtreg.direccion,
                        ubicacionCenVotacion = dtreg.local,
                        derroteroCenVotacion = dtreg.derrotero,
                        circunscripcionCenVotacion = dtreg.circunscripcion,
                        descripcion = dtreg.descripcion,
                        idTipoCV = dtreg.idTipoCV,
                        id_CatMunicipio = dtreg.idMuni,
                        latitude = 0,
                        longitude = 0,
                        //imgPlano = urlimg,
                        //fotoFachada,
                        Estado = "BUENAS CONDICIONES",
                        Activo = true,
                    });
                    db.SaveChanges();

                    var lastid = db.CentrosVotacion.Local.Max(x => x.id_CentroVotacion);
                    List<int> lstid = new List<int>();

                    #region Agregar JRV
                    foreach (var item in dtreg.lstJRV)
                    {
                        addJRV(lastid, item);
                        var lastidJRV = db.JRV.Local.Max(x => x.id_JRV);
                        lstid.Add(lastidJRV);
                    }
                    #endregion
                    #region Agregar fotos
                    foreach (var i in dtreg.lstimg)
                    {
                        img = new Imagenes(i.Split(',')[1], Guid.NewGuid().ToString(), "png", 500, 500);
                        urlimg = "/Images/"+img.SaveFile("Images");

                        if (urlimg.Length == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "NO SE PUDIERON SUBIR LOS ARCHIVOS POR FAVOR CONTACTE CON EL ADMINISTRADOR");
                        }

                        db.GaleriaCentroVotacion.Add(new GaleriaCentroVotacion {
                             idGaleriaCentro = Guid.NewGuid(),
                            fotoUrl= urlimg,
                            fechaHoraCreacion = DateTime.Now,
                            id_CentroVotacion = lastid
                        });
                        db.SaveChanges();
                    }
                    #endregion
               
                    transaccion.Commit();
                    return Request.CreateResponse(HttpStatusCode.OK, new { lastid, lstid });
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.ToString());
                }
            }

        }

        // PUT: api/RegistrosApi/5
        [HttpPut]
        [Route("api/RegistrosApi/Put")]
        public HttpResponseMessage Put(dtRegCV dtreg, int idCV)
        {
            Imagenes img;
            string urlimg, urldelete;
            string serverPath = HttpContext.Current.Server.MapPath("~");

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var modelCV = (from a in db.CentrosVotacion
                                   where a.id_CentroVotacion == idCV
                                   select a).FirstOrDefault();

                    modelCV.nomCenVotacion = dtreg.CV;
                    modelCV.direccionCenVotacion = dtreg.direccion;
                    modelCV.ubicacionCenVotacion = dtreg.local;
                    modelCV.derroteroCenVotacion = dtreg.derrotero;
                    modelCV.circunscripcionCenVotacion = dtreg.circunscripcion;
                    modelCV.descripcion = dtreg.descripcion;
                    modelCV.idTipoCV = dtreg.idTipoCV;
                    modelCV.id_CatMunicipio = dtreg.idMuni;
                   // modelCV.imgPlano = urlimg;

                    List<int> lstid = new List<int>();
                    foreach (var item in dtreg.lstJRV)
                    {
                        if (!item.db)
                        {
                            addJRV(idCV, item);
                            var lastidJRV = db.JRV.Local.Max(x => x.id_JRV);
                            lstid.Add(lastidJRV);
                        }
                        else
                        {
                            var modelJRV = (from a in db.JRV
                                            where a.id_JRV == item.idJRV
                                            select a).FirstOrDefault();

                            modelJRV.codeJRV = item.codigoJVR;
                            modelJRV.cantInscritos = item.cant_inscritos;
                            modelJRV.cantVerificados = item.cant_verificados;
                        }
                    }


                    #region Eliminar fotos
                    var lstimg = from a in db.GaleriaCentroVotacion
                                 where a.id_CentroVotacion == idCV
                                 select a;
                    foreach (var j in lstimg)
                    {
                        urldelete = serverPath + j.fotoUrl;
                        if (File.Exists(urldelete))
                        {
                            File.Delete(urldelete);
                        }
                    }
                    db.GaleriaCentroVotacion.RemoveRange(lstimg);
                    #endregion

                    #region Agregar fotos
                    foreach (var i in dtreg.lstimg)
                    {
                        img = new Imagenes(i.Split(',')[1], Guid.NewGuid().ToString(), "png", 500, 500);
                        urlimg = "/Images/" + img.SaveFile("Images");

                        if (urlimg.Length == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "NO SE PUDIERON SUBIR LOS ARCHIVOS POR FAVOR CONTACTE CON INFORMATICA");
                        }

                        db.GaleriaCentroVotacion.Add(new GaleriaCentroVotacion
                        {
                            idGaleriaCentro = Guid.NewGuid(),
                            fotoUrl = urlimg,
                            fechaHoraCreacion = DateTime.Now,
                            id_CentroVotacion = idCV
                        });
                    }
                    #endregion

                    db.SaveChanges();
                    transaction.Commit();
                    return Request.CreateResponse(HttpStatusCode.OK, new { lstid });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.ToString());
                }
            }
         
        }

        // DELETE: api/RegistrosApi/5
        [HttpDelete]
        [Route("api/RegistrosApi/Delete")]
        public void Delete(int id)
        {
            var modelJRV = (from a in db.JRV
                            where a.id_JRV == id
                            select a).FirstOrDefault();
            db.JRV.Remove(modelJRV);
            db.SaveChanges();
        }

        public void addJRV(int lastid, dtRegJRV item)
        {
            db.JRV.Add(new JRV
            {
                id_CentroVotacion = lastid,
                codeJRV = item.codigoJVR,
                cantInscritos = item.cant_inscritos,
                cantVerificados = item.cant_verificados
            });

            db.SaveChanges();
        }

        [HttpPut]
        [Route("api/RegistrosApi/updateMapaMunicipio")]
        public HttpResponseMessage updateMapaMunicipio(dtIMG_Municipio dt, int idMuni)
        {
            Imagenes img;
            string urlimg, urldelete;
            try
            {
                string serverPath = HttpContext.Current.Server.MapPath("~");

                var muni = (from a in db.cat_Municipio
                            where a.id_CatMunicipio == idMuni
                            select a).FirstOrDefault();

                string mapa =  addplanos(dt.imgMapa, muni.imdMapa);
                string plano = addplanos(dt.imgPlano, muni.imgPlano);

                if (String.IsNullOrEmpty(mapa) || String.IsNullOrEmpty(plano))
                {
                    Request.CreateResponse(HttpStatusCode.BadRequest, "NO SE PUDIERON SUBIR LOS ARCHIVOS DE PLANOS POR FAVOR CONTACTAR AL ADMINISTRADOR");
                }

                muni.imdMapa = mapa;
                muni.imgPlano = plano;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Cambios subidos Correctamente");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        public string addplanos(string data, string url= "1.png") {
            Imagenes img;
            string urlimg, urldelete;
            string serverPath = HttpContext.Current.Server.MapPath("~");

            urldelete = serverPath + url;
            if (File.Exists(urldelete))
            {
                File.Delete(urldelete);
            }

            img = new Imagenes(data.Split(',')[1], Guid.NewGuid().ToString(), "png", 500, 500);
            urlimg = "/Planos/" + img.SaveFile("Planos");

            if (urlimg.Length == 0)
            {
                return "";
            }
            return urlimg;
        }

    }
    public class dtRegCV
    {
        public string idCV { get; set; }
        public string CV { get; set; }
        public string local { get; set; }
        public string direccion { get; set; }
        public string circunscripcion { get; set; }
        public string descripcion { get; set; }
        public string derrotero { get; set; }
        public int idTipoCV { get; set; }
        public int idMuni { get; set; }
        public String imgPlano { get; set; }
        //public int idDpto { get; set; }
        public List<string> lstimg { get; set; }
        public List<dtRegJRV> lstJRV { get; set; }
    }

    public class dtRegJRV {
        public int idJRV { get; set; }
        public string codigoJVR { get; set; }
        public int cant_inscritos { get; set; }
        public int cant_verificados { get; set; }
        public bool db { get; set; }
    }

    public class dtIMG_Municipio {

        public string imgPlano { get; set; }
        public string imgMapa { get; set; }
    }
}
