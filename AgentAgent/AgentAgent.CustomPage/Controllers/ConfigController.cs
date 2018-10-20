using AgentAgent.CustomPage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgentAgent.CustomPage.Controllers
{
    public class ConfigController : Controller
    {
        // GET: Config
        public ActionResult Index()
        {
            Config config = new Config()
            {
                PoolId = 12345,
                ServerAdjustmentFactor = 10,
                IgnoreSearchServer = false,
                UseApiCreate = true,
                UseApiDelete = false       

            };

            return View(config);
        }

        public ActionResult Number(int num)
        {
            return Content("Your number is: " + num);
        }

    }
}