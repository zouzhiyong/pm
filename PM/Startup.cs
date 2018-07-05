using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PM.Startup))]
namespace PM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
