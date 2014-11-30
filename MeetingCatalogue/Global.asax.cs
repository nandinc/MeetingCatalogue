using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MeetingCatalogue
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

             // namespace for the EdmxWriter class
            using (var ctx = new MeetingCatalogue.DAL.MeetingCatalogueContext())
            {
                using (var writer = new System.Xml.XmlTextWriter(@"c:\Temp\Model.edmx", System.Text.Encoding.Default))
                {
                    System.Data.Entity.Infrastructure.EdmxWriter.WriteEdmx(ctx, writer);
                }
            }
        }
    }
}
