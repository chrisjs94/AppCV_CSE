using System;
using System.Web.Mvc;
using APPCV_CSE.Datos;
using APPCV_CSE.Models;
using System.Linq;
using System.Collections.Generic;

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
                    if (data.radioBuscarPor == "1") //Cedula de identidad
                    {
                        var qryporCedula = db.PADRON.Where(cond => cond.ci.ToUpper() == data.txtSearch.ToUpper()).ToList();
                        return Json(qryporCedula, JsonRequestBehavior.AllowGet);
                    }
                    else if (data.radioBuscarPor == "2") //Nombre y/o apellido
                    {
                        var qryporNombre = db.PADRON.Where(cond =>
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
                    else
                        return Json(new List<PADRON>(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new ArgumentNullException("data.txtSearch is null");
                }
            }
        }
    }
}