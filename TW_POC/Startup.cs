using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TW_POC.Startup))]
namespace TW_POC
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
