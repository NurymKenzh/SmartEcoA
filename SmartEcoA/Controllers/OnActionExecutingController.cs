using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Controllers
{
    public class OnActionExecutingController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(filterContext.RouteData.Values["language"].ToString());
        }
    }
}
