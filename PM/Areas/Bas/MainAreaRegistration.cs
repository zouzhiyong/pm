using System.Web.Mvc;

namespace PM.Areas.Main
{
    public class MainAreaRegistration: AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Bas";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                this.AreaName + "default",
                this.AreaName + "/{controller}/{action}/{id}",
                new { area = this.AreaName, controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "PM.Areas." + this.AreaName + ".Controllers" }
            );
        }
    }
}