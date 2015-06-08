using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VC_REST_Discover_Explorer.Startup))]
namespace VC_REST_Discover_Explorer
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
