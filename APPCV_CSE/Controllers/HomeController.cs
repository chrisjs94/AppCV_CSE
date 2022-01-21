using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APPCV_CSE.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Visualizacion()
        {
            return View();
        }
        [Authorize(Roles = "ADMIN")]
        public ActionResult addPlanos()
        {
            return View();
        }
        public ActionResult Consolidado()
        {
            return View();
        }
    }
}