using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(master.framework.web.js.Startup))]
namespace master.framework.web.js
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
