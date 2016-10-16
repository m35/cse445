using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TryIt22.Startup))]
namespace TryIt22
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
