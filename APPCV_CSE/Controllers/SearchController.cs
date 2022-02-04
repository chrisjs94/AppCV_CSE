using System;
using System.Web.Mvc;
using APPCV_CSE.Datos;
using APPCV_CSE.Models;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;

namespace APPCV_CSE.Controllers
{
    public class SearchController : Controller
    {
        private DBJUNTASEntities db;
        public SearchController()
        {
            db = new DBJUNTASEntities();
        }

        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(BusquedaPersona data)
        {
            db.Database.CommandTimeout = 500;
            if (data == null)
            {
                throw new ArgumentNullException("data object is null");
            }
            else
            {
                if (!String.IsNullOrEmpty(data.txtSearch))
                {
                    var qry = from x in db.vwPADRON
                              select new
                              {
                                  x.ci,
                                  nombrecompleto = x.papell + " " + x.sappell + ", " + x.pnom + " " + x.snom,
                                  x.papell,
                                  x.sappell,
                                  x.pnom,
                                  x.snom,
                                  x.domicilio,
                                  x.lugar_nac,
                                  fecha_nac = SqlFunctions.DatePart("dd", x.fecha_nac) + " de " + SqlFunctions.DateName("MM", x.fecha_nac) + " de " + SqlFunctions.DatePart("yyyy", x.fecha_nac),
                                  x.jrv,
                                  x.cantInscritos,
                                  x.cantVerificados,
                                  x.sexo,
                                  x.latitude,
                                  x.longitude,
                                  galeria = from y in db.GaleriaCentroVotacion where y.id_CentroVotacion == x.id_CentroVotacion select y.fotoUrl
                              };
                    string strqury = qry.ToString();
                    if (data.radioBuscarPor == "1") //Cedula de identidad
                    {
                        var qryporCedula = qry.Where(cond => cond.ci == data.txtSearch.ToUpper()).ToList();
                        return Json(qryporCedula, JsonRequestBehavior.AllowGet);
                    }
                    else if (data.radioBuscarPor == "2") //Nombre y/o apellido
                    {
                        var qryporNombre = qry.Where(cond =>
                        new string[]
                        {
                            (cond.pnom + cond.snom + cond.papell + cond.sappell).Replace(" ", "").ToUpper(),
                            (cond.papell + cond.sappell + cond.pnom + cond.snom).Replace(" ", "").ToUpper(),
                            (cond.papell + cond.sappell + cond.pnom).Replace(" ", "").ToUpper(),
                            (cond.pnom + cond.papell).Replace(" ", "").ToUpper(),
                            (cond.papell + cond.sappell).Replace(" ", "").ToUpper(),
                            (cond.papell + cond.pnom).Replace(" ", "").ToUpper(),
                            (cond.papell + cond.snom).Replace(" ", "").ToUpper(),
                        }.Any(a => a.Contains(data.txtSearch.ToUpper().Replace(" ", ""))));

                        return Json(qryporNombre, JsonRequestBehavior.AllowGet);
                    }
                    else if (data.radioBuscarPor == "3")
                    {
                        int jrv = int.Parse(data.txtSearch);
                        var qryPorCV = qry.Where(cond => cond.jrv == jrv);

                        return Json(qryPorCV, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new List<vwPADRON>(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new ArgumentNullException("data.txtSearch is null");
                }
            }
        }
    }
}