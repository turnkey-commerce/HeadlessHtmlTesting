using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelloMe.Web.Controllers
{
    public class HelloController : Controller
    {
        //
        // GET: /Hello/

        public ActionResult Index()
        {
            return View();
        }


        //
        // GET: /Hello/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Hello/Create

        [HttpPost]
        public ActionResult Create(string name)
        {
            ViewBag.Name = name;
            return View();
        }
        
    }
}
