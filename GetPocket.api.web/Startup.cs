using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GetPocket.api.web.Startup))]
namespace GetPocket.api.web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
