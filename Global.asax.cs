using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ThemeSwitcher
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
                HttpCookie myCookie = new HttpCookie("Default-Theme");
                myCookie.Value = "0";
                myCookie.Expires = DateTime.Now.AddDays(1d);
                HttpContext.Current.Response.Cookies.Add(myCookie);
          
        }
    }
}
