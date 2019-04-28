using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(tds.Startup))]
namespace tds
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
